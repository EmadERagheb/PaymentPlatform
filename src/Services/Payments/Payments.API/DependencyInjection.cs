

namespace Payments.API;

public static class DependencyInjection
{
    public static IServiceCollection AddPresentation(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddControllers();
        services.AddCors(options =>
            options.AddDefaultPolicy(policy =>
            {
                policy.AllowAnyOrigin()
                      .AllowAnyMethod()
                      .AllowAnyHeader();
            })
        );
        services.Configure<ApiBehaviorOptions>(options =>
        {
            options.InvalidModelStateResponseFactory = context =>
            {
                var logger = context.HttpContext.RequestServices.GetRequiredService<ILogger<Program>>();
                var fieldErrors = context.ModelState
                    .Where(x => x.Value.Errors.Any())
                    .ToDictionary(x => x.Key, x => x.Value.Errors.Select(e => e.ErrorMessage).ToArray());

                logger.LogWarning("Model validation failed for {Action}. Field errors: {@FieldErrors}",
                    context.ActionDescriptor.DisplayName,
                    fieldErrors);
                return new BadRequestObjectResult(new ValidationProblemDetails(context.ModelState));
            };
        });
        services.AddHealthChecks(configuration);

        return services;

    }
    public static IServiceCollection AddConfigurations(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<LoggingOptions>(options =>
        {
            configuration.GetSection(LoggingOptions.SectionName).Bind(options);
            // Validate configuration
            if (options.RequestLogging.SlowRequestThresholdMs <= 0)
                options.RequestLogging.SlowRequestThresholdMs = 5000;

            if (options.Performance.SlowOperationThresholdMs <= 0)
                options.Performance.SlowOperationThresholdMs = 1000;
        });
        services.Configure<LokiOptions>(configuration.GetSection(LokiOptions.SectionName));
        services.Configure<OutboxOptions>(configuration.GetSection(OutboxOptions.SectionName));
        return services;
    }
    private static IServiceCollection AddHealthChecks(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddHealthChecks().AddSqlServer(configuration.GetConnectionString("DefaultConnection")!);

        return services;
    }
}
