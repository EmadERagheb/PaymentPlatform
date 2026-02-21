using Microsoft.EntityFrameworkCore;
using Transactions.Domain.Aggregates;

namespace Transactions.Application.Data;

public interface ITransactionDbContext
{
    public DbSet<Transaction> Transactions { get; }
    public DbSet<TransactionItem> TransactionItems { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    int SaveChanges();
}
