using Fluxor;
using ClientTradePortal.Services.Http;

namespace ClientTradePortal.Tests.Store.Trading;

public class TradingEffectsTests
{
    private readonly Mock<ITradingService> _mockTradingService;
    private readonly Mock<ILogger<TradingEffects>> _mockLogger;
    private readonly Mock<IState<TradingState>> _mockTradingState;
    private readonly Mock<IDispatcher> _mockDispatcher;
    private readonly TradingEffects _effects;

    public TradingEffectsTests()
    {
        _mockTradingService = new Mock<ITradingService>();
        _mockLogger = new Mock<ILogger<TradingEffects>>();
        _mockTradingState = new Mock<IState<TradingState>>();
        _mockDispatcher = new Mock<IDispatcher>();
        _effects = new TradingEffects(
            _mockTradingService.Object,
            _mockLogger.Object,
            _mockTradingState.Object);
    }

    #region HandleFetchStockPriceAction Tests

    [Fact]
    public async Task HandleFetchStockPriceAction_WhenSuccessful_DispatchesSuccessAction()
    {
        // Arrange
        var action = new FetchStockPriceAction("AAPL");
        var stockQuote = new StockQuoteResponse { Symbol = "AAPL", Price = 175.50m };
        _mockTradingService.Setup(x => x.GetStockPriceAsync("AAPL", It.IsAny<CancellationToken>()))
            .ReturnsAsync(stockQuote);

        // Act
        await _effects.HandleFetchStockPriceAction(action, _mockDispatcher.Object);

        // Assert
        _mockTradingService.Verify(x => x.GetStockPriceAsync("AAPL", It.IsAny<CancellationToken>()), Times.Once);
        _mockDispatcher.Verify(x => x.Dispatch(It.Is<FetchStockPriceSuccessAction>(
            a => a.Price == 175.50m)), Times.Once);
    }

    [Fact]
    public async Task HandleFetchStockPriceAction_WhenFails_DispatchesFailureAction()
    {
        // Arrange
        var action = new FetchStockPriceAction("AAPL");
        var exception = new ApplicationException("Failed to fetch stock price");
        _mockTradingService.Setup(x => x.GetStockPriceAsync("AAPL", It.IsAny<CancellationToken>()))
            .ThrowsAsync(exception);

        // Act
        await _effects.HandleFetchStockPriceAction(action, _mockDispatcher.Object);

        // Assert
        _mockDispatcher.Verify(x => x.Dispatch(It.Is<FetchStockPriceFailureAction>(
            a => a.ErrorMessage == "Failed to fetch stock price")), Times.Once);
    }

    #endregion

    #region HandleValidateOrderAction Tests

