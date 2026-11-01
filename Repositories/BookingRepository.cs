using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using MovieTicketBookingSystem.Data;
using MovieTicketBookingSystem.Models;
using MovieTicketBookingSystem.Repositories.Interfaces;

namespace MovieTicketBookingSystem.Repositories;

public class BookingRepository : IBookingRepository
{
    private readonly ApplicationDbContext _context;

    public BookingRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<Booking>> GetAllAsync(
        int? showId,
        string? status)
    {
        var query = _context.Bookings
            .AsNoTracking()
            .Include(b => b.BookingSeats)
                .ThenInclude(bs => bs.Seat)
            .AsQueryable();

        if (showId.HasValue)
        {
            query = query.Where(b => b.ShowId == showId.Value);
        }

        if (!string.IsNullOrWhiteSpace(status))
        {
            query = query.Where(b => b.Status == status);
        }

        return await query
            .OrderByDescending(b => b.BookingDate)
            .ToListAsync();
    }

    public async Task<Booking?> GetByIdAsync(int id)
    {
        return await _context.Bookings
            .Include(b => b.BookingSeats)
                .ThenInclude(bs => bs.Seat)
            .FirstOrDefaultAsync(b => b.BookingId == id);
    }

    public async Task<List<Booking>> GetByShowIdAsync(int showId)
    {
        return await _context.Bookings
            .AsNoTracking()
            .Include(b => b.BookingSeats)
                .ThenInclude(bs => bs.Seat)
            .Where(b => b.ShowId == showId)
            .OrderByDescending(b => b.BookingDate)
            .ToListAsync();
    }

    public async Task<List<ShowSeat>> GetShowSeatsAsync(
        int showId,
        List<int> seatIds)
    {
        return await _context.ShowSeats
            .Include(ss => ss.Seat)
            .Where(ss =>
                ss.ShowId == showId &&
                seatIds.Contains(ss.SeatId))
            .ToListAsync();
    }

    public async Task AddWithSeatsAsync(
        Booking booking,
        List<ShowSeat> showSeats)
    {
        await using IDbContextTransaction transaction =
            await _context.Database.BeginTransactionAsync();

        try
        {
            _context.Bookings.Add(booking);

            await _context.SaveChangesAsync();

            foreach (var showSeat in showSeats)
            {
                showSeat.Status = "Booked";
            }

            var bookingSeats = showSeats.Select(showSeat =>
                new BookingSeat
                {
                    BookingId = booking.BookingId,
                    SeatId = showSeat.SeatId,
                    Price = booking.TotalAmount / showSeats.Count
                }).ToList();

            _context.BookingSeats.AddRange(bookingSeats);

            await _context.SaveChangesAsync();

            await transaction.CommitAsync();
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    public async Task CancelAsync(
        Booking booking,
        List<ShowSeat> showSeats)
    {
        await using IDbContextTransaction transaction =
            await _context.Database.BeginTransactionAsync();

        try
        {
            booking.Status = "Cancelled";

            foreach (var showSeat in showSeats)
            {
                showSeat.Status = "Available";
            }

            await _context.SaveChangesAsync();

            await transaction.CommitAsync();
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }
}