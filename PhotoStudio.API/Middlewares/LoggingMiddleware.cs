using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

public class LoggingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<LoggingMiddleware> _logger;

    public LoggingMiddleware(RequestDelegate next, ILogger<LoggingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var start = DateTime.UtcNow;

        // Logovanje početka obrade zahteva
        _logger.LogInformation("Request starting: {Method} {Url} at {StartTime}",
            context.Request.Method,
            context.Request.Path,
            start.ToString("o")); // "o" za ISO 8601 format

        // Poziv sledećeg middleware-a u pipeline-u
        await _next(context);

        var elapsedMs = (DateTime.UtcNow - start).TotalMilliseconds;

        // Logovanje završetka obrade zahteva
        _logger.LogInformation("Request finished: {Method} {Url} responded {StatusCode} in {ElapsedMilliseconds}ms",
            context.Request.Method,
            context.Request.Path,
            context.Response.StatusCode,
            elapsedMs);
    }
}
