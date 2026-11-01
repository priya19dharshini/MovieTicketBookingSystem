using Microsoft.EntityFrameworkCore;
using MovieTicketBookingSystem.Data;
using MovieTicketBookingSystem.Models;
using MovieTicketBookingSystem.Repositories.Interfaces;

namespace MovieTicketBookingSystem.Repositories;

public class ShowRepository : IShowRepository
{
    private readonly ApplicationDbContext _context;

    public ShowRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<Show>> GetAllAsync(
        DateTime? date,
        int? movieId,
        int? theatreId)
    {
        var query = _context.Shows
            .AsNoTracking()
            .AsQueryable();

        if (date.HasValue)
        {
            var start = date.Value.Date;
            var end = start.AddDays(1);

            query = query.Where(s =>
                s.StartTime >= start &&
                s.StartTime < end);
        }

        if (movieId.HasValue)
        {
            query = query.Where(s => s.MovieId == movieId.Value);
        }

        if (theatreId.HasValue)
        {
            query = query.Where(s =>
                s.Screen.TheatreId == theatreId.Value);
        }

        return await query
            .OrderBy(s => s.StartTime)
            .ToListAsync();
    }

    public async Task<Show?> GetByIdAsync(int id)
    {
        return await _context.Shows
            .FirstOrDefaultAsync(s => s.ShowId == id);
    }

    public async Task<List<Show>> GetByMovieIdAsync(int movieId)
    {
        return await _context.Shows
            .AsNoTracking()
            .Where(s => s.MovieId == movieId)
            .OrderBy(s => s.StartTime)
            .ToListAsync();
    }

    public async Task<List<Show>> GetByTheatreIdAsync(int theatreId)
    {
        return await _context.Shows
            .AsNoTracking()
            .Where(s => s.Screen.TheatreId == theatreId)
            .OrderBy(s => s.StartTime)
            .ToListAsync();
    }

    public async Task<List<Show>> SearchByDateAsync(DateTime date)
    {
        var start = date.Date;
        var end = start.AddDays(1);

        return await _context.Shows
            .AsNoTracking()
            .Where(s => s.StartTime >= start && s.StartTime < end)
            .OrderBy(s => s.StartTime)
            .ToListAsync();
    }

    public async Task AddWithSeatsAsync(Show show, List<Seat> seats)
    {
        await using var transaction =
            await _context.Database.BeginTransactionAsync();

        try
        {
            await _context.Shows.AddAsync(show);

            // Save first to generate ShowId.
            await _context.SaveChangesAsync();

            var showSeats = seats.Select(seat => new ShowSeat
            {
                ShowId = show.ShowId,
                SeatId = seat.SeatId,
                Status = "Available"
            }).ToList();

            await _context.ShowSeats.AddRangeAsync(showSeats);

            await _context.SaveChangesAsync();

            await transaction.CommitAsync();
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    public async Task UpdateAsync(Show show)
    {
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Show show)
    {
        await using var transaction =
            await _context.Database.BeginTransactionAsync();

        try
        {
            var showSeats = await _context.ShowSeats
                .Where(ss => ss.ShowId == show.ShowId)
                .ToListAsync();

            _context.ShowSeats.RemoveRange(showSeats);
            _context.Shows.Remove(show);

            await _context.SaveChangesAsync();

            await transaction.CommitAsync();
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    public async Task<bool> HasTimeOverlapAsync(
    int screenId,
    DateTime startTime,
    DateTime endTime,
    int? excludeShowId = null)
    {
        return await _context.Shows.AnyAsync(s =>
            s.ScreenId == screenId &&
            s.StartTime < endTime &&
            s.EndTime > startTime &&
            (!excludeShowId.HasValue ||
             s.ShowId != excludeShowId.Value));
    }
}