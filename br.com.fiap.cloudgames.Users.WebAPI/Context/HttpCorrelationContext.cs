using br.com.fiap.cloudgames.Users.Application.Abstractions;
using br.com.fiap.cloudgames.Users.WebAPI.Middlewares;

namespace br.com.fiap.cloudgames.Users.WebAPI.Context;

public class HttpCorrelationContext : ICorrelationContext
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public HttpCorrelationContext(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public string CorrelationId
    {
        get
        {
            var context = _httpContextAccessor.HttpContext;
            if (context == null)
                return Guid.NewGuid().ToString();
            if (context.Items.TryGetValue(RequestLoggingMiddleware.CorrelationItemName, out var value))
                return value?.ToString() ?? Guid.NewGuid().ToString();
            return Guid.NewGuid().ToString();
        }
    }
}