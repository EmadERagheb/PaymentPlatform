using Testcontainers.RabbitMq;

namespace Transactions.IntegrationTests.Fixtures;

internal class RabbitMqContainerFixture: IAsyncLifetime
{
    private readonly RabbitMqContainer _rabbitMqContainer= new RabbitMqBuilder()
        .WithUsername("guest")
        .WithPassword("guest")
        .WithImage("rabbitmq:3-management-alpine")
        .Build();

    public async Task DisposeAsync() => await _rabbitMqContainer.DisposeAsync();

    public Task InitializeAsync()
    {
        return _rabbitMqContainer.StartAsync();
    }


 
}
