using System.Net;
using System.Text.Json;

namespace TaskManagement.Middleware;

public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;

    public ExceptionHandlingMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
            await HandleAuthorizationResponseAsync(context);
        }
        catch (UnauthorizedAccessException ex)
        {
            await WriteError(context, HttpStatusCode.Unauthorized, ex.Message);
        }
        catch (InvalidOperationException ex)
        {
            await WriteError(context, HttpStatusCode.BadRequest, ex.Message);
        }
        catch (Exception)
        {
            await WriteError(context, HttpStatusCode.InternalServerError, "An unexpected error occurred.");
        }
    }

    private static async Task HandleAuthorizationResponseAsync(HttpContext context)
    {
        if (context.Response.HasStarted)
        {
            return;
        }

        if (context.Response.StatusCode == StatusCodes.Status401Unauthorized)
        {
            await WriteError(context, HttpStatusCode.Unauthorized, "Authentication is required to access this resource.");
            return;
        }

        if (context.Response.StatusCode == StatusCodes.Status403Forbidden)
        {
            await WriteError(context, HttpStatusCode.Forbidden, "You are not authorized to perform this action.");
        }
    }

    private static async Task WriteError(HttpContext context, HttpStatusCode statusCode, string message)
    {
        context.Response.Clear();
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)statusCode;
        var payload = JsonSerializer.Serialize(new { message });
        await context.Response.WriteAsync(payload);
    }
}
