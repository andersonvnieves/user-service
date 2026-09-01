using br.com.fiap.cloudgames.Users.Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace br.com.fiap.cloudgames.Users.WebAPI.Setup
{
    public static class DatabaseInitializerExtensions
    {
        public static async Task InitializeDatabaseAsync(this WebApplication app)
        {
            var logger = app.Services.GetRequiredService<ILogger<Program>>();
            var maxRetries = 10;
            var delay = TimeSpan.FromSeconds(5);

            for (int i = 0; i < maxRetries; i++)
            {
                try
                {
                    logger.LogInformation("Trying to apply DB Migrations (Attempt {Attempt}/{Max})...", i + 1, maxRetries);                    
                    using (var scope = app.Services.CreateScope())
                    {
                        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                        await dbContext.Database.MigrateAsync();
                    }

                    logger.LogInformation("Migrations successfully applies. Starting Seed Identity...");
                    using (var scope = app.Services.CreateScope())
                    {
                        var services = scope.ServiceProvider;
                        var configuration = services.GetRequiredService<IConfiguration>();
                        await IdentitySeeder.SeedRoles(services, configuration);
                        await IdentitySeeder.SeedBootstrapUser(services, configuration);
                    }

                    logger.LogInformation("Seed Identity sucessfully created.");
                    break;
                }
                catch (Exception ex)
                {
                    if (i == maxRetries - 1)
                    {
                        logger.LogError(ex, "Max attemepts reached, failed to initialize DB");
                        throw;
                    }

                    logger.LogWarning(ex, "Failed to connect to SQL database. Retryng in {Seconds} seconds...", delay.TotalSeconds);
                    await Task.Delay(delay);
                }
            }
        }
    }
}
