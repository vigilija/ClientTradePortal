using Refit;

namespace ClientTradePortal.Tests.Services.Account;

public class AccountServiceTests
{
    private readonly Mock<IAccountApiClient> _mockApiClient;
    private readonly Mock<ILogger<AccountService>> _mockLogger;
    private readonly IMemoryCache _cache;
    private readonly AccountService _service;

    public AccountServiceTests()
    {
        _mockApiClient = new Mock<IAccountApiClient>();
        _mockLogger = new Mock<ILogger<AccountService>>();
        _cache = new MemoryCache(new MemoryCacheOptions());
        _service = new AccountService(_mockApiClient.Object, _mockLogger.Object, _cache);
    }

    #region GetAccountAsync Tests

    [Fact]
    public async Task GetAccountAsync_WhenCacheHit_ReturnsCachedAccount()
    {
        // Arrange
        var accountId = Guid.NewGuid();
        var cachedAccount = new AccountResponse
        {
            AccountId = accountId,
            ClientId = Guid.NewGuid(),
            CashBalance = 10000m,
            Currency = "EUR"
        };
        _cache.Set($"account_{accountId}", cachedAccount, TimeSpan.FromMinutes(5));

        // Act
        var result = await _service.GetAccountAsync(accountId);

        // Assert
        result.Should().NotBeNull();
        result.AccountId.Should().Be(accountId);
        result.CashBalance.Should().Be(10000m);
        _mockApiClient.Verify(x => x.GetAccountAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task GetAccountAsync_WhenCacheMiss_FetchesFromApiAndCaches()
    {
        // Arrange
        var accountId = Guid.NewGuid();
        var accountData = new AccountResponse
        {
            AccountId = accountId,
            ClientId = Guid.NewGuid(),
            CashBalance = 15000m,
            Currency = "EUR"
        };
        var apiResponse = new ApiAccountResponse<AccountResponse>
        {
            Success = true,
            Data = accountData
        };
        _mockApiClient.Setup(x => x.GetAccountAsync(accountId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(apiResponse);

        // Act
        var result = await _service.GetAccountAsync(accountId);

        // Assert
        result.Should().NotBeNull();
        result.AccountId.Should().Be(accountId);
        result.CashBalance.Should().Be(15000m);
        _mockApiClient.Verify(x => x.GetAccountAsync(accountId, It.IsAny<CancellationToken>()), Times.Once);

        // Verify caching
        var cachedValue = _cache.Get<AccountResponse>($"account_{accountId}");
        cachedValue.Should().NotBeNull();
        cachedValue!.AccountId.Should().Be(accountId);
    }

    [Fact]
    public async Task GetAccountAsync_WhenApiReturnsSuccessFalse_ThrowsApplicationException()
    {
        // Arrange
        var accountId = Guid.NewGuid();
        var apiResponse = new ApiAccountResponse<AccountResponse>
        {
            Success = false,
            Data = null,
            Message = "Account not found",
            Errors = new List<string> { "404" }
        };
        _mockApiClient.Setup(x => x.GetAccountAsync(accountId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(apiResponse);

        // Act
        Func<Task> act = async () => await _service.GetAccountAsync(accountId);

        // Assert
        await act.Should().ThrowAsync<ApplicationException>()
            .WithMessage("Failed to retrieve account*");
    }

    [Fact]
    public async Task GetAccountAsync_WhenApiReturnsNullData_ThrowsApplicationException()
    {
        // Arrange
        var accountId = Guid.NewGuid();
        var apiResponse = new ApiAccountResponse<AccountResponse>
        {
            Success = true,
            Data = null
        };
        _mockApiClient.Setup(x => x.GetAccountAsync(accountId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(apiResponse);

        // Act
        Func<Task> act = async () => await _service.GetAccountAsync(accountId);

        // Assert
        await act.Should().ThrowAsync<ApplicationException>()
            .WithMessage("Failed to retrieve account*");
    }

    [Fact]
    public async Task GetAccountAsync_WhenApiThrowsException_ThrowsApplicationException()
    {
        // Arrange
        var accountId = Guid.NewGuid();
        var apiException = await ApiException.Create(
            new HttpRequestMessage(),
            HttpMethod.Get,
            new HttpResponseMessage(System.Net.HttpStatusCode.InternalServerError),
            new RefitSettings());

        _mockApiClient.Setup(x => x.GetAccountAsync(accountId, It.IsAny<CancellationToken>()))
            .ThrowsAsync(apiException);

        // Act
        Func<Task> act = async () => await _service.GetAccountAsync(accountId);

        // Assert
        await act.Should().ThrowAsync<ApplicationException>()
            .WithMessage("Failed to fetch account");
    }

    #endregion

    #region GetBalanceAsync Tests

    [Fact]
    public async Task GetBalanceAsync_WhenSuccessful_ReturnsBalance()
    {
        // Arrange
        var accountId = Guid.NewGuid();
        var balanceData = new AccountBalanceResponse
        {
            AccountId = accountId,
            ClientId = Guid.NewGuid(),
            CashBalance = 25000m,
            Currency = "EUR",
            Positions = new List<StockPositionResponse>
            {
                new StockPositionResponse
                {
                    Symbol = "AAPL",
                    Quantity = 10,
                    AveragePrice = 150m,
                    CurrentPrice = 175m
                }
            }
        };
        var apiResponse = new ApiAccountResponse<AccountBalanceResponse>
        {
            Success = true,
            Data = balanceData
        };
        _mockApiClient.Setup(x => x.GetBalanceAsync(accountId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(apiResponse);

        // Act
        var result = await _service.GetBalanceAsync(accountId);

        // Assert
        result.Should().NotBeNull();
        result.AccountId.Should().Be(accountId);
        result.CashBalance.Should().Be(25000m);
        result.Positions.Should().HaveCount(1);
        result.Positions[0].Symbol.Should().Be("AAPL");
    }

    [Fact]
    public async Task GetBalanceAsync_WhenApiReturnsSuccessFalse_ThrowsApplicationException()
    {
        // Arrange
        var accountId = Guid.NewGuid();
        var apiResponse = new ApiAccountResponse<AccountBalanceResponse>
        {
            Success = false,
            Data = null,
            Message = "Balance not available",
            Errors = new List<string> { "500" }
        };
        _mockApiClient.Setup(x => x.GetBalanceAsync(accountId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(apiResponse);

        // Act
        Func<Task> act = async () => await _service.GetBalanceAsync(accountId);

        // Assert
        await act.Should().ThrowAsync<ApplicationException>()
            .WithMessage("Failed to retrieve balance*");
    }

    [Fact]
    public async Task GetBalanceAsync_WhenApiReturnsNullData_ThrowsApplicationException()
    {
        // Arrange
        var accountId = Guid.NewGuid();
        var apiResponse = new ApiAccountResponse<AccountBalanceResponse>
        {
            Success = true,
            Data = null
        };
        _mockApiClient.Setup(x => x.GetBalanceAsync(accountId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(apiResponse);

        // Act
        Func<Task> act = async () => await _service.GetBalanceAsync(accountId);

        // Assert
        await act.Should().ThrowAsync<ApplicationException>()
            .WithMessage("Failed to retrieve balance*");
    }

    [Fact]
    public async Task GetBalanceAsync_WhenApiThrowsException_ThrowsApplicationException()
    {
        // Arrange
        var accountId = Guid.NewGuid();
        var apiException = await ApiException.Create(
            new HttpRequestMessage(),
            HttpMethod.Get,
            new HttpResponseMessage(System.Net.HttpStatusCode.ServiceUnavailable),
            new RefitSettings());

        _mockApiClient.Setup(x => x.GetBalanceAsync(accountId, It.IsAny<CancellationToken>()))
            .ThrowsAsync(apiException);

        // Act
        Func<Task> act = async () => await _service.GetBalanceAsync(accountId);

        // Assert
        await act.Should().ThrowAsync<ApplicationException>()
            .WithMessage("Failed to fetch balance");
    }

    #endregion

    #region GetPositionsAsync Tests

    [Fact]
    public async Task GetPositionsAsync_WhenSuccessful_ReturnsPositions()
    {
        // Arrange
        var accountId = Guid.NewGuid();
        var positions = new List<StockPositionResponse>
        {
            new StockPositionResponse
            {
                Symbol = "AAPL",
                Quantity = 10,
                AveragePrice = 150m,
                CurrentPrice = 175m
            },
            new StockPositionResponse
            {
                Symbol = "MSFT",
                Quantity = 5,
                AveragePrice = 300m,
                CurrentPrice = 350m
            }
        };
        var apiResponse = new ApiAccountResponse<List<StockPositionResponse>>
        {
            Success = true,
            Data = positions
        };
        _mockApiClient.Setup(x => x.GetPositionsAsync(accountId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(apiResponse);

        // Act
        var result = await _service.GetPositionsAsync(accountId);

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(2);
        result[0].Symbol.Should().Be("AAPL");
        result[0].Quantity.Should().Be(10);
        result[1].Symbol.Should().Be("MSFT");
        result[1].Quantity.Should().Be(5);
    }

    [Fact]
    public async Task GetPositionsAsync_WhenApiReturnsSuccessFalse_ThrowsApplicationException()
    {
        // Arrange
        var accountId = Guid.NewGuid();
        var apiResponse = new ApiAccountResponse<List<StockPositionResponse>>
        {
            Success = false,
            Data = null,
            Message = "Positions not available",
            Errors = new List<string> { "503" }
        };
        _mockApiClient.Setup(x => x.GetPositionsAsync(accountId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(apiResponse);

        // Act
        Func<Task> act = async () => await _service.GetPositionsAsync(accountId);

        // Assert
        await act.Should().ThrowAsync<ApplicationException>()
            .WithMessage("Failed to retrieve positions*");
    }

    [Fact]
    public async Task GetPositionsAsync_WhenApiReturnsNullData_ThrowsApplicationException()
    {
        // Arrange
        var accountId = Guid.NewGuid();
        var apiResponse = new ApiAccountResponse<List<StockPositionResponse>>
        {
            Success = true,
            Data = null
        };
        _mockApiClient.Setup(x => x.GetPositionsAsync(accountId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(apiResponse);

        // Act
        Func<Task> act = async () => await _service.GetPositionsAsync(accountId);

        // Assert
        await act.Should().ThrowAsync<ApplicationException>()
            .WithMessage("Failed to retrieve positions*");
    }

    [Fact]
    public async Task GetPositionsAsync_WhenApiThrowsException_ThrowsApplicationException()
    {
        // Arrange
        var accountId = Guid.NewGuid();
        var apiException = await ApiException.Create(
            new HttpRequestMessage(),
            HttpMethod.Get,
            new HttpResponseMessage(System.Net.HttpStatusCode.BadGateway),
            new RefitSettings());

        _mockApiClient.Setup(x => x.GetPositionsAsync(accountId, It.IsAny<CancellationToken>()))
            .ThrowsAsync(apiException);

        // Act
        Func<Task> act = async () => await _service.GetPositionsAsync(accountId);

        // Assert
        await act.Should().ThrowAsync<ApplicationException>()
            .WithMessage("Failed to fetch positions");
    }

    #endregion
}
