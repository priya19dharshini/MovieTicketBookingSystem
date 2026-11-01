using MovieTicketBookingSystem.Models;

namespace MovieTicketBookingSystem.Repositories.Interfaces;

public interface IShowRepository
{
    Task<List<Show>> GetAllAsync(
        DateTime? date,
        int? movieId,
        int? theatreId);

    Task<Show?> GetByIdAsync(int id);

    Task<List<Show>> GetByMovieIdAsync(int movieId);

    Task<List<Show>> GetByTheatreIdAsync(int theatreId);

    Task<List<Show>> SearchByDateAsync(DateTime date);

    Task AddWithSeatsAsync(Show show, List<Seat> seats);

    Task UpdateAsync(Show show);

    Task DeleteAsync(Show show);
}