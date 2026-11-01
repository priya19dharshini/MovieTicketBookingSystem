using MovieTicketBookingSystem.DTOs.Screens;
using MovieTicketBookingSystem.Exceptions;
using MovieTicketBookingSystem.Models;
using MovieTicketBookingSystem.Repositories.Interfaces;
using MovieTicketBookingSystem.Services.Interfaces;

namespace MovieTicketBookingSystem.Services;

public class ScreenService : IScreenService
{
    private readonly IScreenRepository _repository;
    private readonly ITheatreRepository _theatreRepository;

    public ScreenService(
        IScreenRepository repository,
        ITheatreRepository theatreRepository)
    {
        _repository = repository;
        _theatreRepository = theatreRepository;
    }

    public async Task<List<ScreenResponse>> GetAllAsync(int? theatreId)
    {
        var screens = await _repository.GetAllAsync(theatreId);

        return screens.Select(MapToResponse).ToList();
    }

    public async Task<ScreenResponse?> GetByIdAsync(int id)
    {
        var screen = await _repository.GetByIdAsync(id);

        if (screen == null)
            return null;

        return MapToResponse(screen);
    }

    public async Task<List<ScreenResponse>?> GetByTheatreIdAsync(
        int theatreId)
    {
        if (theatreId <= 0)
            throw new BadRequestException(
                "Theatre ID must be greater than zero.");

        var theatre = await _theatreRepository.GetByIdAsync(theatreId);

        if (theatre == null)
            throw new NotFoundException("Theatre not found.");

        var screens = await _repository.GetByTheatreIdAsync(theatreId);

        return screens.Select(MapToResponse).ToList();
    }

    public async Task<ScreenResponse> CreateAsync(
        CreateScreenRequest request)
    {
        ValidateScreen(request.TheatreId, request.ScreenName);

        var theatre = await _theatreRepository.GetByIdAsync(
            request.TheatreId);

        if (theatre == null)
            throw new NotFoundException("Theatre not found.");

        var exists = await _repository.ExistsByNameAsync(
            request.TheatreId,
            request.ScreenName);

        if (exists)
            throw new ConflictException(
                "This screen name already exists in the theatre.");

        var screen = new Screen
        {
            TheatreId = request.TheatreId,
            ScreenName = request.ScreenName
        };

        await _repository.AddAsync(screen);

        return MapToResponse(screen);
    }

    public async Task<ScreenResponse?> UpdateAsync(
        int id, UpdateScreenRequest request)
    {
        if (id <= 0)
            throw new BadRequestException(
                "Screen ID must be greater than zero.");

        var screen = await _repository.GetByIdAsync(id);

        if (screen == null)
            return null;

        ValidateScreen(request.TheatreId, request.ScreenName);

        var theatre = await _theatreRepository.GetByIdAsync(
            request.TheatreId);

        if (theatre == null)
            throw new NotFoundException("Theatre not found.");

        var exists = await _repository.ExistsByNameAsync(
            request.TheatreId,
            request.ScreenName,
            id);

        if (exists)
            throw new ConflictException(
                "This screen name already exists in the theatre.");

        screen.TheatreId = request.TheatreId;
        screen.ScreenName = request.ScreenName;

        await _repository.UpdateAsync(screen);

        return MapToResponse(screen);
    }

    public async Task<bool> DeleteAsync(int id)
    {
        if (id <= 0)
            throw new BadRequestException(
                "Screen ID must be greater than zero.");

        var screen = await _repository.GetByIdAsync(id);

        if (screen == null)
            return false;

        await _repository.DeleteAsync(screen);

        return true;
    }

    private static void ValidateScreen(
        int theatreId, string screenName)
    {
        if (theatreId <= 0)
            throw new BadRequestException(
                "Theatre ID must be greater than zero.");

        if (string.IsNullOrWhiteSpace(screenName))
            throw new BadRequestException(
                "Screen name is required.");

        if (screenName.Length > 50)
            throw new BadRequestException(
                "Screen name cannot exceed 50 characters.");
    }

    private static ScreenResponse MapToResponse(Screen screen)
    {
        return new ScreenResponse
        {
            ScreenId = screen.ScreenId,
            TheatreId = screen.TheatreId,
            ScreenName = screen.ScreenName
        };
    }
}