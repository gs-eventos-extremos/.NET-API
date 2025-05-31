using AspNetCoreRateLimit;
using Microsoft.Extensions.Options;

namespace WeatherEmergencyAPI.Middleware
{
    public class RateLimitMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<RateLimitMiddleware> _logger;

        public RateLimitMiddleware(RequestDelegate next, ILogger<RateLimitMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            // Log quando rate limit é atingido
            if (context.Response.StatusCode == 429)
            {
                var endpoint = $"{context.Request.Method} {context.Request.Path}";
                var clientId = context.Request.Headers["X-ClientId"].FirstOrDefault() ?? "anonymous";
                var ip = context.Connection.RemoteIpAddress?.ToString() ?? "unknown";

                _logger.LogWarning($"Rate limit excedido - Endpoint: {endpoint}, ClientId: {clientId}, IP: {ip}");
            }

            await _next(context);
        }
    }

    public static class RateLimitMiddlewareExtensions
    {
        public static IApplicationBuilder UseCustomRateLimit(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<RateLimitMiddleware>();
        }
    }
}