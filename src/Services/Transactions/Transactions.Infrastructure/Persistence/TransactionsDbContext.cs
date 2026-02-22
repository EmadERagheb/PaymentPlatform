
using System.Reflection;
namespace Transactions.Infrastructure.Persistence;

public class TransactionsDbContext(DbContextOptions<TransactionsDbContext> options) : DbContext(options), ITransactionDbContext
{
    public DbSet<Transaction> Transactions { get; set; }
    public DbSet<TransactionItem> TransactionItems { get; set; }
    public DbSet<OutboxMessage> OutboxMessages { get; set; }
    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        base.OnModelCreating(builder);
    }
}
