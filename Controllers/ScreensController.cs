using Microsoft.AspNetCore.Mvc;
using MovieTicketBookingSystem.DTOs.Screens;
using MovieTicketBookingSystem.Services.Interfaces;

namespace MovieTicketBookingSystem.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ScreensController : ControllerBase
{
    private readonly IScreenService _screenService;

    public ScreensController(IScreenService screenService)
    {
        _screenService = screenService;
    }

    // 1. Get all screens (optional theatreId filter)
    [HttpGet]
    public async Task<IActionResult> GetAll(
        [FromQuery] int? theatreId = null)
    {
        if (theatreId.HasValue && theatreId.Value < 1)
        {
            return BadRequest("Invalid TheatreId.");
        }

        var screens = await _screenService.GetAllAsync(theatreId);

        return Ok(screens);
    }

    // 2. Get screen by ID
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var screen = await _screenService.GetByIdAsync(id);

        if (screen == null)
        {
            return NotFound();
        }

        return Ok(screen);
    }

    // 3. Get screens belonging to a theatre
    [HttpGet("theatre/{theatreId:int}")]
    public async Task<IActionResult> GetByTheatreId(int theatreId)
    {
        if (theatreId < 1)
        {
            return BadRequest("Invalid TheatreId.");
        }

        var screens = await _screenService.GetByTheatreIdAsync(theatreId);

        if (screens == null)
        {
            return NotFound("Theatre not found.");
        }

        return Ok(screens);
    }

    // 4. Create screen
    [HttpPost]
    public async Task<IActionResult> Create(CreateScreenRequest request)
    {
        var screen = await _screenService.CreateAsync(request);

        return CreatedAtAction(
            nameof(GetById),
            new { id = screen.ScreenId },
            screen);
    }

    // 5. Update screen
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(
        int id, UpdateScreenRequest request)
    {
        var screen = await _screenService.UpdateAsync(id, request);

        if (screen == null)
        {
            return NotFound();
        }

        return Ok(screen);
    }

    // 6. Delete screen
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var result = await _screenService.DeleteAsync(id);

        if (!result)
        {
            return NotFound();
        }

        return NoContent();
    }
}