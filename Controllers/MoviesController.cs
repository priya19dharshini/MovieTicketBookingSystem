
using Microsoft.AspNetCore.Mvc;
using MovieTicketBookingSystem.DTOs.Movies;
using MovieTicketBookingSystem.Models;
using MovieTicketBookingSystem.Services;
using MovieTicketBookingSystem.Services.Interfaces;

namespace MovieTicketBookingSystem.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MoviesController : ControllerBase
{
    private readonly IMovieService _service;

    public MoviesController(IMovieService service)
    {
        _service = service;
    }

    // GET: api/movies
    [HttpGet]
    public async Task<IActionResult> GetAll(
     [FromQuery] int pageNumber = 1,
     [FromQuery] int pageSize = 10,
     [FromQuery] string? title = null)
    {
        if (pageNumber < 1 || pageSize < 1 || pageSize > 100)
        {
            return BadRequest("Invalid pagination values.");
        }

        var movies = await _service.GetAllAsync(
            pageNumber,
            pageSize,
            title);

        return Ok(movies);
    }


    // GET: api/movies/1
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var movie = await _service.GetByIdAsync(id);

        if (movie == null)
            return NotFound();

        return Ok(movie);
    }

    // GET: api/movies/search?title=leo
    [HttpGet("search")]
    public async Task<IActionResult> Search([FromQuery] string title)
    {
        if (string.IsNullOrWhiteSpace(title))
            return BadRequest("Title is required.");

        var movies = await _service.SearchAsync(title);

        return Ok(movies);
    }

    // POST: api/movies
    [HttpPost]
    public async Task<IActionResult> Create(CreateMovieRequest request)
    {
        var movie = await _service.CreateAsync(request);

        return CreatedAtAction(
            nameof(GetById),
            new { id = movie.MovieId },
            movie);
    }

    // PUT: api/movies/1
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(
        int id,
        UpdateMovieRequest request)
    {
        var movie = await _service.UpdateAsync(id, request);

        if (movie == null)
            return NotFound();

        return Ok(movie);
    }

    // DELETE: api/movies/1
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var deleted = await _service.DeleteAsync(id);

        if (!deleted)
            return NotFound();

        return NoContent();
    }
}
