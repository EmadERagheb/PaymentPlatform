namespace Payments.Infrastructure.Configurations;

public class PaymentConfiguration : IEntityTypeConfiguration<Payment>
{
    public void Configure(EntityTypeBuilder<Payment> builder)
    {
        builder.ToTable("Payments");
        builder.HasKey(p => p.Id);
        builder.Property(t => t.Id).HasConversion(id => id.Value, value => PaymentId.Of(value));
        builder.Property(t => t.TransactionId)
            .HasConversion(id => id.Value, value => TransactionId.Of(value))
            .IsRequired();
        builder.HasIndex(t => t.TransactionId).IsUnique();
        builder.ComplexProperty(t => t.TotalAmount, cb =>
        {
            cb.Property(x => x.Amount).HasColumnName(nameof(Money.Amount)).HasPrecision(8, 4);
            cb.Property(x => x.Currency).HasColumnName(nameof(Money.Currency))
                .HasConversion(c => c.Code, code => Currency.Of(code))
                .HasMaxLength(3)
                .IsRequired();
        });
        builder.Property(t => t.State).HasConversion<string>().IsRequired();

    }
}
