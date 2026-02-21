namespace Payments.Domain.ValueObjects;

public sealed record PaymentId
{
    public Guid Value { get; }
    private PaymentId(Guid value)
    {
        Value = value;
    }
    public static PaymentId Of(Guid value)
    {
        if (value == Guid.Empty)
            throw new ValidationException([new ValidationError(nameof(PaymentId), "PaymentId cannot be empty.")]);
        return new PaymentId(value);
    }
}
