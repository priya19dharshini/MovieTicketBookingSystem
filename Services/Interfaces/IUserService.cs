using MovieTicketBookingSystem.DTOs.Auth;

namespace MovieTicketBookingSystem.Services.Interfaces;

public interface IUserService
{
    Task<(bool Success, string Message)> RegisterAsync(RegisterDto dto);

    Task<(bool Success, string Message, string? Token)> LoginAsync(LoginDto dto);
}