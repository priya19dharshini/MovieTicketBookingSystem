using MovieTicketBookingSystem.Models;

namespace MovieTicketBookingSystem.Repositories.Interfaces;

public interface IBookingRepository
{
    Task<List<Booking>> GetAllAsync(
        int? showId,
        string? status);

    Task<Booking?> GetByIdAsync(int id);

    Task<List<Booking>> GetByShowIdAsync(int showId);

    Task<List<ShowSeat>> GetShowSeatsAsync(
        int showId,
        List<int> seatIds);

    Task AddWithSeatsAsync(
        Booking booking,
        List<ShowSeat> showSeats);

    Task CancelAsync(
        Booking booking,
        List<ShowSeat> showSeats);
}