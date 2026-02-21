using Transactions.Domain.ValueObjects;
namespace Transactions.Infrastructure.Configurations;

public class TransactionConfiguration : IEntityTypeConfiguration<Transaction>
{
    public void Configure(EntityTypeBuilder<Transaction> builder)
    {
        builder.ToTable("Transactions");
        builder.HasKey(t => t.Id);
        builder.Property(t => t.Id).HasConversion(id => id.Value, value => TransactionId.Of(value));
        builder.ComplexProperty(t => t.TotalAmount, cb =>
        {
            cb.Property(x => x.Amount).HasColumnName(nameof(Money.Amount)).HasPrecision(8, 4);
            cb.Property(x => x.Currency).HasColumnName(nameof(Money.Currency))
                .HasConversion(c => c.Code, code => Currency.FromCode(code))
                .HasMaxLength(3)
                .IsRequired();
        });
        builder.Property(t => t.State).HasConversion<string>().IsRequired();
        builder.HasMany(t => t.Items)
            .WithOne()
            .HasForeignKey(ti => ti.TransactionId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
