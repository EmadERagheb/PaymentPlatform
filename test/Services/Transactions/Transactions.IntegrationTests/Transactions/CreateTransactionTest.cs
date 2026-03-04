using FluentAssertions;
using Transactions.Application.Transactions.Commands.CreateTransaction;
using Transactions.IntegrationTests.Fixtures;
using BuildingBlocks.Exceptions;

namespace Transactions.IntegrationTests.Transactions;

public class CreateTransactionTest : BaseIntegrationTest
{
    public CreateTransactionTest(TransactionsApiFactory factory) : base(factory)
    {
    }

    [Fact]
    public async Task CreateTransaction_ShouldReturnSuccess_WhenValidCurrency()
    {
        // Arrange
        var request = new CreateTransactionCommand("USD");

        // Act
        var response = await sender.Send(request);

        // Assert
        response.Should().NotBeNull();
        response.IsSuccess.Should().BeTrue();
        response.Value.Should().NotBeNull();
    }

    [Fact]
    public async Task CreateTransaction_ShouldReturnFailure_WhenInvalidCurrency()
    {
        // Arrange
        var request = new CreateTransactionCommand("INVALID");

        // Act
        Func<Task> act = async () => await sender.Send(request);

        // Assert
        var exception = await act.Should().ThrowAsync<ValidationException>();
        exception.Which.Errors.Should().NotBeEmpty();
    }


}