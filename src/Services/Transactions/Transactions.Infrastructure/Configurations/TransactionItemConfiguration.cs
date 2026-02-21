using Transactions.Domain.ValueObjects;
namespace Transactions.Infrastructure.Configurations;

public class TransactionItemConfiguration : IEntityTypeConfiguration<TransactionItem>
{
    public void Configure(EntityTypeBuilder<TransactionItem> builder)
    {
        builder.ToTable("TransactionItems");
        builder.HasKey(ti => ti.Id);
        builder.Property(x => x.Id).HasConversion(id => id.Value, value => TransactionItemId.Of(value));
        builder.Property(ti => ti.Description).IsRequired().HasMaxLength(500);
        builder.Property(ti => ti.UnitPrice).HasPrecision(18, 4);
    }
}
