
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
    private readonly IAuthorizationService _authorizationService;

    public BookingsController(
        IBookingService bookingService,
        IAuthorizationService authorizationService)
    {
        _bookingService = bookingService;
        _authorizationService = authorizationService;
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
            var booking = await _bookingService.CreateAsync(
                request,
                userId);

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

        var authorizationResult =
            await _authorizationService.AuthorizeAsync(
                User,
                booking,
                AuthorizationPolicies.BookingOwner);

        if (!authorizationResult.Succeeded)
            return Forbid();

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
    // Customer can cancel own booking.
    // Admin can cancel any booking.
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
            // Retrieve booking before cancellation.
            var booking = await _bookingService.GetByIdAsync(id);

            if (booking == null)
                return NotFound("Booking not found.");

            // Perform resource-based authorization.
            var authorizationResult =
                await _authorizationService.AuthorizeAsync(
                    User,
                    booking,
                    AuthorizationPolicies.BookingOwner);

            if (!authorizationResult.Succeeded)
                return Forbid();

            // Cancel only after successful authorization.
            var cancelledBooking =
                await _bookingService.CancelAsync(id);

            if (cancelledBooking == null)
                return NotFound("Booking not found.");

            return Ok(cancelledBooking);
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(ex.Message);
        }
    }
}
