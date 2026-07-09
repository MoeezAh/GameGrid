using System.Security.Claims;
using System.Threading.Tasks;
using GameCollection.Application.Common.Interfaces;
using GameCollection.Application.DTOs.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GameCollection.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IIdentityService _identityService;

    public AuthController(IIdentityService identityService)
    {
        _identityService = identityService;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request)
    {
        var (succeeded, response, errors) = await _identityService.RegisterAsync(request);
        if (!succeeded)
        {
            return BadRequest(new { errors });
        }
        return Ok(response);
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        var (succeeded, response, errors) = await _identityService.LoginAsync(request);
        if (!succeeded)
        {
            return BadRequest(new { errors });
        }
        return Ok(response);
    }

    [Authorize]
    [HttpGet("profile")]
    public async Task<IActionResult> GetProfile()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId))
        {
            return Unauthorized();
        }

        var profile = await _identityService.GetUserProfileAsync(userId);
        if (profile == null)
        {
            return NotFound("User profile not found.");
        }

        return Ok(profile);
    }

    [Authorize]
    [HttpPut("profile")]
    public async Task<IActionResult> UpdateProfile([FromBody] UpdateProfileDto request)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId))
        {
            return Unauthorized();
        }

        var (succeeded, errors) = await _identityService.UpdateUserProfileAsync(userId, request);
        if (!succeeded)
        {
            return BadRequest(new { errors });
        }

        return Ok(new { message = "Profile updated successfully." });
    }

    [HttpPost("reset-password")]
    public async Task<IActionResult> ResetPassword([FromBody] PasswordResetRequest request)
    {
        var (succeeded, errors) = await _identityService.ResetPasswordAsync(request);
        if (!succeeded)
        {
            return BadRequest(new { errors });
        }

        return Ok(new { message = "Password has been reset successfully." });
    }
}
