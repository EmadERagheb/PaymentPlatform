namespace Payments.API.Extensions;

public static class ApplicationBuilderExtensions
{
    public static void UseCustomExceptionHandler(this IApplicationBuilder app)
    {
        app.UseMiddleware<CustomExceptionHandler>();

    }
    public static IApplicationBuilder UsePerformanceLoggingMiddleware(this IApplicationBuilder app)
    {
        app.UseMiddleware<PerformanceLoggingMiddleware>();
        return app;
    }
    public static IApplicationBuilder UseCorrelationIdMiddleware(this IApplicationBuilder app)
    {
        app.UseMiddleware<CorrelationIdMiddleware>();
        return app;
    }
}
