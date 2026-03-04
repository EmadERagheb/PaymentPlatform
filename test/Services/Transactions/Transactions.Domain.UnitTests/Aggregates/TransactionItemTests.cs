using BuildingBlocks.Exceptions;
using BuildingBlocks.ValueObjects;
using FluentAssertions;
using Transactions.Domain.Aggregates;
using Transactions.Domain.ValueObjects;

namespace Transactions.Domain.UnitTests.Aggregates;

public class TransactionItemTests
{
    [Fact]
    public void CreateTransactionItem_WithValidInputs_ShouldSucceed()
    {
        // Arrange
        var transaction = new Transaction(Currency.Usd);
        
        // Act
        transaction.AddItem("Test Item", 2, 10.50m);
        
        // Assert
        var item = transaction.Items.Single();
        item.Description.Should().Be("Test Item");
        item.Quantity.Should().Be(2);
        item.UnitPrice.Should().Be(10.50m);
        item.LineTotal.Should().Be(21.00m);
        item.TransactionId.Should().Be(transaction.Id);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void CreateTransactionItem_WithInvalidDescription_ShouldThrowValidationException(string? description)
    {
        // Arrange
        var transaction = new Transaction(Currency.Usd);
        
        // Act
        Action act = () => transaction.AddItem(description!, 1, 10);
        
        // Assert
        act.Should().Throw<ValidationException>()
            .Which.Errors.Should().ContainSingle(e =>
                e.PropertyName == "description" &&
                e.ErrorMessage == "Item description is required.");
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(-100)]
    public void CreateTransactionItem_WithInvalidQuantity_ShouldThrowValidationException(int quantity)
    {
        // Arrange
        var transaction = new Transaction(Currency.Usd);
        
        // Act
        Action act = () => transaction.AddItem("Test Item", quantity, 10);
        
        // Assert
        act.Should().Throw<ValidationException>()
            .Which.Errors.Should().ContainSingle(e =>
                e.PropertyName == "quantity" &&
                e.ErrorMessage == "Quantity must be positive.");
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(-0.01)]
    [InlineData(-100.50)]
    public void CreateTransactionItem_WithNegativeUnitPrice_ShouldThrowValidationException(decimal unitPrice)
    {
        // Arrange
        var transaction = new Transaction(Currency.Usd);
        
        // Act
        Action act = () => transaction.AddItem("Test Item", 1, unitPrice);
        
        // Assert
        act.Should().Throw<ValidationException>()
            .Which.Errors.Should().ContainSingle(e =>
                e.PropertyName == "unitPrice" &&
                e.ErrorMessage == "Unit price cannot be negative.");
    }

    [Fact]
    public void CreateTransactionItem_WithZeroUnitPrice_ShouldSucceed()
    {
        // Arrange
        var transaction = new Transaction(Currency.Usd);
        
        // Act
        transaction.AddItem("Free Item", 1, 0);
        
        // Assert
        var item = transaction.Items.Single();
        item.UnitPrice.Should().Be(0);
        item.LineTotal.Should().Be(0);
    }

    [Fact]
    public void CreateTransactionItem_WithMultipleValidationErrors_ShouldThrowWithAllErrors()
    {
        // Arrange
        var transaction = new Transaction(Currency.Usd);
        
        // Act
        Action act = () => transaction.AddItem("", -1, -10);
        // Assert
        var exception = act.Should().Throw<ValidationException>().Which;
        exception.Errors.Should().HaveCount(3);
        exception.Errors.Should().Contain(e => e.PropertyName == "description");
        exception.Errors.Should().Contain(e => e.PropertyName == "quantity");
        exception.Errors.Should().Contain(e => e.PropertyName == "unitPrice");
    }

    [Fact]
    public void LineTotal_ShouldCalculateCorrectly()
    {
        // Arrange
        var transaction = new Transaction(Currency.Usd);
        
        // Act
        transaction.AddItem("Item", 5, 20.50m);
        
        // Assert
        var item = transaction.Items.Single();
        item.LineTotal.Should().Be(102.50m);
    }
}
