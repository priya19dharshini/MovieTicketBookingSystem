using MovieTicketBookingSystem.DTOs.Bookings;
using MovieTicketBookingSystem.Exceptions;
using MovieTicketBookingSystem.Models;
using MovieTicketBookingSystem.Repositories.Interfaces;
using MovieTicketBookingSystem.Services.Interfaces;

namespace MovieTicketBookingSystem.Services;

public class BookingService : IBookingService
{
    private readonly IBookingRepository _bookingRepository;
    private readonly IShowRepository _showRepository;

    public BookingService(
        IBookingRepository bookingRepository,
        IShowRepository showRepository)
    {
        _bookingRepository = bookingRepository;
        _showRepository = showRepository;
    }

    public async Task<BookingResponse> CreateAsync(
    CreateBookingRequest request,
    int userId)
    {
        // 1. Validate customer details.
        ValidateCustomerDetails(
            request.CustomerName,
            request.CustomerEmail,
            request.CustomerPhone);

        // 2. Validate selected seats.
        if (request.ShowId <= 0)
        {
            throw new BadRequestException(
                "Show ID must be greater than zero.");
        }

        if (request.SeatIds == null || request.SeatIds.Count == 0)
        {
            throw new BadRequestException(
                "Select at least one seat.");
        }

        if (request.SeatIds.Any(id => id <= 0))
        {
            throw new BadRequestException(
                "Seat IDs must be greater than zero.");
        }

        if (request.SeatIds.Distinct().Count() != request.SeatIds.Count)
        {
            throw new BadRequestException(
                "Duplicate Seat IDs are not allowed.");
        }

        // 3. Check whether the show exists.
        var show = await _showRepository.GetByIdAsync(request.ShowId);

        if (show == null)
        {
            throw new NotFoundException("Show not found.");
        }

        // 4. Retrieve the requested ShowSeats.
        var showSeats = await _bookingRepository.GetShowSeatsAsync(
            request.ShowId,
            request.SeatIds);

        // Check whether every requested seat belongs to this show.
        if (showSeats.Count != request.SeatIds.Count)
        {
            throw new BadRequestException(
                "One or more selected seats do not belong to this show.");
        }

        // 5. Check seat availability.
        if (showSeats.Any(ss => ss.Status != "Available"))
        {
            throw new ConflictException(
                "One or more selected seats are already booked.");
        }

        // 6. Calculate total amount using server-side ticket price.
        decimal totalAmount = show.TicketPrice * showSeats.Count;

        // 7. Create Booking.
        var booking = new Booking
        {
            BookingReference =
        "MOV" + Guid.NewGuid().ToString("N")[..12].ToUpper(),

            ShowId = request.ShowId,

            UserId = userId,

            CustomerName = request.CustomerName.Trim(),

            CustomerEmail = request.CustomerEmail.Trim(),

            CustomerPhone = request.CustomerPhone.Trim(),

            BookingDate = DateTime.UtcNow,

            TotalAmount = totalAmount,

            Status = "Confirmed"
        };

        // 8. Save booking, booking seats and seat status
        // inside one database transaction.
        await _bookingRepository.AddWithSeatsAsync(
            booking,
            showSeats);

        // 9. Return booking response.
        return MapToResponse(booking, showSeats);
    }

    public async Task<List<BookingResponse>> GetAllAsync(
        int? showId,
        string? status)
    {
        if (showId.HasValue && showId.Value <= 0)
        {
            throw new BadRequestException(
                "Show ID must be greater than zero.");
        }

        var bookings = await _bookingRepository.GetAllAsync(
            showId,
            status);

        return bookings
            .Select(booking => MapToResponse(
                booking,
                booking.BookingSeats.Select(bs =>
                    new ShowSeat
                    {
                        SeatId = bs.SeatId,
                        Seat = bs.Seat
                    }).ToList()))
            .ToList();
    }

