using Microsoft.AspNetCore.Identity;
using MovieTicketBookingSystem.Constants;
using MovieTicketBookingSystem.DTOs.Auth;
using MovieTicketBookingSystem.Models;
using MovieTicketBookingSystem.Repositories.Interfaces;
using MovieTicketBookingSystem.Services.Interfaces;
using System.Data;

namespace MovieTicketBookingSystem.Services;

public class UserService(
    IUserRepository repo,
    IPasswordHasher<User> passwordHasher, IJwtService jwtService) : IUserService
{
    public async Task<(bool Success, string Message)> RegisterAsync(
        RegisterDto dto)
    {
        var email = dto.Email.Trim().ToLowerInvariant();

        var existingUser = await repo.GetByEmailAsync(email);

        if (existingUser is not null)
            return (false, "Email is already registered.");

        var user = new User
        {
            FullName = dto.FullName,
            Email = email,
            Role = UserRoles.Customer
        };

        user.PasswordHash = passwordHasher.HashPassword(
            user,
            dto.Password
        );

        await repo.AddAsync(user);
        await repo.SaveChangesAsync();

        return (true, "Registration successful.");
    }

    public async Task<(bool Success, string Message, string? Token)> LoginAsync(LoginDto dto)
    {
        var email = dto.Email.Trim().ToLower();

        var user = await repo.GetByEmailAsync(email);

        if (user is null)
        {
            return (false, "Invalid email or password.", null);
        }

        var result = passwordHasher.VerifyHashedPassword(
            user,
            user.PasswordHash,
            dto.Password
        );

        if (result == PasswordVerificationResult.Failed)
        {
            return (false, "Invalid email or password.", null);
        }

        var token = jwtService.GenerateToken(user);

        return (true, "Login successful.", token);
    }

    public async Task CreateRoleUserAsync(
    string fullName,
    string email,
    string password,
    string role)
    {
        var normalizedEmail = email.Trim().ToLowerInvariant();

        var existingUser = await repo.GetByEmailAsync(normalizedEmail);

        if (existingUser is not null)
            throw new InvalidOperationException("Email is already registered.");

        var user = new User
        {
            FullName = fullName,
            Email = normalizedEmail,
            Role = role
        };

        user.PasswordHash = passwordHasher.HashPassword(user, password);

        await repo.AddAsync(user);
        await repo.SaveChangesAsync();
    }


}