using Microsoft.AspNetCore.Mvc;
using MovieTicketBookingSystem.DTOs.Theatres;
using MovieTicketBookingSystem.Services.Interfaces;

namespace MovieTicketBookingSystem.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TheatresController : ControllerBase
{
    private readonly ITheatreService _theatreService;

    public TheatresController(ITheatreService theatreService)
    {
        _theatreService = theatreService;
    }

    // 1. Get all theatres
    [HttpGet]
    public async Task<IActionResult> GetAll(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10)
    {
        if (pageNumber < 1 || pageSize < 1 || pageSize > 100)
        {
            return BadRequest("Invalid pagination values.");
        }

        var theatres = await _theatreService.GetAllAsync(
            pageNumber, pageSize);

        return Ok(theatres);
    }

    // 2. Get theatre by ID
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var theatre = await _theatreService.GetByIdAsync(id);

        if (theatre == null)
        {
            return NotFound();
        }

        return Ok(theatre);
    }

    // 3. Create theatre
    [HttpPost]
    public async Task<IActionResult> Create(
        CreateTheatreRequest request)
    {
        var theatre = await _theatreService.CreateAsync(request);

        return CreatedAtAction(
            nameof(GetById),
            new { id = theatre.TheatreId },
            theatre);
    }

    // 4. Update theatre
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(
        int id, UpdateTheatreRequest request)
    {
        var theatre = await _theatreService.UpdateAsync(id, request);

        if (theatre == null)
        {
            return NotFound();
        }

        return Ok(theatre);
    }

    // 5. Delete theatre
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var result = await _theatreService.DeleteAsync(id);

        if (!result)
        {
            return NotFound();
        }

        return NoContent();
    }
}