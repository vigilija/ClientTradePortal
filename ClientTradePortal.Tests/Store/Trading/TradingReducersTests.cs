namespace ClientTradePortal.Tests.Store.Trading;

public class TradingReducersTests
{
    [Fact]
    public void ReduceUpdateOrderQuantityAction_UpdatesQuantityAndClearsValidation()
    {
        // Arrange
        var initialState = new TradingState
        {
            CurrentOrderRequest = new OrderRequest
            {
                AccountId = Guid.NewGuid(),
                Symbol = "AAPL",
                Quantity = 5,
                IdempotencyKey = Guid.NewGuid()
            },
            ValidationResult = new ValidationResponse
            {
                IsValid = true,
                CurrentPrice = 150m,
                TotalAmount = 750m
            }
        };
        var action = new UpdateOrderQuantityAction(10);

        // Act
        var newState = TradingReducers.ReduceUpdateOrderQuantityAction(initialState, action);

        // Assert
        newState.Should().NotBeSameAs(initialState);
        newState.CurrentOrderRequest.Quantity.Should().Be(10);
        newState.ValidationResult.Should().BeNull();
        newState.CurrentOrderRequest.Symbol.Should().Be("AAPL");
    }

    [Fact]
    public void ReduceUpdateOrderSymbolAction_UpdatesSymbolAndClearsValidation()
    {
        // Arrange
        var initialState = new TradingState
        {
            CurrentOrderRequest = new OrderRequest
            {
                AccountId = Guid.NewGuid(),
                Symbol = "AAPL",
                Quantity = 10,
                IdempotencyKey = Guid.NewGuid()
            },
            ValidationResult = new ValidationResponse
            {
                IsValid = true,
                CurrentPrice = 150m,
                TotalAmount = 1500m
            }
        };
        var action = new UpdateOrderSymbolAction("MSFT");

        // Act
        var newState = TradingReducers.ReduceUpdateOrderSymbolAction(initialState, action);

        // Assert
        newState.Should().NotBeSameAs(initialState);
        newState.CurrentOrderRequest.Symbol.Should().Be("MSFT");
        newState.ValidationResult.Should().BeNull();
        newState.CurrentOrderRequest.Quantity.Should().Be(10);
    }

    [Fact]
    public void ReduceFetchStockPriceSuccessAction_UpdatesCurrentStockPrice()
    {
        // Arrange
        var initialState = new TradingState
        {
            CurrentStockPrice = null
        };
        var action = new FetchStockPriceSuccessAction(175.50m);

        // Act
        var newState = TradingReducers.ReduceFetchStockPriceSuccessAction(initialState, action);

        // Assert
        newState.Should().NotBeSameAs(initialState);
        newState.CurrentStockPrice.Should().Be(175.50m);
    }

    [Fact]
    public void ReduceValidateOrderAction_SetsIsValidatingAndClearsError()
    {
        // Arrange
        var initialState = new TradingState
        {
            IsValidating = false,
            ErrorMessage = "Some previous error"
        };
        var action = new ValidateOrderAction(Guid.NewGuid());

        // Act
        var newState = TradingReducers.ReduceValidateOrderAction(initialState, action);

        // Assert
        newState.Should().NotBeSameAs(initialState);
        newState.IsValidating.Should().BeTrue();
        newState.ErrorMessage.Should().BeNull();
    }

    [Fact]
    public void ReduceValidateOrderSuccessAction_SetsValidationResultAndClearsIsValidating()
    {
        // Arrange
        var initialState = new TradingState
        {
            IsValidating = true,
            ValidationResult = null
        };
        var validationResult = new ValidationResponse
        {
            IsValid = true,
            CurrentPrice = 150m,
            TotalAmount = 1500m
        };
        var action = new ValidateOrderSuccessAction(validationResult);

        // Act
        var newState = TradingReducers.ReduceValidateOrderSuccessAction(initialState, action);

        // Assert
        newState.Should().NotBeSameAs(initialState);
        newState.IsValidating.Should().BeFalse();
        newState.ValidationResult.Should().NotBeNull();
        newState.ValidationResult!.IsValid.Should().BeTrue();
        newState.ValidationResult.CurrentPrice.Should().Be(150m);
    }

    [Fact]
    public void ReduceExecuteOrderAction_SetsIsExecutingAndClearsError()
    {
        // Arrange
        var initialState = new TradingState
        {
            IsExecuting = false,
            ErrorMessage = "Previous error"
        };
        var action = new ExecuteOrderAction(Guid.NewGuid());

        // Act
        var newState = TradingReducers.ReduceExecuteOrderAction(initialState, action);

        // Assert
        newState.Should().NotBeSameAs(initialState);
        newState.IsExecuting.Should().BeTrue();
        newState.ErrorMessage.Should().BeNull();
    }

