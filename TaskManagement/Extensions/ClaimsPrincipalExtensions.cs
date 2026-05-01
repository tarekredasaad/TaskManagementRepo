using Domain.Constants;
using System.Security.Claims;

namespace TaskManagement.Extensions;

public static class ClaimsPrincipalExtensions
{
    public static Guid GetUserId(this ClaimsPrincipal principal)
    {
        var userId = principal.FindFirstValue(CustomClaimTypes.Id);
        return userId is null ? Guid.Empty : Guid.Parse(userId);
    }
}
