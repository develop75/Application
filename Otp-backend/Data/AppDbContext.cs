using Otp.Models;
using Microsoft.EntityFrameworkCore;

namespace Otp.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<User> Users => Set<User>();
    public DbSet<OtpCode> OtpCodes => Set<OtpCode>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // User
        modelBuilder.Entity<User>(e =>
        {
            e.HasKey(u => u.Id);
            e.HasIndex(u => u.Username).IsUnique();
            e.Property(u => u.Username).HasMaxLength(100).IsRequired();
            e.Property(u => u.PasswordHash).HasMaxLength(256).IsRequired();
            e.Property(u => u.DisplayName).HasMaxLength(200);
        });

        // OtpCode
        modelBuilder.Entity<OtpCode>(e =>
        {
            e.HasKey(o => o.Id);
            e.HasIndex(o => o.UserId);
            e.Property(o => o.Code).HasMaxLength(10).IsFixedLength().IsRequired();
            e.HasOne(o => o.User)
             .WithMany(u => u.OtpCodes)
             .HasForeignKey(o => o.UserId)
             .OnDelete(DeleteBehavior.Cascade);
        });

        // Seed: utente di test (password: "2024!")
        // Hash generato con BCrypt.Net: BCrypt.HashPassword("2024!")
        modelBuilder.Entity<User>().HasData(new User
        {
            Id = 1,
            Username = "admin",
            PasswordHash = "$2a$11$K5Ld0fFZfLH1X5u5T8mX9.2z7jQv3uK1wYp8nMcR6sA4dVbE0LjAi",
            DisplayName = "Amministratore",
            IsActive = true,
            CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc)
        });
    }
}
