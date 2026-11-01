
using Microsoft.EntityFrameworkCore;
using MovieTicketBookingSystem.Data;
using MovieTicketBookingSystem.Repositories.Interfaces;
using MovieTicketBookingSystem.Models;

namespace MovieTicketBookingSystem.Repositories;

public class MovieRepository : IMovieRepository
{
    private readonly ApplicationDbContext _context;

    public MovieRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<Movie>> GetAllAsync(
        int pageNumber, int pageSize, string? title)
    {
        var query = _context.Movies.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(title))
        {
            query = query.Where(m => m.Title.Contains(title));
        }

        return await query
            .OrderBy(m => m.MovieId)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }

    public async Task<Movie?> GetByIdAsync(int id)
    {
        return await _context.Movies
            .FirstOrDefaultAsync(m => m.MovieId == id);
    }

    public async Task<Movie?> GetByTitleAsync(string title)
    {
        return await _context.Movies
            .AsNoTracking()
            .FirstOrDefaultAsync(m => m.Title == title);
    }

    public async Task AddAsync(Movie movie)
    {
        await _context.Movies.AddAsync(movie);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Movie movie)
    {
        _context.Movies.Update(movie);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Movie movie)
    {
        _context.Movies.Remove(movie);
        await _context.SaveChangesAsync();
    }

    public async Task<List<Movie>> SearchAsync(string title)
    {
        return await _context.Movies
            .AsNoTracking()
            .Where(m => m.Title.Contains(title))
            .ToListAsync();
    }
}
