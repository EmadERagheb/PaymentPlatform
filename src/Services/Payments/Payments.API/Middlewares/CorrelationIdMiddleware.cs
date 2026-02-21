namespace Payments.API.Middlewares;

public class CorrelationIdMiddleware(RequestDelegate next, ILogger<CorrelationIdMiddleware> logger, IOptions<LoggingOptions> loggingOptions)
{
    private const string CorrelationIdHeader = "X-Correlation-ID";
    public async Task InvokeAsync(HttpContext context)
    {
        var correlationId = GetOrCreateCorrelationId(context);
        // Add to response headers
        context.Response.Headers.Add(CorrelationIdHeader, correlationId);

        // Store in HttpContext for use in controllers and other middleware
        context.Items["CorrelationId"] = correlationId;
        context.Items["RequestStartTime"] = DateTimeOffset.UtcNow;
        // Add to Serilog LogContext for all subsequent logs
        using (Serilog.Context.LogContext.PushProperty("CorrelationId", correlationId))
        using (Serilog.Context.LogContext.PushProperty("RequestId", context.TraceIdentifier))
        {
            if (loggingOptions.Value.EnableStructuredLogging)
            {
                logger.LogDebug("Processing request {RequestMethod} {RequestPath} with CorrelationId: {correlationId}",
                    context.Request.Method, context.Request.Path, correlationId);
            }

            await next(context);

            if (loggingOptions.Value.EnableStructuredLogging)
            {
                var duration = DateTimeOffset.UtcNow - (DateTimeOffset)context.Items["RequestStartTime"]!;
                logger.LogDebug("Completed request {RequestMethod} {RequestPath} with status {StatusCode} in {Duration}ms",
                    context.Request.Method, context.Request.Path, context.Response.StatusCode, duration.TotalMilliseconds);
            }
        }
    }
    private string GetOrCreateCorrelationId(HttpContext context)
    {
        var correlationId = context.Request.Headers[CorrelationIdHeader].FirstOrDefault();

        if (string.IsNullOrEmpty(correlationId))
        {
            correlationId = Guid.NewGuid().ToString("N")[..12]; // Shorter correlation ID
        }
        return correlationId;
    }
}

