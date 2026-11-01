using MovieTicketBookingSystem.DTOs.Seats;

namespace MovieTicketBookingSystem.Services.Interfaces;

public interface ISeatService
{
    Task<List<SeatResponse>?> GetByScreenIdAsync(int screenId);

    Task<SeatResponse?> GetByIdAsync(int id);

    Task<SeatResponse> CreateAsync(CreateSeatRequest request);

    Task<List<SeatResponse>> BulkCreateAsync(
        BulkCreateSeatsRequest request);

    Task<SeatResponse?> UpdateAsync(
        int id, UpdateSeatRequest request);

    Task<bool> DeleteAsync(int id);
}