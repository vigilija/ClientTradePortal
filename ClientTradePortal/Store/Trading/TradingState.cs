namespace ClientTradePortal.Store.Trading;

[FeatureState]
public record TradingState
{
    public OrderRequest CurrentOrderRequest { get; init; } = new();
    public decimal? CurrentStockPrice { get; init; }
    public decimal TotalOrderAmount => (CurrentStockPrice ?? 0) * CurrentOrderRequest.Quantity;
    public bool IsValidating { get; init; }
    public bool IsExecuting { get; init; }
    public ValidationResponse? ValidationResult { get; init; }
    public OrderResponse? LastExecutedOrder { get; init; }
    public List<OrderResponse> OrderHistory { get; init; } = new();
    public string? ErrorMessage { get; init; }
}