
using MovieTicketBookingSystem.DTOs.Bookings;

namespace MovieTicketBookingSystem.Services.Interfaces;

public interface IBookingService
{
    Task<BookingResponse> CreateAsync(
        CreateBookingRequest request,
        int userId);

    Task<List<BookingResponse>> GetAllAsync(
        int? showId,
        string? status);

    Task<BookingResponse?> GetByIdAsync(int id);

    Task<List<BookingResponse>?> GetByShowIdAsync(int showId);

    Task<BookingResponse?> CancelAsync(int id);
}
