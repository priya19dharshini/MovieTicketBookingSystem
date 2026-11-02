using Microsoft.AspNetCore.Mvc;
using MovieTicketBookingSystem.DTOs.Auth;
using MovieTicketBookingSystem.Services;
using MovieTicketBookingSystem.Services.Interfaces;

namespace MovieTicketBookingSystem.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController(IUserService _userService) : ControllerBase
{
    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterDto dto)
    {
        var result = await _userService.RegisterAsync(dto);

        if (!result.Success)
            return BadRequest(new
            {
                message = result.Message
            });

        return Ok(new
        {
            message = result.Message
        });
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginDto dto)
    {
        var result = await _userService.LoginAsync(dto);

        if (!result.Success)
        {
            return Unauthorized(new
            {
                message = result.Message
            });
        }

        return Ok(new
        {
            message = result.Message,
            token = result.Token
        });
    }
}