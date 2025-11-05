using ClientTradePortal.Services.Http;
using Refit;

namespace ClientTradePortal.Tests.Services.Trading;

public class TradingServiceTests
{
    private readonly Mock<ITradingApiClient> _mockTradingApiClient;
    private readonly Mock<IValidationApiClient> _mockValidationApiClient;
    private readonly Mock<ILogger<TradingService>> _mockLogger;
    private readonly IMemoryCache _cache;
    private readonly TradingService _service;

    public TradingServiceTests()
    {
        _mockTradingApiClient = new Mock<ITradingApiClient>();
        _mockValidationApiClient = new Mock<IValidationApiClient>();
        _mockLogger = new Mock<ILogger<TradingService>>();
        _cache = new MemoryCache(new MemoryCacheOptions());
        _service = new TradingService(
            _mockTradingApiClient.Object,
            _mockValidationApiClient.Object,
            _mockLogger.Object,
            _cache);
    }

    #region GetStockPriceAsync Tests

    [Fact]
    public async Task GetStockPriceAsync_WhenCacheHit_ReturnsCachedPrice()
    {
        // Arrange
        var symbol = "AAPL";
        var cachedPrice = new StockQuoteResponse { Symbol = symbol, Price = 150.00m };
        _cache.Set($"stock_price_{symbol}", cachedPrice, TimeSpan.FromSeconds(30));

        // Act
        var result = await _service.GetStockPriceAsync(symbol);

        // Assert
        result.Should().NotBeNull();
        result.Symbol.Should().Be(symbol);
        result.Price.Should().Be(150.00m);
        _mockTradingApiClient.Verify(x => x.GetQuoteAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task GetStockPriceAsync_WhenCacheMiss_FetchesFromApiAndCaches()
    {
        // Arrange
        var symbol = "AAPL";
        var stockQuote = new StockQuoteResponse { Symbol = symbol, Price = 175.50m };
        var apiResponse = new ApiTradingResponse<StockQuoteResponse>
        {
            Success = true,
            Data = stockQuote
        };
        _mockTradingApiClient.Setup(x => x.GetQuoteAsync(symbol, It.IsAny<CancellationToken>()))
            .ReturnsAsync(apiResponse);

        // Act
        var result = await _service.GetStockPriceAsync(symbol);

        // Assert
        result.Should().NotBeNull();
        result.Symbol.Should().Be(symbol);
        result.Price.Should().Be(175.50m);
        _mockTradingApiClient.Verify(x => x.GetQuoteAsync(symbol, It.IsAny<CancellationToken>()), Times.Once);

        // Verify caching
        var cachedValue = _cache.Get<StockQuoteResponse>($"stock_price_{symbol}");
        cachedValue.Should().NotBeNull();
        cachedValue!.Price.Should().Be(175.50m);
    }

    [Fact]
    public async Task GetStockPriceAsync_WhenApiReturnsSuccessFalse_ThrowsApplicationException()
    {
        // Arrange
        var symbol = "AAPL";
        var apiResponse = new ApiTradingResponse<StockQuoteResponse>
        {
            Success = false,
            Data = null,
            Message = "Stock not found"
        };
        _mockTradingApiClient.Setup(x => x.GetQuoteAsync(symbol, It.IsAny<CancellationToken>()))
            .ReturnsAsync(apiResponse);

        // Act
        Func<Task> act = async () => await _service.GetStockPriceAsync(symbol);

        // Assert
        await act.Should().ThrowAsync<ApplicationException>()
            .WithMessage("Failed to retrieve stock price");
    }

    [Fact]
    public async Task GetStockPriceAsync_WhenApiReturnsNullData_ThrowsApplicationException()
    {
        // Arrange
        var symbol = "AAPL";
        var apiResponse = new ApiTradingResponse<StockQuoteResponse>
        {
            Success = true,
            Data = null
        };
        _mockTradingApiClient.Setup(x => x.GetQuoteAsync(symbol, It.IsAny<CancellationToken>()))
            .ReturnsAsync(apiResponse);

        // Act
        Func<Task> act = async () => await _service.GetStockPriceAsync(symbol);

        // Assert
        await act.Should().ThrowAsync<ApplicationException>()
            .WithMessage("Failed to retrieve stock price");
    }

    [Fact]
    public async Task GetStockPriceAsync_WhenApiThrowsException_ThrowsApplicationException()
    {
        // Arrange
        var symbol = "AAPL";
        var apiException = await ApiException.Create(
            new HttpRequestMessage(),
            HttpMethod.Get,
            new HttpResponseMessage(System.Net.HttpStatusCode.InternalServerError),
            new RefitSettings());

        _mockTradingApiClient.Setup(x => x.GetQuoteAsync(symbol, It.IsAny<CancellationToken>()))
            .ThrowsAsync(apiException);

        // Act
        Func<Task> act = async () => await _service.GetStockPriceAsync(symbol);

        // Assert
        await act.Should().ThrowAsync<ApplicationException>()
            .WithMessage("Failed to fetch stock price");
    }

    #endregion

    #region PlaceOrderAsync Tests

    [Fact]
    public async Task PlaceOrderAsync_WhenSuccessful_ReturnsOrderResponse()
    {
        // Arrange
        var orderRequest = new OrderRequest
        {
            AccountId = Guid.NewGuid(),
            Symbol = "AAPL",
            Quantity = 10
        };
        var orderResponse = new OrderResponse
        {
            OrderId = Guid.NewGuid(),
            AccountId = orderRequest.AccountId,
            Symbol = orderRequest.Symbol,
            Quantity = orderRequest.Quantity,
            PricePerShare = 150.00m,
            TotalAmount = 1500.00m,
            Status = "Completed"
        };
        var apiResponse = new ApiTradingResponse<OrderResponse>
        {
            Success = true,
            Data = orderResponse
        };
        _mockTradingApiClient.Setup(x => x.PlaceOrderAsync(orderRequest, It.IsAny<CancellationToken>()))
            .ReturnsAsync(apiResponse);

        // Act
        var result = await _service.PlaceOrderAsync(orderRequest);

        // Assert
        result.Should().NotBeNull();
        result.OrderId.Should().Be(orderResponse.OrderId);
        result.Symbol.Should().Be("AAPL");
        result.Quantity.Should().Be(10);
        result.TotalAmount.Should().Be(1500.00m);
    }

    [Fact]
    public async Task PlaceOrderAsync_WhenApiReturnsSuccessFalse_ThrowsApplicationException()
    {
        // Arrange
        var orderRequest = new OrderRequest
        {
            AccountId = Guid.NewGuid(),
            Symbol = "AAPL",
            Quantity = 10
        };
        var apiResponse = new ApiTradingResponse<OrderResponse>
        {
            Success = false,
            Data = null,
            Message = "Insufficient funds"
        };
        _mockTradingApiClient.Setup(x => x.PlaceOrderAsync(orderRequest, It.IsAny<CancellationToken>()))
            .ReturnsAsync(apiResponse);

        // Act
        Func<Task> act = async () => await _service.PlaceOrderAsync(orderRequest);

        // Assert
        await act.Should().ThrowAsync<ApplicationException>()
            .WithMessage("Failed to place order");
    }

    [Fact]
    public async Task PlaceOrderAsync_WhenApiReturnsNullData_ThrowsApplicationException()
    {
        // Arrange
        var orderRequest = new OrderRequest
        {
            AccountId = Guid.NewGuid(),
            Symbol = "AAPL",
            Quantity = 10
        };
        var apiResponse = new ApiTradingResponse<OrderResponse>
        {
            Success = true,
            Data = null
        };
        _mockTradingApiClient.Setup(x => x.PlaceOrderAsync(orderRequest, It.IsAny<CancellationToken>()))
            .ReturnsAsync(apiResponse);

        // Act
        Func<Task> act = async () => await _service.PlaceOrderAsync(orderRequest);

        // Assert
        await act.Should().ThrowAsync<ApplicationException>()
            .WithMessage("Failed to place order");
    }

    [Fact]
    public async Task PlaceOrderAsync_WhenApiThrowsException_ThrowsApplicationException()
    {
        // Arrange
        var orderRequest = new OrderRequest
        {
            AccountId = Guid.NewGuid(),
            Symbol = "AAPL",
            Quantity = 10
        };
        var apiException = await ApiException.Create(
            new HttpRequestMessage(),
            HttpMethod.Post,
            new HttpResponseMessage(System.Net.HttpStatusCode.BadRequest),
            new RefitSettings());

        _mockTradingApiClient.Setup(x => x.PlaceOrderAsync(orderRequest, It.IsAny<CancellationToken>()))
            .ThrowsAsync(apiException);

        // Act
        Func<Task> act = async () => await _service.PlaceOrderAsync(orderRequest);

        // Assert
        await act.Should().ThrowAsync<ApplicationException>()
            .WithMessage("Failed to place order");
    }

    #endregion

    #region ValidateOrderAsync Tests

    [Fact]
    public async Task ValidateOrderAsync_WhenSuccessful_ReturnsValidationResponse()
    {
        // Arrange
        var validationRequest = new ValidationRequest
        {
            AccountId = Guid.NewGuid(),
            Symbol = "AAPL",
            Quantity = 10
        };
        var validationResponse = new ValidationResponse
        {
            IsValid = true,
            CurrentPrice = 150.00m,
            TotalAmount = 1500.00m
        };
        var apiResponse = new ApiResponse<ValidationResponse>(
            new HttpResponseMessage(System.Net.HttpStatusCode.OK),
            validationResponse,
            new RefitSettings());

        _mockValidationApiClient.Setup(x => x.ValidateOrderAsync(validationRequest, It.IsAny<CancellationToken>()))
            .ReturnsAsync(apiResponse);

        // Act
        var result = await _service.ValidateOrderAsync(validationRequest);

        // Assert
        result.Should().NotBeNull();
        result.IsValid.Should().BeTrue();
        result.CurrentPrice.Should().Be(150.00m);
        result.TotalAmount.Should().Be(1500.00m);
    }

    [Fact]
    public async Task ValidateOrderAsync_WhenApiReturnsNullContent_ReturnsDefaultErrorResponse()
    {
        // Arrange
        var validationRequest = new ValidationRequest
        {
            AccountId = Guid.NewGuid(),
            Symbol = "AAPL",
            Quantity = 10
        };
        var apiResponse = new ApiResponse<ValidationResponse>(
            new HttpResponseMessage(System.Net.HttpStatusCode.OK),
            null,
            new RefitSettings());

        _mockValidationApiClient.Setup(x => x.ValidateOrderAsync(validationRequest, It.IsAny<CancellationToken>()))
            .ReturnsAsync(apiResponse);

        // Act
        var result = await _service.ValidateOrderAsync(validationRequest);

        // Assert
        result.Should().NotBeNull();
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain("Validation failed");
    }

    [Fact]
    public async Task ValidateOrderAsync_WhenApiThrowsException_ReturnsErrorResponse()
    {
        // Arrange
        var validationRequest = new ValidationRequest
        {
            AccountId = Guid.NewGuid(),
            Symbol = "AAPL",
            Quantity = 10
        };
        var apiException = await ApiException.Create(
            new HttpRequestMessage(),
            HttpMethod.Post,
            new HttpResponseMessage(System.Net.HttpStatusCode.ServiceUnavailable),
            new RefitSettings());

        _mockValidationApiClient.Setup(x => x.ValidateOrderAsync(validationRequest, It.IsAny<CancellationToken>()))
            .ThrowsAsync(apiException);

        // Act
        var result = await _service.ValidateOrderAsync(validationRequest);

        // Assert
        result.Should().NotBeNull();
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain("Validation service unavailable");
    }

    #endregion

    #region GetOrdersAsync Tests

    [Fact]
    public async Task GetOrdersAsync_WhenSuccessful_ReturnsOrdersList()
    {
        // Arrange
        var accountId = Guid.NewGuid();
        var orders = new List<OrderResponse>
        {
            new OrderResponse
            {
                OrderId = Guid.NewGuid(),
                AccountId = accountId,
                Symbol = "AAPL",
                Quantity = 10,
                Status = "Completed"
            },
            new OrderResponse
            {
                OrderId = Guid.NewGuid(),
                AccountId = accountId,
                Symbol = "AAPL",
                Quantity = 5,
                Status = "Completed"
            }
        };
        var apiResponse = new ApiTradingResponse<List<OrderResponse>>
        {
            Success = true,
            Data = orders
        };
        _mockTradingApiClient.Setup(x => x.GetOrdersAsync(accountId, 1, 10, It.IsAny<CancellationToken>()))
            .ReturnsAsync(apiResponse);

        // Act
        var result = await _service.GetOrdersAsync(accountId, 1, 10);

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(2);
        result[0].Symbol.Should().Be("AAPL");
        result[1].Quantity.Should().Be(5);
    }

    [Fact]
    public async Task GetOrdersAsync_WhenApiReturnsNullData_ReturnsEmptyList()
    {
        // Arrange
        var accountId = Guid.NewGuid();
        var apiResponse = new ApiTradingResponse<List<OrderResponse>>
        {
            Success = true,
            Data = null
        };
        _mockTradingApiClient.Setup(x => x.GetOrdersAsync(accountId, 1, 10, It.IsAny<CancellationToken>()))
            .ReturnsAsync(apiResponse);

        // Act
        var result = await _service.GetOrdersAsync(accountId, 1, 10);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeEmpty();
    }

    [Fact]
    public async Task GetOrdersAsync_WhenApiThrowsException_ReturnsEmptyList()
    {
        // Arrange
        var accountId = Guid.NewGuid();
        var apiException = await ApiException.Create(
            new HttpRequestMessage(),
            HttpMethod.Get,
            new HttpResponseMessage(System.Net.HttpStatusCode.InternalServerError),
            new RefitSettings());

        _mockTradingApiClient.Setup(x => x.GetOrdersAsync(accountId, 1, 10, It.IsAny<CancellationToken>()))
            .ThrowsAsync(apiException);

        // Act
        var result = await _service.GetOrdersAsync(accountId, 1, 10);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeEmpty();
    }

    #endregion
}
