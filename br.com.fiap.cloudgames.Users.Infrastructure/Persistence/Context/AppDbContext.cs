using br.com.fiap.cloudgames.Users.Domain.Aggregates;
using br.com.fiap.cloudgames.Users.Infrastructure.Persistence.Configurations;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace br.com.fiap.cloudgames.Users.Infrastructure.Persistence.Context;

public class AppDbContext : IdentityDbContext<IdentityUser>
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    { }
  
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfiguration(new UserConfiguration());
    }
    
    public DbSet<User> Users { get; set; }
}