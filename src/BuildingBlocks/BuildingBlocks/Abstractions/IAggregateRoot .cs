namespace BuildingBlocks.Abstractions;

public interface IAggregateRoot:IEntity
{
    IReadOnlyCollection<IDomainEvent> DomainEvents { get; }
    void AddDomainEvent(IDomainEvent domainEvent);
    IDomainEvent[] ClearDomainEvents();
}
public interface IAggregateRoot<T> : IEntity<T>, IAggregateRoot { }
