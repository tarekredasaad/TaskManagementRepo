using System.Security.Claims;
using Domain.Constants;
using Microsoft.AspNetCore.Mvc;
using TaskManagement.Models;

namespace TaskManagement.Controllers;

public abstract class BaseController : ControllerBase
{
    protected readonly UserState _userState;

    protected BaseController(ControllerParameters controllerParameters)
    {
        _userState = controllerParameters.UserState;

        var loggedUser = controllerParameters.HttpContextAccessor.HttpContext?.User;
        _userState.Role =
            loggedUser?.FindFirst(CustomClaimTypes.RoleId)?.Value ??
            loggedUser?.FindFirst(CustomClaimTypes.Role)?.Value ??
            loggedUser?.FindFirst(ClaimTypes.Role)?.Value ??
            string.Empty;

        _userState.ID = loggedUser?.FindFirst(CustomClaimTypes.Id)?.Value ?? string.Empty;

        _userState.Name =
            loggedUser?.FindFirst(CustomClaimTypes.Name)?.Value ??
            loggedUser?.FindFirst(ClaimTypes.Name)?.Value ??
            string.Empty;
    }

    protected Guid CurrentUserId
    {
        get
        {
            if (Guid.TryParse(_userState.ID, out var userId))
            {
                return userId;
            }

            throw new UnauthorizedAccessException("User identifier claim is missing or invalid.");
        }
    }
}
