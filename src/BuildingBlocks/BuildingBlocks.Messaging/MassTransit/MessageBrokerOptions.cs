

namespace BuildingBlocks.Messaging.MassTransit;

public class MessageBrokerOptions
{
    public const string SectionName = "MessageBroker";
    public string Host { get; set; } = default!;
    public string Username { get; set; } = default!;
    public string Password { get; set; } = default!;
}
