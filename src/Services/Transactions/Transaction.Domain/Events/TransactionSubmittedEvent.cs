namespace Transactions.Domain.Events;

public sealed record TransactionSubmittedEvent(Transaction Transaction) : IDomainEvent;

