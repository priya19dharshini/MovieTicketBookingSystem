using MovieTicketBookingSystem.DTOs.Theatres;
using MovieTicketBookingSystem.Exceptions;
using MovieTicketBookingSystem.Models;
using MovieTicketBookingSystem.Repositories.Interfaces;
using MovieTicketBookingSystem.Services.Interfaces;

namespace MovieTicketBookingSystem.Services;

public class TheatreService : ITheatreService
{
    private readonly ITheatreRepository _repository;

    public TheatreService(ITheatreRepository repository)
    {
        _repository = repository;
    }

    public async Task<List<TheatreResponse>> GetAllAsync(
        int pageNumber, int pageSize)
    {
        var theatres = await _repository.GetAllAsync(
            pageNumber, pageSize);

        return theatres.Select(MapToResponse).ToList();
    }

    public async Task<TheatreResponse?> GetByIdAsync(int id)
    {
        var theatre = await _repository.GetByIdAsync(id);

        if (theatre == null)
            return null;

        return MapToResponse(theatre);
    }

    public async Task<TheatreResponse> CreateAsync(
        CreateTheatreRequest request)
    {
        ValidateTheatre(request.Name, request.Location);

        var theatre = new Theatre
        {
            Name = request.Name,
            Location = request.Location
        };

        await _repository.AddAsync(theatre);

        return MapToResponse(theatre);
    }

    public async Task<TheatreResponse?> UpdateAsync(
        int id, UpdateTheatreRequest request)
    {
        var theatre = await _repository.GetByIdAsync(id);

        if (theatre == null)
            return null;

        ValidateTheatre(request.Name, request.Location);

        theatre.Name = request.Name;
        theatre.Location = request.Location;

        await _repository.UpdateAsync(theatre);

        return MapToResponse(theatre);
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var theatre = await _repository.GetByIdAsync(id);

        if (theatre == null)
            return false;

        await _repository.DeleteAsync(theatre);

        return true;
    }

    private static void ValidateTheatre(
        string name, string location)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new BadRequestException(
                "Theatre name is required.");

        if (name.Length > 150)
            throw new BadRequestException(
                "Theatre name cannot exceed 150 characters.");

        if (string.IsNullOrWhiteSpace(location))
            throw new BadRequestException(
                "Theatre location is required.");

        if (location.Length > 200)
            throw new BadRequestException(
                "Theatre location cannot exceed 200 characters.");
    }

    private static TheatreResponse MapToResponse(Theatre theatre)
    {
        return new TheatreResponse
        {
            TheatreId = theatre.TheatreId,
            Name = theatre.Name,
            Location = theatre.Location
        };
    }
}