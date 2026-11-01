
using MovieTicketBookingSystem.DTOs.Movies;
using MovieTicketBookingSystem.Models;

namespace MovieTicketBookingSystem.Services.Interfaces;

public interface IMovieService
{
    Task<List<MovieResponse>> GetAllAsync(
    int pageNumber,
    int pageSize,
    string? title);

    Task<MovieResponse?> GetByIdAsync(int id);

    Task<List<MovieResponse>> SearchAsync(string title);

    Task<MovieResponse> CreateAsync(CreateMovieRequest request);

    Task<MovieResponse?> UpdateAsync(int id, UpdateMovieRequest request);


    Task<bool> DeleteAsync(int id);
}
