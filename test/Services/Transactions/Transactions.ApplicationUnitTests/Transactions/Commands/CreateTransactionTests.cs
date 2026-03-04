
using FluentAssertions;
using NSubstitute;
using Transactions.Application.Data;
using Transactions.Application.Transactions.Commands.CreateTransaction;
using Transactions.Domain.Aggregates;

namespace Transactions.ApplicationUnitTests.Transactions.Commands;

public class CreateTransactionTests
{
    public static readonly CreateTransactionCommand CreateTransactionCommand = new("USD");
    private readonly CreateTransactionCommandHandler _handler;
    private readonly ITransactionDbContext _transactionDbContextMock;
    private readonly CreateTransactionCommandValidator _validator;
    public CreateTransactionTests()
    {
        _transactionDbContextMock = Substitute.For<ITransactionDbContext>();
        _handler = new CreateTransactionCommandHandler(_transactionDbContextMock);
        _validator = new CreateTransactionCommandValidator();
    }

    [Fact]
    public async Task Handle_ShouldCreateTransaction()
    {
        // Arrange
        var command = CreateTransactionCommand;
        // Act
        var result = await _handler.Handle(command, CancellationToken.None);
        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal("USD", result.Value.Currency);
        await _transactionDbContextMock.Transactions.Received(1).AddAsync(Arg.Any<Transaction>(), Arg.Any<CancellationToken>());
        await _transactionDbContextMock.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());

    }
    [Fact]
    public async Task Validate_ShouldReturnFailure_WhenCurrencyIsInvalid()
    {
        // Arrange
        var command = new CreateTransactionCommand("INVALID");
        // Act
        var result = await _validator.ValidateAsync(command);
        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().ContainSingle(e => e.ErrorMessage == "Currency is not valid.");
    }

    [Fact]
    public async Task Validate_ShouldReturnFailure_WhenCurrencyIsEmpty()
    {
        // Arrange
        var command = new CreateTransactionCommand("");
        // Act
        var result = await _validator.ValidateAsync(command);
        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.ErrorMessage == "Currency is required.");
    }

    [Fact]
    public async Task Validate_ShouldReturnFailure_WhenCurrencyLengthIsInvalid()
    {
        // Arrange
        var command = new CreateTransactionCommand("US");
        // Act
        var result = await _validator.ValidateAsync(command);
        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.ErrorMessage == "Currency must be exactly 3 characters long.");
    }

    [Fact]
    public async Task Validate_ShouldReturnSuccess_WhenCurrencyIsValid()
    {
        // Arrange
        var command = new CreateTransactionCommand("USD");
        // Act
        var result = await _validator.ValidateAsync(command);
        // Assert
        result.IsValid.Should().BeTrue();
    }
}