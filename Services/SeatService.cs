using MovieTicketBookingSystem.DTOs.Seats;
using MovieTicketBookingSystem.Models;
using MovieTicketBookingSystem.Repositories.Interfaces;
using MovieTicketBookingSystem.Services.Interfaces;

namespace MovieTicketBookingSystem.Services;

public class SeatService : ISeatService
{
    private readonly ISeatRepository _repository;
    private readonly IScreenRepository _screenRepository;

    public SeatService(
        ISeatRepository repository,
        IScreenRepository screenRepository)
    {
        _repository = repository;
        _screenRepository = screenRepository;
    }

    public async Task<List<SeatResponse>?> GetByScreenIdAsync(
        int screenId)
    {
        var screen = await _screenRepository.GetByIdAsync(screenId);

        if (screen == null)
        {
            return null;
        }

        var seats = await _repository.GetByScreenIdAsync(screenId);

        return seats.Select(MapToResponse).ToList();
    }

    public async Task<SeatResponse?> GetByIdAsync(int id)
    {
        var seat = await _repository.GetByIdAsync(id);

        if (seat == null)
        {
            return null;
        }

        return MapToResponse(seat);
    }

    public async Task<SeatResponse> CreateAsync(
        CreateSeatRequest request)
    {
        var screen = await _screenRepository.GetByIdAsync(
            request.ScreenId);

        if (screen == null)
        {
            throw new KeyNotFoundException("Screen not found.");
        }

        var exists = await _repository.ExistsAsync(
            request.ScreenId,
            request.RowName,
            request.SeatNumber);

        if (exists)
        {
            throw new InvalidOperationException(
                "This seat already exists in the screen.");
        }

        var seat = new Seat
        {
            ScreenId = request.ScreenId,
            RowName = request.RowName,
            SeatNumber = request.SeatNumber,
            SeatType = request.SeatType
        };

        await _repository.AddAsync(seat);

        return MapToResponse(seat);
    }

    public async Task<List<SeatResponse>> BulkCreateAsync(
        BulkCreateSeatsRequest request)
    {
        var screen = await _screenRepository.GetByIdAsync(
            request.ScreenId);

        if (screen == null)
        {
            throw new KeyNotFoundException("Screen not found.");
        }

        // Check duplicates within the request itself.
        var duplicateInRequest = request.Seats
            .GroupBy(s => new
            {
                s.RowName,
                s.SeatNumber
            })
            .Any(g => g.Count() > 1);

        if (duplicateInRequest)
        {
            throw new InvalidOperationException(
                "Duplicate seat coordinates found in the request.");
        }

        // Check whether any seat already exists in the database.
        foreach (var item in request.Seats)
        {
            var exists = await _repository.ExistsAsync(
                request.ScreenId,
                item.RowName,
                item.SeatNumber);

            if (exists)
            {
                throw new InvalidOperationException(
                    $"Seat {item.RowName}{item.SeatNumber} already exists.");
            }
        }

        var seats = request.Seats.Select(item => new Seat
        {
            ScreenId = request.ScreenId,
            RowName = item.RowName,
            SeatNumber = item.SeatNumber,
            SeatType = item.SeatType
        }).ToList();

        await _repository.AddRangeAsync(seats);

        return seats.Select(MapToResponse).ToList();
    }

    public async Task<SeatResponse?> UpdateAsync(
        int id, UpdateSeatRequest request)
    {
        var seat = await _repository.GetByIdAsync(id);

        if (seat == null)
        {
            return null;
        }

        var screen = await _screenRepository.GetByIdAsync(
            request.ScreenId);

        if (screen == null)
        {
            throw new KeyNotFoundException("Screen not found.");
        }

        var exists = await _repository.ExistsAsync(
            request.ScreenId,
            request.RowName,
            request.SeatNumber,
            id);

        if (exists)
        {
            throw new InvalidOperationException(
                "This seat already exists in the screen.");
        }

        seat.ScreenId = request.ScreenId;
        seat.RowName = request.RowName;
        seat.SeatNumber = request.SeatNumber;
        seat.SeatType = request.SeatType;

        await _repository.UpdateAsync(seat);

        return MapToResponse(seat);
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var seat = await _repository.GetByIdAsync(id);

        if (seat == null)
        {
            return false;
        }

        await _repository.DeleteAsync(seat);

        return true;
    }

    private static SeatResponse MapToResponse(Seat seat)
    {
        return new SeatResponse
        {
            SeatId = seat.SeatId,
            ScreenId = seat.ScreenId,
            RowName = seat.RowName,
            SeatNumber = seat.SeatNumber,
            SeatType = seat.SeatType
        };
    }
}