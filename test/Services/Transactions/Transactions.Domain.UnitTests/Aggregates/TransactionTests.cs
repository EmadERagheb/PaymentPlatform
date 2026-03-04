using BuildingBlocks.Exceptions;
using BuildingBlocks.ValueObjects;
using FluentAssertions;
using Transactions.Domain.Aggregates;
using Transactions.Domain.Events;
using Transactions.Domain.ValueObjects;
namespace Transactions.Domain.UnitTests.Aggregates;

public class TransactionTests
{
    [Fact]
    public void CreateTransaction_ShouldInitializeWithDraftState()
    {
        // Arrange
        var currency = Currency.Usd;
        // Act
        var transaction = new Transaction(currency);
        // Assert
        transaction.State.Should().Be(TransactionState.Draft);
        transaction.TotalAmount.Should().Be(Money.Zero(currency));
        transaction.Items.Should().BeEmpty();
    }
    [Fact]
    public void AddItem_ShouldAddItemAndRecalculateTotal()
    {
        // Arrange
        var transaction = new Transaction(Currency.Usd);
        // Act
        transaction.AddItem("Item 1", 2, 10); // Line total = 20
        // Assert
        transaction.Items.Should().HaveCount(1);
        transaction.TotalAmount.Amount.Should().Be(20);
    }
    [Fact]
    public void Submit_ShouldChangeStateToSubmitted()
    {
        // Arrange
        var transaction = new Transaction(Currency.Usd);
        transaction.AddItem("Item 1", 2, 10);
        // Act
        transaction.Submit();
        // Assert
        transaction.State.Should().Be(TransactionState.Submitted);
    }
    [Fact]
    public void Cancel_ShouldChangeStateToCancelled()
    {
        // Arrange
        var transaction = new Transaction(Currency.Usd);
        // Act
        transaction.Cancel();
        // Assert
        transaction.State.Should().Be(TransactionState.Cancelled);
    }
    [Fact]
    public void Complete_ShouldChangeStateToCompleted()
    {             // Arrange
        var transaction = new Transaction(Currency.Usd);
        transaction.AddItem("Item 1", 2, 10);
        transaction.Submit();
        // Act
        transaction.Complete();
        // Assert
        transaction.State.Should().Be(TransactionState.Completed);
    }
    [Fact]
    public void CannotAddItemToNonDraftTransaction()
    {
        // Arrange
        var transaction = new Transaction(Currency.Usd);
        transaction.AddItem("Item 1", 2, 10);
        transaction.Submit();
        // Act
        Action act = () => transaction.AddItem("Item 2", 1, 5);
        // Assert
        act.Should().Throw<ValidationException>()
            .Which.Errors.Should().ContainSingle(e =>
        e.PropertyName == "State" &&
        e.ErrorMessage == "Only draft transactions can add items.");
    }
    [Fact]
    public void CannotSubmitEmptyTransaction()
    {
        // Arrange
        var transaction = new Transaction(Currency.Usd);
        // Act
        Action act = () => transaction.Submit();
        // Assert
        act.Should().Throw<ValidationException>()
            .WithMessage("Cannot submit a transaction with no items.");
    }
    [Fact]
    public void CannotCompleteNonSubmittedTransaction()
    {
        // Arrange
        var transaction = new Transaction(Currency.Usd);
        // Act
        Action act = () => transaction.Complete();
        // Assert
        act.Should().Throw<ValidationException>()
            .Which.Errors.Should().ContainSingle(e =>
        e.PropertyName == "State" &&
        e.ErrorMessage == "Only submitted transactions can be completed.");
    }
    [Fact]
    public void ShouldRaiseDomainEventOnSubmit()
    {
        // Arrange
        var transaction = new Transaction(Currency.Usd);
        transaction.AddItem("Item 1", 2, 10);
        // Act
        transaction.Submit();
        // Assert
        var domainEvent = transaction.DomainEvents.OfType<TransactionSubmitDomainEvent>().SingleOrDefault();
        domainEvent.Should().NotBeNull();
        domainEvent.TransactionId.Should().Be(transaction.Id.Value);
        domainEvent.Currency.Should().Be(transaction.TotalAmount.Currency.Code);
        domainEvent.TotalAmount.Should().Be(transaction.TotalAmount.Amount);
    }
}