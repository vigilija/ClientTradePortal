namespace ClientTradePortal.Tests.Services;

public class AccountServiceTests
{
    [Fact]
    public async Task GetAccountAsync_Should_Return_Account_And_Cache()
    {
        // arrange
        var apiMock = new Mock<IAccountApiClient>();
        var account = new AccountResponse { AccountId = Guid.NewGuid(), CashBalance = 1000m, Currency = "EUR" };
        apiMock.Setup(a => a.GetAccountAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ApiAccountResponse<AccountResponse> { Success = true, Data = account });

        var logger = new Mock<ILogger<AccountService>>().Object;
        var cache = new MemoryCache(new MemoryCacheOptions());

        var service = new AccountService(apiMock.Object, logger, cache);
        var accountId = account.AccountId;

        // act - first call should hit API
        var result1 = await service.GetAccountAsync(accountId);
        // act - second call should read from cache
        var result2 = await service.GetAccountAsync(accountId);

        // assert
        result1.AccountId.Should().Be(accountId);
        result2.AccountId.Should().Be(accountId);
        apiMock.Verify(a => a.GetAccountAsync(accountId, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GetBalanceAsync_Should_Return_Balance()
    {
        // arrange
        var apiMock = new Mock<IAccountApiClient>();
        var balance = new AccountBalanceResponse { CashBalance = 500m };
        apiMock.Setup(a => a.GetBalanceAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ApiAccountResponse<AccountBalanceResponse> { Success = true, Data = balance });

        var logger = new Mock<ILogger<AccountService>>().Object;
        var cache = new MemoryCache(new MemoryCacheOptions());

        var service = new AccountService(apiMock.Object, logger, cache);
        var accountId = Guid.NewGuid();

        // act
        var result = await service.GetBalanceAsync(accountId);

        // assert
        result.CashBalance.Should().Be(500m);
        apiMock.Verify(a => a.GetBalanceAsync(accountId, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GetPositionsAsync_Should_Return_Positions()
    {
        // arrange
        var apiMock = new Mock<IAccountApiClient>();
        var positions = new List<StockPositionResponse>
        {
            new StockPositionResponse { Symbol = "AAPL", Quantity = 10 }
        };

        apiMock.Setup(a => a.GetPositionsAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ApiAccountResponse<List<StockPositionResponse>> { Success = true, Data = positions });

        var logger = new Mock<ILogger<AccountService>>().Object;
        var cache = new MemoryCache(new MemoryCacheOptions());

        var service = new AccountService(apiMock.Object, logger, cache);
        var accountId = Guid.NewGuid();

        // act
        var result = await service.GetPositionsAsync(accountId);

        // assert
        result.Should().HaveCount(1);
        result[0].Symbol.Should().Be("AAPL");
        apiMock.Verify(a => a.GetPositionsAsync(accountId, It.IsAny<CancellationToken>()), Times.Once);
    }
}
