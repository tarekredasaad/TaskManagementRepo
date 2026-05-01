using Application.Dtos;
using Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskManagement.Models;

namespace TaskManagement.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : BaseController
{
    private readonly IAuthService _authService;
    private readonly IUserService _userService;

    public AuthController(
        ControllerParameters controllerParameters,
        IAuthService authService,
        IUserService userService) : base(controllerParameters)
    {
        _authService = authService;
        _userService = userService;
    }

    [HttpPost("register")]
    [AllowAnonymous]
    public async Task<ActionResult<AuthResponse>> Register([FromBody] RegisterRequest request)
    {
        var result = await _authService.RegisterAsync(request);
        return Ok(result);
    }

    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<ActionResult<AuthResponse>> Login([FromBody] LoginRequest request)
    {
        var result = await _authService.LoginAsync(request);
        return Ok(result);
    }

    [HttpGet("me")]
    [Authorize(Roles = "User,Admin")]
    public async Task<ActionResult<UserDto>> Me()
    {
        var user = await _userService.GetByIdAsync(CurrentUserId);
        return user is null ? NotFound() : Ok(user);
    }
}
