using FluentAssertions;
using Transactions.Application.Transactions.Commands.CreateTransaction;
using Transactions.IntegrationTests.Fixtures;
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
        var response = await sender.Send(request);
        // Assert
        response.Should().NotBeNull();
        response.IsFailure.Should().BeTrue();
        response.Error.Should().NotBeNull();
        response.Error.Code.Should().Be("InvalidCurrency");
    }


}