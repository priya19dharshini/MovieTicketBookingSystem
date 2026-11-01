using Microsoft.AspNetCore.Mvc;
using MovieTicketBookingSystem.DTOs.Shows;
using MovieTicketBookingSystem.Services.Interfaces;

namespace MovieTicketBookingSystem.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ShowsController : ControllerBase
{
    private readonly IShowService _showService;

    private readonly IShowSeatService _showSeatService;

    public ShowsController(
     IShowService showService,
     IShowSeatService showSeatService)
    {
        _showService = showService;
        _showSeatService = showSeatService;
    }

    // 1. Get all shows with optional filters
    [HttpGet]
    public async Task<IActionResult> GetAll(
        [FromQuery] DateTime? date,
        [FromQuery] int? movieId,
        [FromQuery] int? theatreId)
    {
        if (movieId.HasValue && movieId.Value < 1 ||
            theatreId.HasValue && theatreId.Value < 1)
        {
            return BadRequest("Invalid filter ID.");
        }

        var shows = await _showService.GetAllAsync(
            date, movieId, theatreId);

        return Ok(shows);
    }

    // 2. Get show by ID
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var show = await _showService.GetByIdAsync(id);

        if (show == null)
        {
            return NotFound();
        }

        return Ok(show);
    }

    // 3. Get shows for a movie
    [HttpGet("movie/{movieId:int}")]
    public async Task<IActionResult> GetByMovieId(int movieId)
    {
        try
        {
            var shows = await _showService.GetByMovieIdAsync(movieId);

            return Ok(shows);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
    }

    // 4. Get shows in a theatre
    [HttpGet("theatre/{theatreId:int}")]
    public async Task<IActionResult> GetByTheatreId(int theatreId)
    {
        var shows = await _showService.GetByTheatreIdAsync(theatreId);

        if (shows == null)
        {
            return NotFound("Theatre not found.");
        }

        return Ok(shows);
    }

    // Get all seats and their availability for a show
    [HttpGet("{showId:int}/seats")]
    public async Task<IActionResult> GetSeatsByShowId(int showId)
    {
        if (showId < 1)
        {
            return BadRequest("Invalid ShowId.");
        }

        var seats = await _showSeatService.GetByShowIdAsync(showId);

        if (seats == null)
        {
            return NotFound("Show not found.");
        }

        return Ok(seats);
    }

    // 5. Search shows by date
    [HttpGet("search")]
    public async Task<IActionResult> SearchByDate(
        [FromQuery] DateTime date)
    {
        if (date == default)
        {
            return BadRequest("A valid date is required.");
        }

        var shows = await _showService.SearchByDateAsync(date);

        return Ok(shows);
    }

    // 6. Create show
    [HttpPost]
    public async Task<IActionResult> Create(CreateShowRequest request)
    {
        try
        {
            var show = await _showService.CreateAsync(request);

            return CreatedAtAction(
                nameof(GetById),
                new { id = show.ShowId },
                show);
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

    // 7. Update show
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(
        int id, UpdateShowRequest request)
    {
        try
        {
            var show = await _showService.UpdateAsync(id, request);

            if (show == null)
            {
                return NotFound();
            }

            return Ok(show);
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

    // 8. Delete show
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var result = await _showService.DeleteAsync(id);

        if (!result)
        {
            return NotFound();
        }

        return NoContent();
    }
}