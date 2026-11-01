using Microsoft.AspNetCore.Mvc;
using MovieTicketBookingSystem.DTOs.Seats;
using MovieTicketBookingSystem.Services.Interfaces;

namespace MovieTicketBookingSystem.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SeatsController : ControllerBase
{
    private readonly ISeatService _seatService;

    public SeatsController(ISeatService seatService)
    {
        _seatService = seatService;
    }

    // 1. Get seats by screen ID
    [HttpGet("screen/{screenId:int}")]
    public async Task<IActionResult> GetByScreenId(int screenId)
    {
        if (screenId < 1)
        {
            return BadRequest("Invalid ScreenId.");
        }

        var seats = await _seatService.GetByScreenIdAsync(screenId);

        if (seats == null)
        {
            return NotFound("Screen not found.");
        }

        return Ok(seats);
    }

    // 2. Get seat by ID
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var seat = await _seatService.GetByIdAsync(id);

        if (seat == null)
        {
            return NotFound();
        }

        return Ok(seat);
    }

    // 3. Create one seat
    [HttpPost]
    public async Task<IActionResult> Create(CreateSeatRequest request)
    {
        try
        {
            var seat = await _seatService.CreateAsync(request);

            return CreatedAtAction(
                nameof(GetById),
                new { id = seat.SeatId },
                seat);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(ex.Message);
        }
    }

    // 4. Bulk create seats
    [HttpPost("bulk")]
    public async Task<IActionResult> BulkCreate(
        BulkCreateSeatsRequest request)
    {
        try
        {
            var seats = await _seatService.BulkCreateAsync(request);

            return StatusCode(
                StatusCodes.Status201Created,
                seats);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(ex.Message);
        }
    }

    // 5. Update seat
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(
        int id, UpdateSeatRequest request)
    {
        try
        {
            var seat = await _seatService.UpdateAsync(id, request);

            if (seat == null)
            {
                return NotFound();
            }

            return Ok(seat);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(ex.Message);
        }
    }

    // 6. Delete seat
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var result = await _seatService.DeleteAsync(id);

        if (!result)
        {
            return NotFound();
        }

        return NoContent();
    }
}