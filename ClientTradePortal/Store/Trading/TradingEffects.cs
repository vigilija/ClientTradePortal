namespace ClientTradePortal.Store.Trading;

public class TradingEffects
{
    private readonly ITradingService _tradingService;
    private readonly ILogger<TradingEffects> _logger;
    private readonly IState<TradingState> _tradingState;

    public TradingEffects(
        ITradingService tradingService,
        ILogger<TradingEffects> logger,
        IState<TradingState> tradingState)
    {
        _tradingService = tradingService;
        _logger = logger;
        _tradingState = tradingState;
    }

    [EffectMethod]
    public async Task HandleFetchStockPriceAction(
        FetchStockPriceAction action,
        IDispatcher dispatcher)
    {
        Console.WriteLine($"EFFECT TRIGGERED TradingEffects: Fetching price for {action.Symbol}");

        try
        {
            var result = await _tradingService.GetStockPriceAsync(action.Symbol);
            dispatcher.Dispatch(new FetchStockPriceSuccessAction(result.Price));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to fetch stock price");
            dispatcher.Dispatch(new FetchStockPriceFailureAction(ex.Message));
        }
    }

    [EffectMethod]
    public async Task HandleValidateOrderAction(
        ValidateOrderAction action,
        IDispatcher dispatcher)
    {
        Console.WriteLine($"EFFECT TRIGGERED HandleValidateOrderAction: Fetching price for {action.AccountId}");
        try
        {
            var state = _tradingState.Value;

            var validationRequest = new ValidationRequest
            {
                AccountId = action.AccountId,
                Symbol = state.CurrentOrderRequest.Symbol,
                Quantity = state.CurrentOrderRequest.Quantity,
                EstimatedPrice = state.CurrentStockPrice ?? 0
            };

            var result = await _tradingService.ValidateOrderAsync(validationRequest);

            dispatcher.Dispatch(new ValidateOrderSuccessAction(result));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to validate order");
            dispatcher.Dispatch(new ValidateOrderFailureAction(ex.Message));
        }
    }
    [EffectMethod]
    public async Task HandleExecuteOrderAction(
           ExecuteOrderAction action,
           IDispatcher dispatcher)
    {
        Console.WriteLine($"EFFECT TRIGGERED HandleExecuteOrderAction: Fetching price for {action.AccountId}");
        try
        {
            var state = _tradingState.Value;

            var orderRequest = new OrderRequest
            {
                AccountId = action.AccountId,
                Symbol = state.CurrentOrderRequest.Symbol,
                Quantity = state.CurrentOrderRequest.Quantity,
                IdempotencyKey = Guid.NewGuid()
            };

            var result = await _tradingService.PlaceOrderAsync(orderRequest);

            dispatcher.Dispatch(new ExecuteOrderSuccessAction(result));

            // Reload account data
            dispatcher.Dispatch(new LoadAccountAction(action.AccountId));

            // Reload order history
            dispatcher.Dispatch(new LoadOrderHistoryAction(action.AccountId));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to execute order");
            dispatcher.Dispatch(new ExecuteOrderFailureAction(ex.Message));
        }
    }

    [EffectMethod]
    public async Task HandleLoadOrderHistoryAction(
        LoadOrderHistoryAction action,
        IDispatcher dispatcher)
    {
        Console.WriteLine($"EFFECT TRIGGERED HandleLoadOrderHistoryAction: Fetching price for {action.AccountId}");
        try
        {
            var orders = await _tradingService.GetOrdersAsync(action.AccountId, 1, 50);
            dispatcher.Dispatch(new LoadOrderHistorySuccessAction(orders));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to load order history");
        }
    }
}