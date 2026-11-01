using MovieTicketBookingSystem.Models;

namespace MovieTicketBookingSystem.Repositories.Interfaces;

public interface IMovieRepository
{
    Task<List<Movie>> GetAllAsync(
    int pageNumber,
    int pageSize,
    string? title);

    Task<Movie?> GetByIdAsync(int id);

    Task<Movie?> GetByTitleAsync(string title);

    Task AddAsync(Movie movie);

    Task UpdateAsync(Movie movie);

    Task DeleteAsync(Movie movie);

    Task<List<Movie?>> SearchAsync(string title);



}
