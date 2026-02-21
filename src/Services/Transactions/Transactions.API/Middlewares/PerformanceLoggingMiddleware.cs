namespace Transactions.API.Middlewares;

public class PerformanceLoggingMiddleware(ILogger<PerformanceLoggingMiddleware> logger, IOptions<LoggingOptions> loggingOptions, RequestDelegate next)
{
    private readonly LoggingOptions options = loggingOptions.Value;
    public async Task InvokeAsync(HttpContext context)
    {
        if (!options.EnablePerformanceLogging)
        {
            await next(context);
            return;
        }
        var stopwatch = Stopwatch.StartNew();
        var initialMemory = options.Performance.LogMemoryUsage ? GC.GetTotalMemory(false) : 0;
        try
        {
            await next(context);
        }
        finally
        {
            stopwatch.Stop();
            var elapsed = stopwatch.ElapsedMilliseconds;

            if (elapsed > options.Performance.SlowOperationThresholdMs)
            {
                var logData = new Dictionary<string, object>
                {
                    ["RequestMethod"] = context.Request.Method,
                    ["RequestPath"] = context.Request.Path.Value ?? string.Empty,
                    ["StatusCode"] = context.Response.StatusCode,
                    ["ElapsedMs"] = elapsed,
                    ["CorrelationId"] = context.Items["CorrelationId"] ?? string.Empty
                };

                if (options.Performance.LogMemoryUsage)
                {
                    var finalMemory = GC.GetTotalMemory(false);
                    logData["MemoryUsedBytes"] = finalMemory - initialMemory;
                    logData["TotalMemoryBytes"] = finalMemory;
                }

                using var scope = logger.BeginScope(logData);

                logger.LogWarning("Slow request detected: {RequestMethod} {RequestPath} took {ElapsedMs}ms",
                    context.Request.Method, context.Request.Path, elapsed);
            }
        }
    }
}