using Application.Dtos;
using Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskManagement.Models;

namespace TaskManagement.Controllers;

[ApiController]
[Route("api/admin/users")]
[Authorize(Roles = "Admin")]
public class AdminUsersController : BaseController
{
    private readonly IUserService _userService;

    public AdminUsersController(ControllerParameters controllerParameters, IUserService userService) : base(controllerParameters)
    {
        _userService = userService;
    }

    [HttpGet]
    public async Task<ActionResult<List<UserDto>>> GetAll()
    {
        var users = await _userService.GetAllAsync();
        return Ok(users);
    }

    [HttpPost]
    public async Task<ActionResult<UserDto>> Create([FromBody] RegisterRequest request, [FromQuery] string role = "User")
    {
        var created = await _userService.CreateByAdminAsync(request, role);
        return CreatedAtAction(nameof(GetAll), new { id = created.Id }, created);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var deleted = await _userService.DeleteAsync(id);
        return deleted ? NoContent() : NotFound("User not found or cannot delete admin user.");
    }
}
