using MovieTicketBookingSystem.DTOs.Seats;
using MovieTicketBookingSystem.Exceptions;
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
        if (screenId <= 0)
            throw new BadRequestException("Screen ID must be greater than zero.");

        var screen = await _screenRepository.GetByIdAsync(screenId);

        if (screen == null)
            throw new NotFoundException("Screen not found.");

        var seats = await _repository.GetByScreenIdAsync(screenId);

        return seats.Select(MapToResponse).ToList();
    }

    public async Task<SeatResponse?> GetByIdAsync(int id)
    {
        if (id <= 0)
            throw new BadRequestException("Seat ID must be greater than zero.");

        var seat = await _repository.GetByIdAsync(id);

        if (seat == null)
            return null;

        return MapToResponse(seat);
    }

    public async Task<SeatResponse> CreateAsync(
        CreateSeatRequest request)
    {
        ValidateSeat(
            request.ScreenId,
            request.RowName,
            request.SeatNumber,
            request.SeatType);

        await ValidateScreenAsync(request.ScreenId);

        var exists = await _repository.ExistsAsync(
            request.ScreenId,
            request.RowName,
            request.SeatNumber);

        if (exists)
            throw new ConflictException(
                "This seat already exists in the screen.");

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
        if (request.ScreenId <= 0)
            throw new BadRequestException(
                "Screen ID must be greater than zero.");

        if (request.Seats == null || request.Seats.Count == 0)
            throw new BadRequestException(
                "At least one seat must be provided.");

        await ValidateScreenAsync(request.ScreenId);

        // Validate every seat in the request.
        foreach (var item in request.Seats)
        {
            ValidateSeat(
                request.ScreenId,
                item.RowName,
                item.SeatNumber,
                item.SeatType);
        }

        // Check duplicates within the request itself.
        var duplicateInRequest = request.Seats
            .GroupBy(s => new
            {
                RowName = s.RowName.Trim().ToUpperInvariant(),
                s.SeatNumber
            })
            .Any(g => g.Count() > 1);

        if (duplicateInRequest)
            throw new ConflictException(
                "Duplicate seat coordinates found in the request.");

        // Check whether seats already exist in the database.
        foreach (var item in request.Seats)
        {
            var exists = await _repository.ExistsAsync(
                request.ScreenId,
                item.RowName,
                item.SeatNumber);

            if (exists)
                throw new ConflictException(
                    $"Seat {item.RowName}{item.SeatNumber} already exists.");
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
        int id,
        UpdateSeatRequest request)
    {
        if (id <= 0)
            throw new BadRequestException(
                "Seat ID must be greater than zero.");

        var seat = await _repository.GetByIdAsync(id);

        if (seat == null)
            return null;

        ValidateSeat(
            request.ScreenId,
            request.RowName,
            request.SeatNumber,
            request.SeatType);

        await ValidateScreenAsync(request.ScreenId);

        var exists = await _repository.ExistsAsync(
            request.ScreenId,
            request.RowName,
            request.SeatNumber,
            id);

        if (exists)
            throw new ConflictException(
                "This seat already exists in the screen.");

        seat.ScreenId = request.ScreenId;
        seat.RowName = request.RowName;
        seat.SeatNumber = request.SeatNumber;
        seat.SeatType = request.SeatType;

        await _repository.UpdateAsync(seat);

        return MapToResponse(seat);
    }

    public async Task<bool> DeleteAsync(int id)
    {
        if (id <= 0)
            throw new BadRequestException(
                "Seat ID must be greater than zero.");

        var seat = await _repository.GetByIdAsync(id);

        if (seat == null)
            return false;

        await _repository.DeleteAsync(seat);

        return true;
    }

    private async Task ValidateScreenAsync(int screenId)
    {
        var screen = await _screenRepository.GetByIdAsync(screenId);

        if (screen == null)
            throw new NotFoundException("Screen not found.");
    }

    private static void ValidateSeat(
        int screenId,
        string rowName,
        int seatNumber,
        string seatType)
    {
        if (screenId <= 0)
            throw new BadRequestException(
                "Screen ID must be greater than zero.");

        if (string.IsNullOrWhiteSpace(rowName))
            throw new BadRequestException(
                "Row name is required.");

        if (rowName.Length > 5)
            throw new BadRequestException(
                "Row name cannot exceed 5 characters.");

        if (seatNumber <= 0)
            throw new BadRequestException(
                "Seat number must be greater than zero.");

        if (string.IsNullOrWhiteSpace(seatType))
            throw new BadRequestException(
                "Seat type is required.");

        if (seatType.Length > 30)
            throw new BadRequestException(
                "Seat type cannot exceed 30 characters.");
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