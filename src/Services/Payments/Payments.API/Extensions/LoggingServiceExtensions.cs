namespace Payments.API.Extensions;

public static class LoggingServiceExtensions
{
    public static IApplicationBuilder UseAdvancedRequestLogging(this IApplicationBuilder app)
    {
        var loggingOptions = app.ApplicationServices.GetRequiredService<IOptions<LoggingOptions>>().Value;

        if (!loggingOptions.EnableRequestLogging) return app;
        app.UseSerilogRequestLogging(options =>
        {
            options.MessageTemplate = loggingOptions.RequestLogging.MessageTemplate;
            options.IncludeQueryInRequestPath = loggingOptions.RequestLogging.IncludeQueryInRequestPath;

            options.GetLevel = (httpContext, elapsed, ex) =>
            {
                // Error responses
                if (ex != null) return LogEventLevel.Error;
                if (httpContext.Response.StatusCode >= 500) return LogEventLevel.Error;
                if (httpContext.Response.StatusCode >= 400) return LogEventLevel.Warning;

                // Slow requests
                if (elapsed > loggingOptions.RequestLogging.SlowRequestThresholdMs)
                    return LogEventLevel.Warning;

                // Skip logging for excluded paths
                var path = httpContext.Request.Path.Value?.ToLowerInvariant();
                if (loggingOptions.RequestLogging.ExcludePaths.Any(excludePath =>
                    path?.StartsWith(excludePath.ToLowerInvariant()) == true))
                {
                    return LogEventLevel.Debug;
                }

                return LogEventLevel.Information;
            };

            options.EnrichDiagnosticContext = (diagnosticContext, httpContext) =>
            {
                EnrichWithBasicContext(diagnosticContext, httpContext);
                EnrichWithHeaders(diagnosticContext, httpContext, loggingOptions);
            };
        });

        return app;
    }
    private static void EnrichWithBasicContext(IDiagnosticContext diagnosticContext, HttpContext httpContext)
    {
        diagnosticContext.Set("RequestHost", httpContext.Request.Host.Value);
        diagnosticContext.Set("RequestScheme", httpContext.Request.Scheme);
        diagnosticContext.Set("RemoteIP", httpContext.Connection.RemoteIpAddress?.ToString());
        diagnosticContext.Set("UserAgent", httpContext.Request.Headers["User-Agent"].FirstOrDefault());
        diagnosticContext.Set("CorrelationId", httpContext.Items["CorrelationId"]);
        diagnosticContext.Set("RequestId", httpContext.TraceIdentifier);
    }
    private static void EnrichWithHeaders(IDiagnosticContext diagnosticContext, HttpContext httpContext, LoggingOptions options)
    {
        if (options.RequestLogging.LogRequestHeaders)
        {
            var safeHeaders = httpContext.Request.Headers
                .Where(h => !options.SensitiveHeaders.Contains(h.Key, StringComparer.OrdinalIgnoreCase))
                .ToDictionary(h => h.Key, h => h.Value.ToString());

            diagnosticContext.Set("RequestHeaders", safeHeaders, true);
        }

        if (options.RequestLogging.LogResponseHeaders)
        {
            var responseHeaders = httpContext.Response.Headers
                .Where(h => !options.SensitiveHeaders.Contains(h.Key, StringComparer.OrdinalIgnoreCase))
                .ToDictionary(h => h.Key, h => h.Value.ToString());

            diagnosticContext.Set("ResponseHeaders", responseHeaders, true);
        }
    }




}


