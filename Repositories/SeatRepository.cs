using Microsoft.EntityFrameworkCore;
using MovieTicketBookingSystem.Data;
using MovieTicketBookingSystem.Models;
using MovieTicketBookingSystem.Repositories.Interfaces;

namespace MovieTicketBookingSystem.Repositories;

public class SeatRepository : ISeatRepository
{
    private readonly ApplicationDbContext _context;

    public SeatRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<Seat>> GetByScreenIdAsync(int screenId)
    {
        return await _context.Seats
            .AsNoTracking()
            .Where(s => s.ScreenId == screenId)
            .OrderBy(s => s.RowName)
            .ThenBy(s => s.SeatNumber)
            .ToListAsync();
    }

    public async Task<Seat?> GetByIdAsync(int id)
    {
        return await _context.Seats
            .FirstOrDefaultAsync(s => s.SeatId == id);
    }

    public async Task<bool> ExistsAsync(
        int screenId,
        string rowName,
        int seatNumber,
        int? excludeSeatId = null)
    {
        var query = _context.Seats.Where(s =>
            s.ScreenId == screenId &&
            s.RowName == rowName &&
            s.SeatNumber == seatNumber);

        if (excludeSeatId.HasValue)
        {
            query = query.Where(
                s => s.SeatId != excludeSeatId.Value);
        }

        return await query.AnyAsync();
    }

    public async Task AddAsync(Seat seat)
    {
        await _context.Seats.AddAsync(seat);

        await _context.SaveChangesAsync();
    }

    public async Task AddRangeAsync(List<Seat> seats)
    {
        await _context.Seats.AddRangeAsync(seats);

        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Seat seat)
    {
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Seat seat)
    {
        _context.Seats.Remove(seat);

        await _context.SaveChangesAsync();
    }
}