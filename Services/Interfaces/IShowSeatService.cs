using MovieTicketBookingSystem.DTOs.ShowSeats;

namespace MovieTicketBookingSystem.Services.Interfaces;

public interface IShowSeatService
{
    Task<List<ShowSeatResponse>?> GetByShowIdAsync(int showId);
}