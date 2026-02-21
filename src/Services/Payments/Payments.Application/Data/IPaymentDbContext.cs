namespace Payments.Application.Data;

public interface IPaymentDbContext
{
    public DbSet<Payment> Payments { get; set; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