    public async Task<BookingResponse?> GetByIdAsync(int id)
    {
        if (id <= 0)
        {
            throw new BadRequestException(
                "Booking ID must be greater than zero.");
        }

        var booking = await _bookingRepository.GetByIdAsync(id);

        if (booking == null)
        {
            return null;
        }

        var showSeats = booking.BookingSeats
            .Select(bs => new ShowSeat
            {
                SeatId = bs.SeatId,
                Seat = bs.Seat
            })
            .ToList();

        return MapToResponse(booking, showSeats);
    }

    public async Task<List<BookingResponse>?> GetByShowIdAsync(
        int showId)
    {
        if (showId <= 0)
        {
            throw new BadRequestException(
                "Show ID must be greater than zero.");
        }

        var show = await _showRepository.GetByIdAsync(showId);

        if (show == null)
        {
            return null;
        }

        var bookings = await _bookingRepository.GetByShowIdAsync(showId);

        return bookings
            .Select(booking => MapToResponse(
                booking,
                booking.BookingSeats.Select(bs =>
                    new ShowSeat
                    {
                        SeatId = bs.SeatId,
                        Seat = bs.Seat
                    }).ToList()))
            .ToList();
    }

    public async Task<BookingResponse?> CancelAsync(int id)
    {
        // 1. Validate Booking ID.
        if (id <= 0)
        {
            throw new BadRequestException(
                "Booking ID must be greater than zero.");
        }

        // 2. Find booking.
        var booking = await _bookingRepository.GetByIdAsync(id);

        if (booking == null)
        {
            return null;
        }

        // 3. Only Confirmed bookings can be cancelled.
        if (booking.Status != "Confirmed")
        {
            throw new ConflictException(
                "Only confirmed bookings can be cancelled.");
        }

        // 4. Get the ShowSeats belonging to this booking.
        var seatIds = booking.BookingSeats
            .Select(bs => bs.SeatId)
            .ToList();

        var showSeats = await _bookingRepository.GetShowSeatsAsync(
            booking.ShowId,
            seatIds);

        if (showSeats.Count != seatIds.Count)
        {
            throw new ConflictException(
                "Booking seat availability records could not be found.");
        }

        // 5. Cancel booking and release seats in one transaction.
        await _bookingRepository.CancelAsync(booking, showSeats);

        // 6. Return updated booking.
        return MapToResponse(booking, showSeats);
    }

    private static void ValidateCustomerDetails(
        string customerName,
        string customerEmail,
        string customerPhone)
    {
        if (string.IsNullOrWhiteSpace(customerName))
        {
            throw new BadRequestException(
                "Customer name is required.");
        }

        if (customerName.Trim().Length > 100)
        {
            throw new BadRequestException(
                "Customer name cannot exceed 100 characters.");
        }

        if (string.IsNullOrWhiteSpace(customerEmail))
        {
            throw new BadRequestException(
                "Customer email is required.");
        }

        if (customerEmail.Trim().Length > 150)
        {
            throw new BadRequestException(
                "Customer email cannot exceed 150 characters.");
        }

        if (string.IsNullOrWhiteSpace(customerPhone))
        {
            throw new BadRequestException(
                "Customer phone is required.");
        }

        if (customerPhone.Trim().Length > 15)
        {
            throw new BadRequestException(
                "Customer phone cannot exceed 15 characters.");
        }
    }

    private static BookingResponse MapToResponse(
    Booking booking,
    List<ShowSeat> showSeats)
    {
        return new BookingResponse
        {
            BookingId = booking.BookingId,

            BookingReference = booking.BookingReference,

            ShowId = booking.ShowId,

            UserId = booking.UserId,

            CustomerName = booking.CustomerName,

            CustomerEmail = booking.CustomerEmail,

            CustomerPhone = booking.CustomerPhone,

            BookingDate = booking.BookingDate,

            TotalAmount = booking.TotalAmount,

            Status = booking.Status,

            Seats = showSeats
                .Where(ss => ss.Seat != null)
                .OrderBy(ss => ss.Seat.RowName)
                .ThenBy(ss => ss.Seat.SeatNumber)
                .Select(ss =>
                    $"{ss.Seat.RowName}{ss.Seat.SeatNumber}")
                .ToList()
        };
    }
}
