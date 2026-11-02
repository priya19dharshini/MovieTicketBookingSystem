using MovieTicketBookingSystem.Models;

namespace MovieTicketBookingSystem.Services.Interfaces;

public interface IJwtService
{
    string GenerateToken(User user);
}