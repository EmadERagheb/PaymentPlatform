
namespace Transactions.Domain.Events;

public sealed record TransactionCompletedEvent(Transaction Transaction) : IDomainEvent;

