using MovieTicketBookingSystem.Models;

namespace MovieTicketBookingSystem.Repositories.Interfaces;

public interface IShowSeatRepository
{
    Task<List<ShowSeat>> GetByShowIdAsync(int showId);
}