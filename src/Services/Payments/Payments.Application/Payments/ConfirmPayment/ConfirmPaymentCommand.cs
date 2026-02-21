namespace Payments.Application.Payments.ConfirmPayment;

public sealed record ConfirmPaymentCommand(Guid PaymentId) : ICommand<Result>;