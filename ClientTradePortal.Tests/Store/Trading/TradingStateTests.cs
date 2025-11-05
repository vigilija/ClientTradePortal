namespace ClientTradePortal.Tests.Store.Trading;

public class TradingStateTests
{
    [Fact]
    public void TotalOrderAmount_WhenPriceAndQuantityAreSet_ReturnsCorrectTotal()
    {
        // Arrange
        var state = new TradingState
        {
            CurrentStockPrice = 150.00m,
            CurrentOrderRequest = new OrderRequest
            {
                AccountId = Guid.NewGuid(),
                Symbol = "AAPL",
                Quantity = 10,
                IdempotencyKey = Guid.NewGuid()
            }
        };

        // Act
        var totalAmount = state.TotalOrderAmount;

        // Assert
        totalAmount.Should().Be(1500.00m);
    }

    [Fact]
    public void TotalOrderAmount_WhenPriceIsNull_ReturnsZero()
    {
        // Arrange
        var state = new TradingState
        {
            CurrentStockPrice = null,
            CurrentOrderRequest = new OrderRequest
            {
                AccountId = Guid.NewGuid(),
                Symbol = "AAPL",
                Quantity = 10,
                IdempotencyKey = Guid.NewGuid()
            }
        };

        // Act
        var totalAmount = state.TotalOrderAmount;

        // Assert
        totalAmount.Should().Be(0m);
    }

    [Fact]
    public void TotalOrderAmount_WhenQuantityIsZero_ReturnsZero()
    {
        // Arrange
        var state = new TradingState
        {
            CurrentStockPrice = 150.00m,
            CurrentOrderRequest = new OrderRequest
            {
                AccountId = Guid.NewGuid(),
                Symbol = "AAPL",
                Quantity = 0,
                IdempotencyKey = Guid.NewGuid()
            }
        };

        // Act
        var totalAmount = state.TotalOrderAmount;

        // Assert
        totalAmount.Should().Be(0m);
    }

    [Fact]
    public void TotalOrderAmount_WhenBothPriceAndQuantityAreZero_ReturnsZero()
    {
        // Arrange
        var state = new TradingState
        {
            CurrentStockPrice = 0m,
            CurrentOrderRequest = new OrderRequest
            {
                AccountId = Guid.NewGuid(),
                Symbol = "AAPL",
                Quantity = 0,
                IdempotencyKey = Guid.NewGuid()
            }
        };

        // Act
        var totalAmount = state.TotalOrderAmount;

        // Assert
        totalAmount.Should().Be(0m);
    }

    [Fact]
    public void TotalOrderAmount_WithDecimalPrice_CalculatesCorrectly()
    {
        // Arrange
        var state = new TradingState
        {
            CurrentStockPrice = 175.50m,
            CurrentOrderRequest = new OrderRequest
            {
                AccountId = Guid.NewGuid(),
                Symbol = "AAPL",
                Quantity = 7,
                IdempotencyKey = Guid.NewGuid()
            }
        };

        // Act
        var totalAmount = state.TotalOrderAmount;

        // Assert
        totalAmount.Should().Be(1228.50m);
    }

    [Fact]
    public void TradingState_DefaultState_HasCorrectDefaults()
    {
        // Arrange & Act
        var state = new TradingState();

        // Assert
        state.CurrentOrderRequest.Should().NotBeNull();
        state.CurrentStockPrice.Should().BeNull();
        state.TotalOrderAmount.Should().Be(0m);
        state.IsValidating.Should().BeFalse();
        state.IsExecuting.Should().BeFalse();
        state.ValidationResult.Should().BeNull();
        state.LastExecutedOrder.Should().BeNull();
        state.OrderHistory.Should().NotBeNull();
        state.OrderHistory.Should().BeEmpty();
        state.ErrorMessage.Should().BeNull();
    }

    [Fact]
    public void TradingState_WithExpression_CreatesNewInstance()
    {
        // Arrange
        var originalState = new TradingState
        {
            CurrentStockPrice = 100m,
            IsValidating = true
        };

        // Act
        var newState = originalState with { CurrentStockPrice = 200m };

        // Assert
        newState.Should().NotBeSameAs(originalState);
        originalState.CurrentStockPrice.Should().Be(100m);
        newState.CurrentStockPrice.Should().Be(200m);
        newState.IsValidating.Should().BeTrue(); // Other properties remain the same
    }
}
