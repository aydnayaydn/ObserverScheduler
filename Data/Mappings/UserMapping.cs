using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ObserverScheduler.Entities;
using ObserverScheduler.Helper;

namespace ObserverScheduler.Data.Mappings;
public class UserMapping
{
    public static void OnModelCreating(EntityTypeBuilder<User> builder)
    {
        _ = builder.Property(u => u.UserName)
            .HasMaxLength(200)
            .IsRequired();

        _ = builder.Property(u => u.Role)
            .HasMaxLength(50)
            .IsRequired();

        _ = builder.Property(u => u.ApiKey)
            .HasMaxLength(100)
            .IsRequired();

        _ = builder.Property(u => u.ApiKeyTag)
            .HasMaxLength(100)
            .IsRequired();

        SeedData(builder);
    }

    private static void SeedData(EntityTypeBuilder<User> modelBuilder)
    {
        modelBuilder.HasData(
            new User
            {
                Id = Guid.Parse("E0CB33F3-591A-4A25-AABA-BD05F796B5FB"),
                UserName = "observer",
                Role = "admin",
                ApiKey = EncryptionHelper.Encrypt("fire_1qaz2wsx3edc4rfv", out string tag),
                ApiKeyTag = tag
            }
        );
    }
}