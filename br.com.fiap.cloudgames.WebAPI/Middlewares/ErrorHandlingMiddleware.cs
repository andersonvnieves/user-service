using System.Net;
using System.Text.Json;
using System.Security.Claims;
using br.com.fiap.cloudgames.Domain.Exceptions;

namespace br.com.fiap.cloudgames.WebAPI.Middlewares;

public class ErrorHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ErrorHandlingMiddleware> _logger;
    
    public ErrorHandlingMiddleware(
        RequestDelegate next,
        ILogger<ErrorHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }
    
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            var correlationId =
                context.Items.TryGetValue(RequestLoggingMiddleware.CorrelationItemName, out var fromItems) ? fromItems?.ToString() :
                context.Request.Headers.TryGetValue(RequestLoggingMiddleware.CorrelationHeaderName, out var fromHeader) ? fromHeader.ToString() :
                null;

            var method = context.Request.Method;
            var path = context.Request.Path;
            var queryString = context.Request.QueryString.Value;
            var traceId = context.TraceIdentifier;
            var userId =
                context.User.FindFirstValue(ClaimTypes.NameIdentifier) ??
                context.User.Identity?.Name;

            using var scope = _logger.BeginScope(new Dictionary<string, object?>
            {
                ["CorrelationId"] = correlationId,
                ["TraceId"] = traceId
            });

            _logger.LogError(
                ex,
                "Unhandled exception while processing HTTP {Method} {Path}{QueryString}. UserId={UserId}",
                method,
                path,
                queryString,
                userId);

            await HandleExceptionAsync(context, ex, correlationId);
        }
    }
    
    private static async Task HandleExceptionAsync(
        HttpContext context,
        Exception exception,
        string? correlationId)
    {
        if (context.Response.HasStarted)
            throw exception;

        HttpStatusCode statusCode;
        object response;
        switch (exception)
        {
            case DomainException domainEx:
                statusCode = HttpStatusCode.BadRequest;
                response = new
                {
                    message = "Business validation failed",
                    errors = domainEx.Errors,
                    status = (int)statusCode,
                    correlationId
                };
                break;

            case ArgumentNullException:
            case ArgumentException:
            case ApplicationException:
                statusCode = HttpStatusCode.BadRequest;
                response = new
                {
                    message = exception.Message,
                    errors = new[] { exception.Message },
                    status = (int)statusCode,
                    correlationId
                };
                break;
            case UnauthorizedAccessException:
                statusCode = HttpStatusCode.Unauthorized;
                response = new
                {
                    message = "Unauthorized",
                    status = (int)statusCode,
                    correlationId
                };
                break;
            default:
                statusCode = HttpStatusCode.InternalServerError;
                response = new
                {
                    message = "Unexpected error",
                    status = (int)statusCode,
                    correlationId
                };
                break;
        }

        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)statusCode;
        var json = JsonSerializer.Serialize(response);
        await context.Response.WriteAsync(json);
    }
}

public static class ErrorHandlingMiddlewareExtensions
{
    public static IApplicationBuilder UseErrorHandlingMiddleware(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<ErrorHandlingMiddleware>();
    }
}