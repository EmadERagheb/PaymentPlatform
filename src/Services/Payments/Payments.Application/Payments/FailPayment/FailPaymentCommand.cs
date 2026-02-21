namespace Payments.Application.Payments.FailPayment;

public sealed record FailPaymentCommand(Guid PaymentId) : ICommand<Result>;

