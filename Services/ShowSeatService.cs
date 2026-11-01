using MovieTicketBookingSystem.DTOs.ShowSeats;
using MovieTicketBookingSystem.Repositories.Interfaces;
using MovieTicketBookingSystem.Services.Interfaces;

namespace MovieTicketBookingSystem.Services;

public class ShowSeatService : IShowSeatService
{
    private readonly IShowRepository _showRepository;
    private readonly IShowSeatRepository _showSeatRepository;

    public ShowSeatService(
        IShowRepository showRepository,
        IShowSeatRepository showSeatRepository)
    {
        _showRepository = showRepository;
        _showSeatRepository = showSeatRepository;
    }

    public async Task<List<ShowSeatResponse>?> GetByShowIdAsync(
        int showId)
    {
        // First check whether the show exists.
        var show = await _showRepository.GetByIdAsync(showId);

        if (show == null)
        {
            return null;
        }

        // Retrieve availability records for this show.
        var showSeats = await _showSeatRepository.GetByShowIdAsync(
            showId);

        // Convert entities into response DTOs.
        return showSeats.Select(ss => new ShowSeatResponse
        {
            ShowSeatId = ss.ShowSeatId,
            ShowId = ss.ShowId,
            SeatId = ss.SeatId,
            RowName = ss.Seat.RowName,
            SeatNumber = ss.Seat.SeatNumber,
            SeatType = ss.Seat.SeatType,
            Status = ss.Status
        }).ToList();
    }
}