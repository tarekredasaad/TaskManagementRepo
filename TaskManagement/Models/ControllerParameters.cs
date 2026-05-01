using Microsoft.AspNetCore.Http;

namespace TaskManagement.Models;

public class ControllerParameters
{
    public UserState UserState { get; set; } = new();
    public IHttpContextAccessor HttpContextAccessor { get; set; } = default!;
}
