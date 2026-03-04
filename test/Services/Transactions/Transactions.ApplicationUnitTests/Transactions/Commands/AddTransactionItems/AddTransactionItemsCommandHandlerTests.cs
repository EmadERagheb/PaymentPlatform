

using BuildingBlocks.ValueObjects;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MockQueryable;
using MockQueryable.NSubstitute;
using NSubstitute;
using Transactions.Application.Data;
using Transactions.Application.Transactions.Commands.AddTransactionItems;
using Transactions.Domain.Aggregates;
using Transactions.Domain.ValueObjects;

namespace Transactions.ApplicationUnitTests.Transactions.Commands.AddTransactionItems;

public class AddTransactionItemsCommandHandlerTests
{
    private readonly ITransactionDbContext _transactionDbContextMock;
    private readonly ILogger<AddTransactionItemsCommandHandler> _loggerMock;
    private readonly AddTransactionItemsCommandHandler _handler;

    public AddTransactionItemsCommandHandlerTests()
    {
        _transactionDbContextMock = Substitute.For<ITransactionDbContext>();
        _loggerMock = Substitute.For<ILogger<AddTransactionItemsCommandHandler>>();
        _handler = new AddTransactionItemsCommandHandler(_transactionDbContextMock, _loggerMock);
    }

    [Fact]
    public async Task Handle_ShouldReturnFailure_WhenTransactionNotFound()
    {
        // Arrange
        var transactionId = Guid.NewGuid();
        var command = new AddTransactionItemsCommand(
            transactionId,
            [new AddTransactionItemDto("Item 1", 2, 10.50m)]);

        var mockDbSet = new List<Transaction>().BuildMockDbSet();
        _transactionDbContextMock.Transactions.Returns(mockDbSet);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("TransactionNotFound");
        result.Error.Name.Should().Be("Transaction not found.");
    }

    [Fact]
    public async Task Handle_ShouldAddItemsAndReturnSuccess_WhenTransactionExists()
    {
        // Arrange
        var transaction = new Transaction(Currency.Usd);
        var transactionId = transaction.Id.Value;
        var command = new AddTransactionItemsCommand(
            transactionId,
            [
                new AddTransactionItemDto("Item 1", 2, 10.00m),
                new AddTransactionItemDto("Item 2", 1, 25.00m)
            ]);

        var mockDbSet = new List<Transaction> { transaction }.BuildMockDbSet();
        _transactionDbContextMock.Transactions.Returns(mockDbSet);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        transaction.Items.Should().HaveCount(2);
        transaction.TotalAmount.Amount.Should().Be(45.00m); // (2*10) + (1*25)
        await _transactionDbContextMock.TransactionItems.Received(1).AddRangeAsync(Arg.Any<IEnumerable<TransactionItem>>());
        _transactionDbContextMock.Transactions.Received(1).Update(transaction);
        await _transactionDbContextMock.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_ShouldCalculateTotalCorrectly_WhenMultipleItemsAdded()
    {
        // Arrange
        var transaction = new Transaction(Currency.Usd);
        var transactionId = transaction.Id.Value;
        var command = new AddTransactionItemsCommand(
            transactionId,
            [
                new AddTransactionItemDto("Item 1", 3, 10.00m),  // 30.00
                new AddTransactionItemDto("Item 2", 2, 15.50m),  // 31.00
                new AddTransactionItemDto("Item 3", 1, 100.00m)  // 100.00
            ]);

        var mockDbSet = new List<Transaction> { transaction }.BuildMockDbSet();
        _transactionDbContextMock.Transactions.Returns(mockDbSet);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        transaction.Items.Should().HaveCount(3);
        transaction.TotalAmount.Amount.Should().Be(161.00m);
    }

    [Fact]
    public async Task Handle_ShouldNotCallSaveChanges_WhenTransactionNotFound()
    {
        // Arrange
        var command = new AddTransactionItemsCommand(
            Guid.NewGuid(),
            [new AddTransactionItemDto("Item 1", 2, 10.50m)]);

        var mockDbSet = new List<Transaction>().BuildMockDbSet();
        _transactionDbContextMock.Transactions.Returns(mockDbSet);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        await _transactionDbContextMock.TransactionItems.DidNotReceive().AddRangeAsync(Arg.Any<IEnumerable<TransactionItem>>());
        _transactionDbContextMock.Transactions.DidNotReceive().Update(Arg.Any<Transaction>());
        await _transactionDbContextMock.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
    }
}
