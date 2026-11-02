using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MovieTicketBookingSystem.Constants;
using MovieTicketBookingSystem.Data;
using MovieTicketBookingSystem.Models;

namespace MovieTicketBookingSystem.Data;

public static class RoleSeeder
{
    public static async Task SeedAsync(
        IServiceProvider services,
        IConfiguration configuration)
    {
        using var scope = services.CreateScope();

        var context = scope.ServiceProvider
            .GetRequiredService<ApplicationDbContext>();

        var passwordHasher = scope.ServiceProvider
            .GetRequiredService<IPasswordHasher<User>>();

        var accounts = new[]
        {
            new
            {
                FullName = "System Admin",
                Email = "admin@moviebooking.com",
                Role = UserRoles.Admin,
                Password = configuration["SeedUsers:AdminPassword"]
            },
            new
            {
                FullName = "Theatre Manager",
                Email = "manager@moviebooking.com",
                Role = UserRoles.TheatreManager,
                Password = configuration["SeedUsers:ManagerPassword"]
            }
        };

        foreach (var account in accounts)
        {
            if (string.IsNullOrWhiteSpace(account.Password))
                throw new InvalidOperationException(
                    $"Password configuration missing for {account.Email}");

            var existingUser = await context.Users
                .FirstOrDefaultAsync(x => x.Email == account.Email);

            if (existingUser is not null)
                continue;

            var user = new User
            {
                FullName = account.FullName,
                Email = account.Email,
                Role = account.Role
            };

            user.PasswordHash = passwordHasher.HashPassword(
                user,
                account.Password);

            context.Users.Add(user);
        }

        await context.SaveChangesAsync();
    }
}
