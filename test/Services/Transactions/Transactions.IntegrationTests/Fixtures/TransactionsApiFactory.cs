using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using RabbitMQ.Client;
using Testcontainers.MsSql;
using Testcontainers.RabbitMq;
using Transactions.Infrastructure.Persistence;


namespace Transactions.IntegrationTests.Fixtures;

public class TransactionsApiFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
    private readonly MsSqlContainer _dbContainer = new MsSqlBuilder()
        .WithImage("mcr.microsoft.com/mssql/server:2022-latest")
        .WithPassword("Password123")
        .Build();
    private readonly RabbitMqContainer _rabbitMqContainer = new RabbitMqBuilder()
        .WithImage("rabbitmq:3-management")
        .WithUsername("guest")
        .WithPassword("guest")
        .Build();

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureTestServices(services =>
        {
            // Remove the existing DbContext registration
            var descriptor = services.SingleOrDefault(
                d => d.ServiceType == typeof(DbContextOptions<TransactionsDbContext>));

            // Remove the existing RabbitMQ connection factory registration
            var rabbitMqDescriptor = services.SingleOrDefault(
                d => d.ServiceType == typeof(IConnectionFactory));

            if (descriptor != null)
                services.Remove(descriptor);
            if (rabbitMqDescriptor != null)
                services.Remove(rabbitMqDescriptor);
            // Add DbContext using the Testcontainer connection string
            services.AddDbContext<TransactionsDbContext>(options =>
            {
                options.UseSqlServer(_dbContainer.GetConnectionString());
            });
            // Add RabbitMQ connection factory using the Testcontainer connection details
            services.AddSingleton<IConnectionFactory>(new ConnectionFactory
            {
                HostName = _rabbitMqContainer.Hostname,
                Port = _rabbitMqContainer.GetMappedPublicPort(5672),
                UserName = "guest",
                Password = "guest"
            });
        });
    }

    public async Task InitializeAsync()
    {
        await _dbContainer.StartAsync();
        await _rabbitMqContainer.StartAsync();

        // Ensure database is created and migrations are applied
        using var scope = Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<TransactionsDbContext>();
        await dbContext.Database.MigrateAsync();
    }

    public new async Task DisposeAsync()
    {
        await _dbContainer.DisposeAsync();
        await _rabbitMqContainer.DisposeAsync();
        await base.DisposeAsync();
    }
}
