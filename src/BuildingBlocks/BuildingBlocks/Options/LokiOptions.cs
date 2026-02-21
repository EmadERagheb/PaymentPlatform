
namespace BuildingBlocks.Options;

public class LokiOptions
{
    public const string SectionName = "Loki";
    public string? Username { get; set; }
    public string? Password { get; set; }
    public string? Url { get; set; }
    public LokiLabel[] DefaultLabels { get; set; } = Array.Empty<LokiLabel>();
    public string[] PropertiesAsLabels { get; set; } = { "level", "RequestId", "CorrelationId" };
    public bool EnableBatching { get; set; } = true;
    public int BatchSizeLimit { get; set; } = 1000;
    public TimeSpan Period { get; set; } = TimeSpan.FromSeconds(5);
    public int QueueLimit { get; set; } = 10000;
    public string RestrictedToMinimumLevel { get; set; } = "Information";
}
public class LokiLabel
{
    public string Key { get; set; } = string.Empty;
    public string Value { get; set; } = string.Empty;
}