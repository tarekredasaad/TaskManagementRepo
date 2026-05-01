using Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MyProject.Infrastructure.Repositories.User;

namespace MyProject.Infrastructure.Services;

public static class DatabaseSeeder
{
    public static async Task SeedAsync(IServiceProvider services)
    {
        using var scope = services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var userRepository = scope.ServiceProvider.GetRequiredService<IUserRepository>();
        var passwordHasher = scope.ServiceProvider.GetRequiredService<IPasswordHasher<User>>();
        var logger = scope.ServiceProvider.GetRequiredService<ILoggerFactory>().CreateLogger("DatabaseSeeder");

        await db.Database.EnsureCreatedAsync();

        var existingAdmin = await userRepository.GetByEmailAsync("admin@example.com");
        if (existingAdmin is not null)
        {
            return;
        }

        var admin = new User
        {
            Name = "System Admin",
            Email = "admin@example.com",
            Role = "Admin",
            CreatedAt = DateTime.UtcNow
        };
        admin.PasswordHash = passwordHasher.HashPassword(admin, "Admin@123");

        await userRepository.AddAsync(admin);
        await db.SaveChangesAsync();

        logger.LogInformation("Default admin user seeded.");
    }
}
