using Serilog;
using Serilog.Events;
using Serilog.Filters;
using Serilog.Sinks.Grafana.Loki;
namespace Transactions.API.Extensions;

public static class SerilogExtensions
{
    public static IHostBuilder ConfigureSerilog(this IHostBuilder hostBuilder)
    {
        hostBuilder.UseSerilog((context, services, configuration) =>
        {
            configuration
            .ReadFrom.Configuration(context.Configuration)
            .ReadFrom.Services(services)
            .Enrich.WithMachineName()
            .Enrich.WithThreadId()
            .Enrich.WithEnvironmentName()
            .Enrich.WithProcessId()
            .Enrich.WithProperty("Application", "TransactionsAPI")
            .ConfigureLokiSink(context.Configuration, context.HostingEnvironment)
            .ConfigureFilters(context.Configuration)
            .ConfigureEnrichment(context.HostingEnvironment);
        });
        return hostBuilder;
    }
    private static LoggerConfiguration ConfigureLokiSink(this LoggerConfiguration configuration, IConfiguration config, IHostEnvironment environment)
    {
        var lokiOptions = config.GetSection(LokiOptions.SectionName).Get<LokiOptions>() ?? new LokiOptions();
        var lokiUri = lokiOptions.Url;
        if (string.IsNullOrEmpty(lokiUri)) return configuration;
        var labels = new List<Serilog.Sinks.Grafana.Loki.LokiLabel>
        {
            new() { Key = "app", Value = "transactions-api" },
            new() { Key = "environment", Value = environment.EnvironmentName.ToLower() },
            new() { Key = "machine", Value = Environment.MachineName.ToLower() }
        };

        // Add custom labels from configuration
        if (lokiOptions.DefaultLabels?.Any() == true)
        {
            labels.AddRange(lokiOptions.DefaultLabels.Select(l =>
                new Serilog.Sinks.Grafana.Loki.LokiLabel { Key = l.Key, Value = l.Value }));
        }

        var credentials = !string.IsNullOrEmpty(lokiOptions.Username) && !string.IsNullOrEmpty(lokiOptions.Password)
            ? new LokiCredentials { Login = lokiOptions.Username, Password = lokiOptions.Password }
            : null;

        // Parse minimum level
        Enum.TryParse<LogEventLevel>(lokiOptions.RestrictedToMinimumLevel, out var minLevel);

        configuration.WriteTo.GrafanaLoki(
            uri: lokiUri,
            labels: labels,
            propertiesAsLabels: lokiOptions.PropertiesAsLabels,
            credentials: credentials,
            restrictedToMinimumLevel: minLevel,
            period: lokiOptions.Period,
            batchPostingLimit: lokiOptions.BatchSizeLimit,
            queueLimit: lokiOptions.QueueLimit
        );

        return configuration;
    }

    private static LoggerConfiguration ConfigureFilters(this LoggerConfiguration configuration, IConfiguration config)
    {
        // Filter out health check noise
        configuration.Filter.ByExcluding(Matching.WithProperty<string>("RequestPath", path =>
            path.Contains("/health") || path.Contains("/metrics")));

        // Filter out static files in production
        if (config.GetValue<string>("ASPNETCORE_ENVIRONMENT") == "Production")
        {
            configuration.Filter.ByExcluding(Matching.WithProperty<string>("RequestPath", path =>
                path.Contains(".css") || path.Contains(".js") || path.Contains(".ico")));
        }
        return configuration;
    }

    private static LoggerConfiguration ConfigureEnrichment(this LoggerConfiguration configuration, IHostEnvironment environment)
    {
        // Add environment-specific enrichment
        if (environment.IsProduction())
        {
            configuration.Enrich.WithProperty("Tier", "Production");
        }
        else if (environment.IsStaging())
        {
            configuration.Enrich.WithProperty("Tier", "Staging");
        }
        else
        {
            configuration.Enrich.WithProperty("Tier", "Development");
        }

        return configuration;
    }
}