    [Fact]
    public void ReduceExecuteOrderSuccessAction_ResetsOrderAndSetsLastExecutedOrder()
    {
        // Arrange
        var accountId = Guid.NewGuid();
        var initialState = new TradingState
        {
            IsExecuting = true,
            CurrentOrderRequest = new OrderRequest
            {
                AccountId = accountId,
                Symbol = "AAPL",
                Quantity = 10,
                IdempotencyKey = Guid.NewGuid()
            },
            ValidationResult = new ValidationResponse
            {
                IsValid = true,
                CurrentPrice = 150m,
                TotalAmount = 1500m
            },
            LastExecutedOrder = null
        };
        var executedOrder = new OrderResponse
        {
            OrderId = Guid.NewGuid(),
            AccountId = accountId,
            Symbol = "AAPL",
            Quantity = 10,
            PricePerShare = 150m,
            TotalAmount = 1500m,
            Status = "Completed"
        };
        var action = new ExecuteOrderSuccessAction(executedOrder);

        // Act
        var newState = TradingReducers.ReduceExecuteOrderSuccessAction(initialState, action);

        // Assert
        newState.Should().NotBeSameAs(initialState);
        newState.IsExecuting.Should().BeFalse();
        newState.LastExecutedOrder.Should().NotBeNull();
        newState.LastExecutedOrder!.OrderId.Should().Be(executedOrder.OrderId);
        newState.CurrentOrderRequest.Quantity.Should().Be(0);
        newState.ValidationResult.Should().BeNull();
    }

    [Fact]
    public void ReduceExecuteOrderFailureAction_SetsErrorMessageAndClearsIsExecuting()
    {
        // Arrange
        var initialState = new TradingState
        {
            IsExecuting = true,
            ErrorMessage = null
        };
        var action = new ExecuteOrderFailureAction("Insufficient funds");

        // Act
        var newState = TradingReducers.ReduceExecuteOrderFailureAction(initialState, action);

        // Assert
        newState.Should().NotBeSameAs(initialState);
        newState.IsExecuting.Should().BeFalse();
        newState.ErrorMessage.Should().Be("Insufficient funds");
    }

    [Fact]
    public void ReduceLoadOrderHistorySuccessAction_UpdatesOrderHistory()
    {
        // Arrange
        var initialState = new TradingState
        {
            OrderHistory = new List<OrderResponse>()
        };
        var orders = new List<OrderResponse>
        {
            new OrderResponse
            {
                OrderId = Guid.NewGuid(),
                Symbol = "AAPL",
                Quantity = 10,
                Status = "Completed"
            },
            new OrderResponse
            {
                OrderId = Guid.NewGuid(),
                Symbol = "MSFT",
                Quantity = 5,
                Status = "Completed"
            }
        };
        var action = new LoadOrderHistorySuccessAction(orders);

        // Act
        var newState = TradingReducers.ReduceLoadOrderHistorySuccessAction(initialState, action);

        // Assert
        newState.Should().NotBeSameAs(initialState);
        newState.OrderHistory.Should().HaveCount(2);
        newState.OrderHistory[0].Symbol.Should().Be("AAPL");
        newState.OrderHistory[1].Symbol.Should().Be("MSFT");
    }

    [Fact]
    public void ReduceResetOrderAction_ResetsAllOrderRelatedState()
    {
        // Arrange
        var initialState = new TradingState
        {
            CurrentOrderRequest = new OrderRequest
            {
                AccountId = Guid.NewGuid(),
                Symbol = "AAPL",
                Quantity = 10,
                IdempotencyKey = Guid.NewGuid()
            },
            CurrentStockPrice = 150m,
            ValidationResult = new ValidationResponse
            {
                IsValid = true,
                CurrentPrice = 150m,
                TotalAmount = 1500m
            },
            ErrorMessage = "Some error"
        };
        var action = new ResetOrderAction();

        // Act
        var newState = TradingReducers.ReduceResetOrderAction(initialState, action);

        // Assert
        newState.Should().NotBeSameAs(initialState);
        newState.CurrentOrderRequest.Quantity.Should().Be(0);
        newState.CurrentStockPrice.Should().BeNull();
        newState.ValidationResult.Should().BeNull();
        newState.ErrorMessage.Should().BeNull();
    }

    [Fact]
    public void Reducers_ShouldReturnNewStateInstance_NotMutateOriginal()
    {
        // Arrange
        var originalState = new TradingState
        {
            CurrentOrderRequest = new OrderRequest
            {
                AccountId = Guid.NewGuid(),
                Symbol = "AAPL",
                Quantity = 5,
                IdempotencyKey = Guid.NewGuid()
            }
        };
        var action = new UpdateOrderQuantityAction(10);

        // Act
        var newState = TradingReducers.ReduceUpdateOrderQuantityAction(originalState, action);

        // Assert
        newState.Should().NotBeSameAs(originalState);
        originalState.CurrentOrderRequest.Quantity.Should().Be(5);
        newState.CurrentOrderRequest.Quantity.Should().Be(10);
    }
}
