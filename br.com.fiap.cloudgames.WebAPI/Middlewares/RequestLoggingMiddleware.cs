using System.Diagnostics;
using System.Security.Claims;

namespace br.com.fiap.cloudgames.WebAPI.Middlewares;

public class RequestLoggingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<RequestLoggingMiddleware> _logger;
    public const string CorrelationHeaderName = "X-Correlation-ID";
    public const string CorrelationItemName = "CorrelationId";

    public RequestLoggingMiddleware(RequestDelegate next, ILogger<RequestLoggingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var startTimestamp = Stopwatch.GetTimestamp();
        var correlationId = GetOrCreateCorrelationId(context);

        context.Items[CorrelationItemName] = correlationId;
        context.Response.Headers[CorrelationHeaderName] = correlationId;

        var method = context.Request.Method;
        var path = context.Request.Path;
        var queryString = context.Request.QueryString.Value;
        var traceId = context.TraceIdentifier;
        var remoteIp = context.Connection.RemoteIpAddress?.ToString();
        var userAgent = context.Request.Headers.UserAgent.ToString();
        var contentType = context.Request.ContentType;
        var contentLength = context.Request.ContentLength;
        var userId =
            context.User.FindFirstValue(ClaimTypes.NameIdentifier) ??
            context.User.Identity?.Name;

        using var scope = _logger.BeginScope(new Dictionary<string, object?>
        {
            ["CorrelationId"] = correlationId,
            ["TraceId"] = traceId
        });

        _logger.LogInformation(
            "HTTP {Method} {Path}{QueryString} started. RemoteIp={RemoteIp}, UserId={UserId}, UserAgent={UserAgent}, ContentType={ContentType}, ContentLength={ContentLength}",
            method,
            path,
            queryString,
            remoteIp,
            userId,
            userAgent,
            contentType,
            contentLength);

        try
        {
            await _next(context);
        }
        finally
        {
            var elapsedMs = (long)Stopwatch.GetElapsedTime(startTimestamp).TotalMilliseconds;
            var statusCode = context.Response.StatusCode;
            var responseContentType = context.Response.ContentType;

            var level = statusCode >= 500 ? LogLevel.Error :
                statusCode >= 400 ? LogLevel.Warning :
                LogLevel.Information;

            _logger.Log(
                level,
                "HTTP {Method} {Path}{QueryString} finished. StatusCode={StatusCode}, ElapsedMs={ElapsedMs}, ResponseContentType={ResponseContentType}",
                method,
                path,
                queryString,
                statusCode,
                elapsedMs,
                responseContentType);
        }
    }

    private static string GetOrCreateCorrelationId(HttpContext context)
    {
        if(context.Request.Headers.TryGetValue(CorrelationHeaderName, out var correlationId))
            return correlationId;
        
        return Guid.NewGuid().ToString();
    }
}

public static class RequestLoggingMiddlewareExtensions
{
    public static IApplicationBuilder UseRequestLoggingMiddleware(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<RequestLoggingMiddleware>();
    }
}