
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Transactions.Infrastructure.Persistence;

namespace Transactions.IntegrationTests.Fixtures;

public abstract class BaseIntegrationTest:IClassFixture<TransactionsApiFactory>
{
    private readonly IServiceScope _scope;
    protected readonly ISender sender;
    protected readonly TransactionsDbContext dbContext;
       
    protected BaseIntegrationTest(TransactionsApiFactory factory)
    {
        _scope = factory.Services.CreateScope();
        sender = _scope.ServiceProvider.GetRequiredService<ISender>();
        dbContext = _scope.ServiceProvider.GetRequiredService<TransactionsDbContext>();
    }
}
