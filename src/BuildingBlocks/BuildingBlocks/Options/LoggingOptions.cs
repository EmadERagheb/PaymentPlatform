
namespace BuildingBlocks.Options;

public class LoggingOptions
{
    public const string SectionName = "Logging";

    public bool EnableRequestLogging { get; set; } = true;
    public bool EnablePerformanceLogging { get; set; } = true;
    public bool EnableDetailedErrors { get; set; } = false;
    public bool EnableStructuredLogging { get; set; } = true;
    public string[] SensitiveHeaders { get; set; } = { "Authorization", "Cookie", "X-API-Key" };
    public string[] SensitiveProperties { get; set; } = { "password", "token", "key", "secret" };
    public RequestLoggingOptions RequestLogging { get; set; } = new();
    public PerformanceLoggingOptions Performance { get; set; } = new();

}
public class PerformanceLoggingOptions
{
    public bool Enabled { get; set; } = true;
    public int SlowOperationThresholdMs { get; set; } = 2000;
    public bool LogMemoryUsage { get; set; } = false;
    public bool LogCpuUsage { get; set; } = false;
}
public class RequestLoggingOptions
{
    public string MessageTemplate { get; set; } = "HTTP {RequestMethod} {RequestPath} responded {StatusCode} in {Elapsed:0.0000} ms";
    public bool IncludeQueryInRequestPath { get; set; } = false;
    public bool LogRequestHeaders { get; set; } = false;
    public bool LogResponseHeaders { get; set; } = false;
    public string[] ExcludePaths { get; set; } = { "/health", "/metrics", "/swagger" };
    public int SlowRequestThresholdMs { get; set; } = 5000;
}

