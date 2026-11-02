using MovieTicketBookingSystem.DTOs.Shows;
using MovieTicketBookingSystem.Exceptions;
using MovieTicketBookingSystem.Models;
using MovieTicketBookingSystem.Repositories.Interfaces;
using MovieTicketBookingSystem.Services.Interfaces;

namespace MovieTicketBookingSystem.Services;

public class ShowService : IShowService
{
    private readonly IShowRepository _repository;
    private readonly ITheatreRepository _theatreRepository;
    private readonly IMovieRepository _movieRepository;
    private readonly IScreenRepository _screenRepository;
    private readonly ISeatRepository _seatRepository;

    public ShowService(
        IShowRepository repository,
        ITheatreRepository theatreRepository,
        IMovieRepository movieRepository,
        IScreenRepository screenRepository,
        ISeatRepository seatRepository)
    {
        _repository = repository;
        _theatreRepository = theatreRepository;
        _movieRepository = movieRepository;
        _screenRepository = screenRepository;
        _seatRepository = seatRepository;
    }

    public async Task<List<ShowResponse>> GetAllAsync(
        DateTime? date,
        int? movieId,
        int? theatreId)
    {
        if (movieId.HasValue && movieId.Value <= 0)
            throw new BadRequestException("Movie ID must be greater than zero.");

        if (theatreId.HasValue && theatreId.Value <= 0)
            throw new BadRequestException("Theatre ID must be greater than zero.");

        var shows = await _repository.GetAllAsync(
            date, movieId, theatreId);

        return shows.Select(MapToResponse).ToList();
    }

    public async Task<ShowResponse?> GetByIdAsync(int id)
    {
        if (id <= 0)
            throw new BadRequestException("Show ID must be greater than zero.");

        var show = await _repository.GetByIdAsync(id);

        return show == null ? null : MapToResponse(show);
    }

    public async Task<List<ShowResponse>> GetByMovieIdAsync(int movieId)
    {
        if (movieId <= 0)
            throw new BadRequestException("Movie ID must be greater than zero.");

        var movie = await _movieRepository.GetByIdAsync(movieId);

        if (movie == null)
            throw new NotFoundException("Movie not found.");

        var shows = await _repository.GetByMovieIdAsync(movieId);

        return shows.Select(MapToResponse).ToList();
    }

    public async Task<List<ShowResponse>?> GetByTheatreIdAsync(
        int theatreId)
    {
        if (theatreId <= 0)
            throw new BadRequestException("Theatre ID must be greater than zero.");

        var theatre = await _theatreRepository.GetByIdAsync(theatreId);

        if (theatre == null)
            return null;

        var shows = await _repository.GetByTheatreIdAsync(theatreId);

        return shows.Select(MapToResponse).ToList();
    }

    public async Task<List<ShowResponse>> SearchByDateAsync(DateTime date)
    {
        if (date == default)
            throw new BadRequestException("A valid date is required.");

        var shows = await _repository.SearchByDateAsync(date);

        return shows.Select(MapToResponse).ToList();
    }

    public async Task<ShowResponse> CreateAsync(CreateShowRequest request)
    {
        ValidateSchedule(
            request.MovieId,
            request.ScreenId,
            request.StartTime,
            request.EndTime,
            request.TicketPrice);

        var movie = await _movieRepository.GetByIdAsync(request.MovieId);

        if (movie == null)
            throw new NotFoundException("Movie not found.");

        var screen = await _screenRepository.GetByIdAsync(request.ScreenId);

        if (screen == null)
            throw new NotFoundException("Screen not found.");

        var hasOverlap = await _repository.HasTimeOverlapAsync(
    request.ScreenId,
    request.StartTime,
    request.EndTime);

        if (hasOverlap)
            throw new ConflictException(
                "Another show is already scheduled on this screen during the requested time.");

        var seats = await _seatRepository.GetByScreenIdAsync(
            request.ScreenId);

        var show = new Show
        {
            MovieId = request.MovieId,
            ScreenId = request.ScreenId,
            StartTime = request.StartTime,
            EndTime = request.EndTime,
            TicketPrice = request.TicketPrice
        };

        // Repository creates Show and its Available ShowSeats
        // within one database transaction.
        await _repository.AddWithSeatsAsync(show, seats);

        return MapToResponse(show);
    }

    public async Task<ShowResponse?> UpdateAsync(
        int id,
        UpdateShowRequest request)
    {
        if (id <= 0)
            throw new BadRequestException("Show ID must be greater than zero.");

        var show = await _repository.GetByIdAsync(id);

        if (show == null)
            return null;

        ValidateSchedule(
            request.MovieId,
            request.ScreenId,
            request.StartTime,
            request.EndTime,
            request.TicketPrice);

        var movie = await _movieRepository.GetByIdAsync(request.MovieId);

        if (movie == null)
            throw new NotFoundException("Movie not found.");

        var screen = await _screenRepository.GetByIdAsync(request.ScreenId);

        if (screen == null)
            throw new NotFoundException("Screen not found.");

        var hasOverlap = await _repository.HasTimeOverlapAsync(
    request.ScreenId,
    request.StartTime,
    request.EndTime,
    id);

        if (hasOverlap)
            throw new ConflictException(
                "Another show is already scheduled on this screen during the requested time.");

        // Screen changes would require rebuilding ShowSeats.
        // Keep the screen fixed for this implementation.
        if (show.ScreenId != request.ScreenId)
            throw new ConflictException(
                "Changing the screen of an existing show is not supported.");

        show.MovieId = request.MovieId;
        show.StartTime = request.StartTime;
        show.EndTime = request.EndTime;
        show.TicketPrice = request.TicketPrice;

        await _repository.UpdateAsync(show);

        return MapToResponse(show);
    }

    public async Task<bool> DeleteAsync(int id)
    {
        if (id <= 0)
            throw new BadRequestException("Show ID must be greater than zero.");

        var show = await _repository.GetByIdAsync(id);

        if (show == null)
            return false;

        await _repository.DeleteAsync(show);

        return true;
    }

    private static void ValidateSchedule(
        int movieId,
        int screenId,
        DateTime startTime,
        DateTime endTime,
        decimal ticketPrice)
    {
        if (movieId <= 0)
            throw new BadRequestException(
                "Movie ID must be greater than zero.");

        if (screenId <= 0)
            throw new BadRequestException(
                "Screen ID must be greater than zero.");

        if (startTime == default)
            throw new BadRequestException(
                "A valid start time is required.");

        if (endTime == default)
            throw new BadRequestException(
                "A valid end time is required.");

        if (endTime <= startTime)
            throw new BadRequestException(
                "EndTime must be later than StartTime.");

        if (ticketPrice < 0)
            throw new BadRequestException(
                "TicketPrice cannot be negative.");

        if (ticketPrice > 99999999.99m)
            throw new BadRequestException(
                "TicketPrice cannot exceed 99999999.99.");
    }

    private static ShowResponse MapToResponse(Show show)
    {
        return new ShowResponse
        {
            ShowId = show.ShowId,
            MovieId = show.MovieId,
            ScreenId = show.ScreenId,
            StartTime = show.StartTime,
            EndTime = show.EndTime,
            TicketPrice = show.TicketPrice
        };
    }
}