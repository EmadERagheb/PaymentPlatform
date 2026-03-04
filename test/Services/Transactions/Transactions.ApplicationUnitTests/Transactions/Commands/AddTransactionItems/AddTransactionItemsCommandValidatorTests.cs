using FluentAssertions;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Transactions.Application.Data;
using Transactions.Application.Transactions.Commands.AddTransactionItems;


namespace Transactions.ApplicationUnitTests.Transactions.Commands.AddTransactionItems;

public class AddTransactionItemsCommandValidatorTests
{
    private readonly ITransactionDbContext _transactionDbContextMock;

    private readonly AddTransactionItemsCommandValidator _validator;

    public AddTransactionItemsCommandValidatorTests()
    {
        _transactionDbContextMock = Substitute.For<ITransactionDbContext>();
        _validator = new AddTransactionItemsCommandValidator();
    }

    [Fact]
    public async Task Validate_ShouldReturnSuccess_WhenCommandIsValid()
    {
        // Arrange
        var command = new AddTransactionItemsCommand(
            Guid.NewGuid(),
            [new AddTransactionItemDto("Item 1", 2, 10.50m)]);

        // Act
        var result = await _validator.ValidateAsync(command);

        // Assert
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public async Task Validate_ShouldReturnFailure_WhenTransactionIdIsEmpty()
    {
        // Arrange
        var command = new AddTransactionItemsCommand(
            Guid.Empty,
            [new AddTransactionItemDto("Item 1", 2, 10.50m)]);

        // Act
        var result = await _validator.ValidateAsync(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().ContainSingle(e => e.ErrorMessage == "Transaction ID is required.");
    }

    [Fact]
    public async Task Validate_ShouldReturnFailure_WhenItemsIsEmpty()
    {
        // Arrange
        var command = new AddTransactionItemsCommand(
            Guid.NewGuid(),
            []);

        // Act
        var result = await _validator.ValidateAsync(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.ErrorMessage == "At least one item is required.");
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(-100)]
    public async Task Validate_ShouldReturnFailure_WhenItemHasInvalidUnitPrice(decimal unitPrice)
    {
        // Arrange
        var command = new AddTransactionItemsCommand(
            Guid.NewGuid(),
            [new AddTransactionItemDto("Item 1", 2, unitPrice)]);

        // Act
        var result = await _validator.ValidateAsync(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.ErrorMessage == "All items must have a valid unit price.");
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(-50)]
    public async Task Validate_ShouldReturnFailure_WhenItemHasInvalidQuantity(int quantity)
    {
        // Arrange
        var command = new AddTransactionItemsCommand(
            Guid.NewGuid(),
            [new AddTransactionItemDto("Item 1", quantity, 10.50m)]);

        // Act
        var result = await _validator.ValidateAsync(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.ErrorMessage == "All items must have a valid quantity.");
    }

    [Fact]
    public async Task Validate_ShouldReturnFailure_WhenAnyItemHasInvalidUnitPrice()
    {
        // Arrange
        var command = new AddTransactionItemsCommand(
            Guid.NewGuid(),
            [
                new AddTransactionItemDto("Item 1", 2, 10.50m),
                new AddTransactionItemDto("Item 2", 1, -5m)
            ]);

        // Act
        var result = await _validator.ValidateAsync(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.ErrorMessage == "All items must have a valid unit price.");
    }

    [Fact]
    public async Task Validate_ShouldReturnFailure_WhenAnyItemHasInvalidQuantity()
    {
        // Arrange
        var command = new AddTransactionItemsCommand(
            Guid.NewGuid(),
            [
                new AddTransactionItemDto("Item 1", 2, 10.50m),
                new AddTransactionItemDto("Item 2", 0, 5m)
            ]);

        // Act
        var result = await _validator.ValidateAsync(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.ErrorMessage == "All items must have a valid quantity.");
    }

    [Fact]
    public async Task Validate_ShouldReturnMultipleErrors_WhenMultipleValidationsFail()
    {
        // Arrange
        var command = new AddTransactionItemsCommand(
            Guid.Empty,
            [new AddTransactionItemDto("Item 1", 0, -10m)]);

        // Act
        var result = await _validator.ValidateAsync(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().HaveCountGreaterThanOrEqualTo(3);
        result.Errors.Should().Contain(e => e.ErrorMessage == "Transaction ID is required.");
        result.Errors.Should().Contain(e => e.ErrorMessage == "All items must have a valid unit price.");
        result.Errors.Should().Contain(e => e.ErrorMessage == "All items must have a valid quantity.");
    }

    [Fact]
    public async Task Validate_ShouldReturnSuccess_WhenMultipleValidItemsProvided()
    {
        // Arrange
        var command = new AddTransactionItemsCommand(
            Guid.NewGuid(),
            [
                new AddTransactionItemDto("Item 1", 2, 10.50m),
                new AddTransactionItemDto("Item 2", 5, 25.00m),
                new AddTransactionItemDto("Item 3", 1, 100.00m)
            ]);

        // Act
        var result = await _validator.ValidateAsync(command);

        // Assert
        result.IsValid.Should().BeTrue();
    }
}
