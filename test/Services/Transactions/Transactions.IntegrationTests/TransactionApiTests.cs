using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Transactions.IntegrationTests.Fixtures;

namespace Transactions.IntegrationTests;

public class TransactionApiTests : IClassFixture<TransactionsApiFactory>
{
    private readonly HttpClient _client;

    public TransactionApiTests(TransactionsApiFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task CreateTransaction_ShouldReturnSuccess_WhenValidCurrency()
    {


    }

    [Fact]
    public async Task CreateTransaction_ShouldReturnBadRequest_WhenInvalidCurrency()
    {
        // Arrange
        var request = new { Currency = "INVALID" };

        // Act
        var response = await _client.PostAsJsonAsync("/api/transactions", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
}
