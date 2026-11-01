using Microsoft.AspNetCore.Mvc;
using MovieTicketBookingSystem.DTOs.Bookings;
using MovieTicketBookingSystem.Services.Interfaces;

namespace MovieTicketBookingSystem.Controllers;

[Route("api/[controller]")]
[ApiController]
public class BookingsController : ControllerBase
{
    private readonly IBookingService _bookingService;

    public BookingsController(IBookingService bookingService)
    {
        _bookingService = bookingService;
    }

    // POST: api/bookings
    [HttpPost]
    public async Task<IActionResult> Create(
        CreateBookingRequest request)
    {
        try
        {
            var booking = await _bookingService.CreateAsync(request);

            return CreatedAtAction(
                nameof(GetById),
                new { id = booking.BookingId },
                booking);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(ex.Message);
        }
    }

    // GET: api/bookings
    // GET: api/bookings?showId=1
    // GET: api/bookings?status=Confirmed
    [HttpGet]
    public async Task<IActionResult> GetAll(
        [FromQuery] int? showId,
        [FromQuery] string? status)
    {
        if (showId.HasValue && showId.Value < 1)
        {
            return BadRequest("Invalid ShowId.");
        }

        var bookings = await _bookingService.GetAllAsync(
            showId,
            status);

        return Ok(bookings);
    }

    // GET: api/bookings/1
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        if (id < 1)
        {
            return BadRequest("Invalid BookingId.");
        }

        var booking = await _bookingService.GetByIdAsync(id);

        if (booking == null)
        {
            return NotFound("Booking not found.");
        }

        return Ok(booking);
    }

    // GET: api/bookings/show/1
    [HttpGet("show/{showId:int}")]
    public async Task<IActionResult> GetByShowId(int showId)
    {
        if (showId < 1)
        {
            return BadRequest("Invalid ShowId.");
        }

        var bookings = await _bookingService.GetByShowIdAsync(showId);

        if (bookings == null)
        {
            return NotFound("Show not found.");
        }

        return Ok(bookings);
    }

    // PUT: api/bookings/1/cancel
    [HttpPut("{id:int}/cancel")]
    public async Task<IActionResult> Cancel(int id)
    {
        if (id < 1)
        {
            return BadRequest("Invalid BookingId.");
        }

        try
        {
            var booking = await _bookingService.CancelAsync(id);

            if (booking == null)
            {
                return NotFound("Booking not found.");
            }

            return Ok(booking);
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(ex.Message);
        }
    }
}