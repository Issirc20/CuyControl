namespace CuyControl.Web.Middlewares;

/// <summary>
/// Middleware para registro de peticiones HTTP.
/// </summary>
public class RequestLoggingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<RequestLoggingMiddleware> _logger;

    /// <summary>
    /// Constructor del middleware.
    /// </summary>
    public RequestLoggingMiddleware(RequestDelegate next, ILogger<RequestLoggingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    /// <summary>
    /// Invoca el middleware.
    /// </summary>
    public async Task InvokeAsync(HttpContext context)
    {
        var startTime = DateTime.UtcNow;
        var requestMethod = context.Request.Method;
        var requestPath = context.Request.Path.Value;

        try
        {
            await _next(context);

            var duration = (DateTime.UtcNow - startTime).TotalMilliseconds;
            var statusCode = context.Response.StatusCode;

            _logger.LogInformation(
                $"HTTP {requestMethod} {requestPath} respondió con {statusCode} en {duration:F2}ms"
            );
        }
        catch (Exception ex)
        {
            var duration = (DateTime.UtcNow - startTime).TotalMilliseconds;
            _logger.LogError(
                $"HTTP {requestMethod} {requestPath} lanzó una excepción en {duration:F2}ms: {ex.Message}"
            );
            throw;
        }
    }
}

/// <summary>
/// Extensión para agregar el middleware de logging de peticiones.
/// </summary>
public static class RequestLoggingMiddlewareExtensions
{
    /// <summary>
    /// Agrega el middleware de logging de peticiones.
    /// </summary>
    public static IApplicationBuilder UseRequestLogging(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<RequestLoggingMiddleware>();
    }
}
