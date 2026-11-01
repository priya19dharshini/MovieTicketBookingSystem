
using MovieTicketBookingSystem.DTOs.Movies;
using MovieTicketBookingSystem.Models;
using MovieTicketBookingSystem.Repositories.Interfaces;
using MovieTicketBookingSystem.Services.Interfaces;


namespace MovieTicketBookingSystem.Services;

public class MovieService : IMovieService
{
    private readonly IMovieRepository _repository;

    public MovieService(IMovieRepository repository)
    {
        _repository = repository;
    }

    public async Task<List<MovieResponse>> GetAllAsync(
        int pageNumber, int pageSize, string? title)
    {
        var movies = await _repository.GetAllAsync(
            pageNumber, pageSize, title);

        return movies.Select(MapToResponse).ToList();
    }

    public async Task<MovieResponse?> GetByIdAsync(int id)
    {
        var movie = await _repository.GetByIdAsync(id);

        if (movie == null)
            return null;

        return MapToResponse(movie);
    }

    public async Task<List<MovieResponse>> SearchAsync(string title)
    {
        var movies = await _repository.SearchAsync(title);

        return movies.Select(MapToResponse).ToList();
    }

    public async Task<MovieResponse> CreateAsync(CreateMovieRequest request)
    {
        if (request.DurationMinutes <= 0)
            throw new ArgumentException("Duration must be greater than zero.");

        var movie = new Movie
        {
            Title = request.Title,
            Description = request.Description,
            DurationMinutes = request.DurationMinutes,
            ReleaseDate = request.ReleaseDate,
            Language = request.Language,
            Genre = request.Genre
        };

        await _repository.AddAsync(movie);

        return MapToResponse(movie);
    }

    public async Task<MovieResponse?> UpdateAsync(
        int id,
        UpdateMovieRequest request)
    {
        var movie = await _repository.GetByIdAsync(id);

        if (movie == null)
            return null;

        if (request.DurationMinutes <= 0)
            throw new ArgumentException("Duration must be greater than zero.");

        movie.Title = request.Title;
        movie.Description = request.Description;
        movie.DurationMinutes = request.DurationMinutes;
        movie.ReleaseDate = request.ReleaseDate;
        movie.Language = request.Language;
        movie.Genre = request.Genre;

        await _repository.UpdateAsync(movie);

        return MapToResponse(movie);
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var movie = await _repository.GetByIdAsync(id);

        if (movie == null)
            return false;

        await _repository.DeleteAsync(movie);

        return true;
    }

    private static MovieResponse MapToResponse(Movie movie)
    {
        return new MovieResponse
        {
            MovieId = movie.MovieId,
            Title = movie.Title,
            Description = movie.Description,
            DurationMinutes = movie.DurationMinutes,
            ReleaseDate = movie.ReleaseDate,
            Language = movie.Language,
            Genre = movie.Genre
        };
    }
}
