using MovieTicketBookingSystem.Models;

namespace MovieTicketBookingSystem.Repositories.Interfaces;

public interface ISeatRepository
{
    Task<List<Seat>> GetByScreenIdAsync(int screenId);

    Task<Seat?> GetByIdAsync(int id);

    Task<bool> ExistsAsync(
        int screenId,
        string rowName,
        int seatNumber,
        int? excludeSeatId = null);

    Task AddAsync(Seat seat);

    Task AddRangeAsync(List<Seat> seats);

    Task UpdateAsync(Seat seat);

    Task DeleteAsync(Seat seat);
}