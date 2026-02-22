namespace Transactions.Domain.Events;

public sealed record TransactionSubmitDomainEvent(Guid TransactionId ,string Currency,decimal TotalAmount) : IDomainEvent;

