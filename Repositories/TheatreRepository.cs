using Microsoft.EntityFrameworkCore;
using MovieTicketBookingSystem.Data;
using MovieTicketBookingSystem.Models;
using MovieTicketBookingSystem.Repositories.Interfaces;

namespace MovieTicketBookingSystem.Repositories;

public class TheatreRepository : ITheatreRepository
{
    private readonly ApplicationDbContext _context;

    public TheatreRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<Theatre>> GetAllAsync(
        int pageNumber, int pageSize)
    {
        return await _context.Theatres
            .AsNoTracking()
            .OrderBy(t => t.TheatreId)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }

    public async Task<Theatre?> GetByIdAsync(int id)
    {
        return await _context.Theatres
            .FirstOrDefaultAsync(t => t.TheatreId == id);
    }

    public async Task AddAsync(Theatre theatre)
    {
        await _context.Theatres.AddAsync(theatre);

        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Theatre theatre)
    {
        _context.Theatres.Update(theatre);

        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Theatre theatre)
    {
        _context.Theatres.Remove(theatre);

        await _context.SaveChangesAsync();
    }
}