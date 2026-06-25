using br.com.fiap.cloudgames.Domain.Aggregates;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace br.com.fiap.cloudgames.Infrastructure.Persistence.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.HasKey(x => x.Id);
        builder.OwnsOne(u => u.Name, name =>
        {
            name.Property(n => n.FirstName)
                .IsRequired()
                .HasMaxLength(100)
                .HasColumnName("FirstName");
            name.Property(n => n.LastName)
                .IsRequired()
                .HasMaxLength(100)
                .HasColumnName("LastName");
        });
        builder.OwnsOne(u => u.Email, email =>
        {
            email.Property(e => e.Email)
                .IsRequired()
                .HasMaxLength(254)
                .HasColumnName("Email");
        });
        builder.Property(u => u.UserAccountStatus)
            .HasConversion<string>()
            .IsRequired();
        builder.Property(u => u.IdentityId)
            .IsRequired()
            .HasMaxLength(100);
        builder.Property(u => u.CreationDate)
            .IsRequired();
        builder.Property(u => u.Role)
            .HasConversion<string>()
            .IsRequired();
        
        builder.HasIndex(u => u.IdentityId).IsUnique();
    }
}