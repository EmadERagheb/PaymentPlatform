using System.Reflection;
using Transactions.Application;
using Transactions.Domain.Aggregates;
using Transactions.Infrastructure.Persistence;

namespace Transactions.ArchitectureTests.Infrastructure;

public class BaseTest
{
    protected static readonly Assembly DomainAssembly = typeof(Transaction).Assembly;
    protected static readonly Assembly ApplicationAssembly = typeof(DependencyInjection).Assembly;
    protected static readonly Assembly InfrastructureAssembly = typeof(TransactionsDbContext).Assembly;
    protected static readonly Assembly ApiAssembly = typeof(Program).Assembly;
}
