public static class TradingReducers
{
    [ReducerMethod]
    public static TradingState ReduceUpdateOrderQuantityAction(
        TradingState state,
        UpdateOrderQuantityAction action)
    {
        return state with
        {
            CurrentOrderRequest = state.CurrentOrderRequest with
            {
                Quantity = action.Quantity
            },
            ValidationResult = null
        };
    }

    [ReducerMethod]
    public static TradingState ReduceUpdateOrderSymbolAction(
        TradingState state,
        UpdateOrderSymbolAction action)
    {
        return state with
        {
            CurrentOrderRequest = state.CurrentOrderRequest with
            {
                Symbol = action.Symbol
            },
            ValidationResult = null
        };
    }

    [ReducerMethod]
    public static TradingState ReduceFetchStockPriceSuccessAction(
        TradingState state,
        FetchStockPriceSuccessAction action)
    {
        return state with
        {
            CurrentStockPrice = action.Price
        };
    }

    [ReducerMethod]
    public static TradingState ReduceValidateOrderAction(
        TradingState state,
        ValidateOrderAction action)
    {
        return state with
        {
            IsValidating = true,
            ErrorMessage = null
        };
    }

    [ReducerMethod]
    public static TradingState ReduceValidateOrderSuccessAction(
        TradingState state,
        ValidateOrderSuccessAction action)
    {
        return state with
        {
            IsValidating = false,
            ValidationResult = action.Result
        };
    }

    [ReducerMethod]
    public static TradingState ReduceExecuteOrderAction(
        TradingState state,
        ExecuteOrderAction action)
    {
        return state with
        {
            IsExecuting = true,
            ErrorMessage = null
        };
    }

    [ReducerMethod]
    public static TradingState ReduceExecuteOrderSuccessAction(
        TradingState state,
        ExecuteOrderSuccessAction action)
    {
        return state with
        {
            IsExecuting = false,
            LastExecutedOrder = action.Order,
            CurrentOrderRequest = new OrderRequest(),
            ValidationResult = null
        };
    }

    [ReducerMethod]
    public static TradingState ReduceExecuteOrderFailureAction(
        TradingState state,
        ExecuteOrderFailureAction action)
    {
        return state with
        {
            IsExecuting = false,
            ErrorMessage = action.ErrorMessage
        };
    }

    [ReducerMethod]
    public static TradingState ReduceLoadOrderHistorySuccessAction(
        TradingState state,
        LoadOrderHistorySuccessAction action)
    {
        return state with
        {
            OrderHistory = action.Orders
        };
    }

    [ReducerMethod]
    public static TradingState ReduceResetOrderAction(
        TradingState state,
        ResetOrderAction action)
    {
        return state with
        {
            CurrentOrderRequest = new OrderRequest(),
            CurrentStockPrice = null,
            ValidationResult = null,
            ErrorMessage = null
        };
    }
}// Modify the OrderRequest class to make it a record type, as the error indicates that
// the `with` expression requires the type to be a record or struct.

public record OrderRequest
{
    public Guid AccountId { get; init; }
    public string Symbol { get; init; }
    public int Quantity { get; init; }
    public Guid IdempotencyKey { get; init; }
}
