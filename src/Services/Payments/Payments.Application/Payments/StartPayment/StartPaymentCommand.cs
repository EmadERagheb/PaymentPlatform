namespace Payments.Application.Payments.StartPayment;

public sealed record StartPaymentCommand(Guid TransactionId, decimal Amount, string Currency) : ICommand<Result<StartedPaymentResponse>>;
