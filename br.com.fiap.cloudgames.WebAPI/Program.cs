using br.com.fiap.cloudgames.Application.Publishers;
using br.com.fiap.cloudgames.Application.Services;
using br.com.fiap.cloudgames.Application.UnitsOfWork;
using br.com.fiap.cloudgames.Application.UseCases.User.ChangeUserRole;
using br.com.fiap.cloudgames.Application.UseCases.User.LogIn;
using br.com.fiap.cloudgames.Application.UseCases.User.RegisterUser;
using br.com.fiap.cloudgames.Application.UseCases.User.RetrieveUser;
using br.com.fiap.cloudgames.Domain.Repositories;
using br.com.fiap.cloudgames.Infrastructure.Config;
using br.com.fiap.cloudgames.Infrastructure.Messagging;
using br.com.fiap.cloudgames.Infrastructure.Messaging.Publishers;
using br.com.fiap.cloudgames.Infrastructure.Persistence;
using br.com.fiap.cloudgames.Infrastructure.Persistence.Context;
using br.com.fiap.cloudgames.Infrastructure.Persistence.Repositories;
using br.com.fiap.cloudgames.Infrastructure.Service;
using br.com.fiap.cloudgames.WebAPI.Middlewares;
using br.com.fiap.cloudgames.WebAPI.Setup;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Logging
builder.Logging.ClearProviders();
builder.Logging.AddSimpleConsole(options =>
{
    options.IncludeScopes = true;
    options.SingleLine = true;
    options.TimestampFormat = "yyyy-MM-ddTHH:mm:ss.fffzzz ";
});

//Settings
builder.Services.Configure<JwtTokenSettings>(builder.Configuration.GetSection("Jwt"));
builder.Services.Configure<RabbitMqSettings>(builder.Configuration.GetSection("RabbitMQ"));

//Add Db Context
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("Default")), ServiceLifetime.Scoped );

//Identity
builder.Services.AddIdentity<IdentityUser, IdentityRole>(option =>
    {
        option.Password.RequireUppercase = true;
        option.Password.RequireLowercase = true;
        option.Password.RequireDigit = true;
        option.Password.RequireNonAlphanumeric = true;
        option.Password.RequiredLength = 8;
        option.User.RequireUniqueEmail = true;
    })
    .AddEntityFrameworkStores<AppDbContext>()
    .AddApiEndpoints();

//Authentication
builder.Services.AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = "Bearer";
        options.DefaultChallengeScheme = "Bearer";
    })
    .AddJwtBearer("Bearer", options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters()
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"])),
            RoleClaimType = ClaimTypes.Role
        };
    });

//Authorization
builder.Services.AddAuthorization();

//Repositories
builder.Services.AddScoped<IUserRepository, UserRepository>();

//UnitOfWork
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

//Messaging
builder.Services.AddSingleton<RabbitMqConnection>();
//builder.Services.AddScoped<IMessagePublisher, RabbitMqMessagePublisher>();
builder.Services.AddScoped<IUserCreatedEventPublisher, UserCreatedEventPublisher>();

//UseCases
builder.Services.AddScoped<LogInUseCase>();
builder.Services.AddScoped<RegisterUserUseCase>();
builder.Services.AddScoped<ChangeUserRoleUseCase>();
builder.Services.AddScoped<RetrieveUserUseCase>();

//Services
builder.Services.AddScoped<IUserAuthService, IdentityUserAuthService>();
builder.Services.AddScoped<ITokenService, JwtTokenService>();

builder.Services.AddControllers();

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

// Add Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new()
    {
        Title = "FIAP Cloud Games (FCG)",
        Version = "v1",
        Description = "API de jogos e usuários"
    });
});

var app = builder.Build();

//Run Migrations
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    await dbContext.Database.MigrateAsync();
}

//Seed Identity
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var configuration = services.GetRequiredService<IConfiguration>();
    await IdentitySeeder.SeedRoles(services, configuration);
    await IdentitySeeder.SeedBootstrapUser(services, configuration);
}

app.UseRequestLoggingMiddleware();
app.UseErrorHandlingMiddleware();

//Map Identity Endpoints
//app.MapIdentityApi<IdentityUser>();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI();
}

//app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