    [Fact]
    public async Task HandleValidateOrderAction_WhenSuccessful_DispatchesSuccessAction()
    {
        // Arrange
        var accountId = Guid.NewGuid();
        var action = new ValidateOrderAction(accountId);
        var tradingState = new TradingState
        {
            CurrentOrderRequest = new OrderRequest
            {
                AccountId = accountId,
                Symbol = "AAPL",
                Quantity = 10,
                IdempotencyKey = Guid.NewGuid()
            },
            CurrentStockPrice = 150m
        };
        var validationResponse = new ValidationResponse
        {
            IsValid = true,
            CurrentPrice = 150m,
            TotalAmount = 1500m
        };
        _mockTradingState.Setup(x => x.Value).Returns(tradingState);
        _mockTradingService.Setup(x => x.ValidateOrderAsync(It.IsAny<ValidationRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(validationResponse);

        // Act
        await _effects.HandleValidateOrderAction(action, _mockDispatcher.Object);

        // Assert
        _mockTradingService.Verify(x => x.ValidateOrderAsync(
            It.Is<ValidationRequest>(r =>
                r.AccountId == accountId &&
                r.Symbol == "AAPL" &&
                r.Quantity == 10 &&
                r.EstimatedPrice == 150m),
            It.IsAny<CancellationToken>()), Times.Once);
        _mockDispatcher.Verify(x => x.Dispatch(It.Is<ValidateOrderSuccessAction>(
            a => a.Result.IsValid && a.Result.TotalAmount == 1500m)), Times.Once);
    }

    [Fact]
    public async Task HandleValidateOrderAction_WhenFails_DispatchesFailureAction()
    {
        // Arrange
        var accountId = Guid.NewGuid();
        var action = new ValidateOrderAction(accountId);
        var tradingState = new TradingState
        {
            CurrentOrderRequest = new OrderRequest
            {
                AccountId = accountId,
                Symbol = "AAPL",
                Quantity = 10,
                IdempotencyKey = Guid.NewGuid()
            },
            CurrentStockPrice = 150m
        };
        var exception = new ApplicationException("Validation service unavailable");
        _mockTradingState.Setup(x => x.Value).Returns(tradingState);
        _mockTradingService.Setup(x => x.ValidateOrderAsync(It.IsAny<ValidationRequest>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(exception);

        // Act
        await _effects.HandleValidateOrderAction(action, _mockDispatcher.Object);

        // Assert
        _mockDispatcher.Verify(x => x.Dispatch(It.Is<ValidateOrderFailureAction>(
            a => a.ErrorMessage == "Validation service unavailable")), Times.Once);
    }

    [Fact]
    public async Task HandleValidateOrderAction_WhenPriceIsNull_UsesZeroAsEstimatedPrice()
    {
        // Arrange
        var accountId = Guid.NewGuid();
        var action = new ValidateOrderAction(accountId);
        var tradingState = new TradingState
        {
            CurrentOrderRequest = new OrderRequest
            {
                AccountId = accountId,
                Symbol = "AAPL",
                Quantity = 10,
                IdempotencyKey = Guid.NewGuid()
            },
            CurrentStockPrice = null
        };
        var validationResponse = new ValidationResponse
        {
            IsValid = false,
            Errors = new List<string> { "Price not available" }
        };
        _mockTradingState.Setup(x => x.Value).Returns(tradingState);
        _mockTradingService.Setup(x => x.ValidateOrderAsync(It.IsAny<ValidationRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(validationResponse);

        // Act
        await _effects.HandleValidateOrderAction(action, _mockDispatcher.Object);

        // Assert
        _mockTradingService.Verify(x => x.ValidateOrderAsync(
            It.Is<ValidationRequest>(r => r.EstimatedPrice == 0m),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    #endregion

    #region HandleExecuteOrderAction Tests

    [Fact]
    public async Task HandleExecuteOrderAction_WhenSuccessful_DispatchesMultipleActions()
    {
        // Arrange
        var accountId = Guid.NewGuid();
        var action = new ExecuteOrderAction(accountId);
        var tradingState = new TradingState
        {
            CurrentOrderRequest = new OrderRequest
            {
                AccountId = accountId,
                Symbol = "AAPL",
                Quantity = 10,
                IdempotencyKey = Guid.NewGuid()
            }
        };
        var orderResponse = new OrderResponse
        {
            OrderId = Guid.NewGuid(),
            AccountId = accountId,
            Symbol = "AAPL",
            Quantity = 10,
            PricePerShare = 150m,
            TotalAmount = 1500m,
            Status = "Completed"
        };
        _mockTradingState.Setup(x => x.Value).Returns(tradingState);
        _mockTradingService.Setup(x => x.PlaceOrderAsync(It.IsAny<OrderRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(orderResponse);

        // Act
        await _effects.HandleExecuteOrderAction(action, _mockDispatcher.Object);

        // Assert
        _mockTradingService.Verify(x => x.PlaceOrderAsync(
            It.Is<OrderRequest>(r =>
                r.AccountId == accountId &&
                r.Symbol == "AAPL" &&
                r.Quantity == 10),
            It.IsAny<CancellationToken>()), Times.Once);
        _mockDispatcher.Verify(x => x.Dispatch(It.Is<ExecuteOrderSuccessAction>(
            a => a.Order.OrderId == orderResponse.OrderId)), Times.Once);
        _mockDispatcher.Verify(x => x.Dispatch(It.Is<LoadAccountAction>(
            a => a.AccountId == accountId)), Times.Once);
        _mockDispatcher.Verify(x => x.Dispatch(It.Is<LoadOrderHistoryAction>(
            a => a.AccountId == accountId)), Times.Once);
    }

    [Fact]
    public async Task HandleExecuteOrderAction_WhenFails_DispatchesFailureAction()
    {
        // Arrange
        var accountId = Guid.NewGuid();
        var action = new ExecuteOrderAction(accountId);
        var tradingState = new TradingState
        {
            CurrentOrderRequest = new OrderRequest
            {
                AccountId = accountId,
                Symbol = "AAPL",
                Quantity = 10,
                IdempotencyKey = Guid.NewGuid()
            }
        };
        var exception = new ApplicationException("Insufficient funds");
        _mockTradingState.Setup(x => x.Value).Returns(tradingState);
        _mockTradingService.Setup(x => x.PlaceOrderAsync(It.IsAny<OrderRequest>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(exception);

        // Act
        await _effects.HandleExecuteOrderAction(action, _mockDispatcher.Object);

        // Assert
        _mockDispatcher.Verify(x => x.Dispatch(It.Is<ExecuteOrderFailureAction>(
            a => a.ErrorMessage == "Insufficient funds")), Times.Once);
        _mockDispatcher.Verify(x => x.Dispatch(It.IsAny<LoadAccountAction>()), Times.Never);
        _mockDispatcher.Verify(x => x.Dispatch(It.IsAny<LoadOrderHistoryAction>()), Times.Never);
    }

    #endregion

    #region HandleLoadOrderHistoryAction Tests

    [Fact]
    public async Task HandleLoadOrderHistoryAction_WhenSuccessful_DispatchesSuccessAction()
    {
        // Arrange
        var accountId = Guid.NewGuid();
        var action = new LoadOrderHistoryAction(accountId);
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
                Symbol = "MSFT",
                Quantity = 5,
                Status = "Completed"
            }
        };
        _mockTradingService.Setup(x => x.GetOrdersAsync(accountId, 1, 50, It.IsAny<CancellationToken>()))
            .ReturnsAsync(orders);

        // Act
        await _effects.HandleLoadOrderHistoryAction(action, _mockDispatcher.Object);

        // Assert
        _mockTradingService.Verify(x => x.GetOrdersAsync(accountId, 1, 50, It.IsAny<CancellationToken>()), Times.Once);
        _mockDispatcher.Verify(x => x.Dispatch(It.Is<LoadOrderHistorySuccessAction>(
            a => a.Orders.Count == 2)), Times.Once);
    }

    [Fact]
    public async Task HandleLoadOrderHistoryAction_WhenFails_DoesNotDispatchAnyAction()
    {
        // Arrange
        var accountId = Guid.NewGuid();
        var action = new LoadOrderHistoryAction(accountId);
        var exception = new ApplicationException("Failed to load orders");
        _mockTradingService.Setup(x => x.GetOrdersAsync(accountId, 1, 50, It.IsAny<CancellationToken>()))
            .ThrowsAsync(exception);

        // Act
        await _effects.HandleLoadOrderHistoryAction(action, _mockDispatcher.Object);

        // Assert
        _mockDispatcher.Verify(x => x.Dispatch(It.IsAny<object>()), Times.Never);
    }

    #endregion
}
