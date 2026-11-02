
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MovieTicketBookingSystem.DTOs.Bookings;
using MovieTicketBookingSystem.Constants;
using MovieTicketBookingSystem.Services.Interfaces;
using System.Security.Claims;

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

    private string? GetCurrentUserId()
    {
        return User.FindFirstValue(ClaimTypes.NameIdentifier);
    }

    // POST: api/bookings
    // Customer and Admin can create bookings.
    [Authorize(Roles = UserRoles.Customer + "," + UserRoles.Admin)]
    [HttpPost]
    public async Task<IActionResult> Create(
        CreateBookingRequest request)
    {
        var userId = GetCurrentUserId();

        if (string.IsNullOrEmpty(userId))
            return Unauthorized();

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
    // Admin only.
    [Authorize(Roles = UserRoles.Admin)]
    [HttpGet]
    public async Task<IActionResult> GetAll(
        [FromQuery] int? showId,
        [FromQuery] string? status)
    {
        if (showId.HasValue && showId.Value < 1)
            return BadRequest("Invalid ShowId.");

        var bookings = await _bookingService.GetAllAsync(
            showId,
            status);

        return Ok(bookings);
    }

    // GET: api/bookings/1
    // Customer can view own booking.
    // Admin can view any booking.
    [Authorize(Roles = UserRoles.Customer + "," + UserRoles.Admin)]
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        if (id < 1)
            return BadRequest("Invalid BookingId.");

        var userId = GetCurrentUserId();

        if (string.IsNullOrEmpty(userId))
            return Unauthorized();

        var booking = await _bookingService.GetByIdAsync(id);

        if (booking == null)
            return NotFound("Booking not found.");

        // Ownership verification will be added in BookingService.
        // Do not expose another customer's booking.

        return Ok(booking);
    }

    // GET: api/bookings/show/1
    // Admin only.
    [Authorize(Roles = UserRoles.Admin)]
    [HttpGet("show/{showId:int}")]
    public async Task<IActionResult> GetByShowId(int showId)
    {
        if (showId < 1)
            return BadRequest("Invalid ShowId.");

        var bookings = await _bookingService.GetByShowIdAsync(showId);

        if (bookings == null)
            return NotFound("Show not found.");

        return Ok(bookings);
    }

    // PUT: api/bookings/1/cancel
    // Customer and Admin can cancel bookings.
    [Authorize(Roles = UserRoles.Customer + "," + UserRoles.Admin)]
    [HttpPut("{id:int}/cancel")]
    public async Task<IActionResult> Cancel(int id)
    {
        if (id < 1)
            return BadRequest("Invalid BookingId.");

        var userId = GetCurrentUserId();

        if (string.IsNullOrEmpty(userId))
            return Unauthorized();

        try
        {
            var booking = await _bookingService.CancelAsync(id);

            if (booking == null)
                return NotFound("Booking not found.");

            // Ownership verification will be added in BookingService.

            return Ok(booking);
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(ex.Message);
        }
    }
}
