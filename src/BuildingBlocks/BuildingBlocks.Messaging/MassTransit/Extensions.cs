using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
namespace BuildingBlocks.Messaging.MassTransit;

public static class Extensions
{
    public static IServiceCollection AddMessageBroker(this IServiceCollection services, IConfiguration configuration, Assembly? assembly = null)
    {
        var options = configuration.GetSection(MessageBrokerOptions.SectionName).Get<MessageBrokerOptions>();
        services.AddMassTransit(config =>
        {
            config.SetKebabCaseEndpointNameFormatter();
            if (assembly is not null)
                config.AddConsumers(assembly);
            config.UsingRabbitMq((context, cfg) =>
            {
                cfg.Host(new Uri(options.Host), host =>
                {
                    host.Username(options.Username);
                    host.Password(options.Password);
                });
                cfg.ConfigureEndpoints(context);
            });
        });
        return services;
    }
}
