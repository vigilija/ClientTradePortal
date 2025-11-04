using ClientTradePortal.Models.DTO;
using Refit;

namespace ClientTradePortal.Services.Http;

public interface ITradingApiClient
{
    [Get("/api/trading/quote")]
    Task<ApiTradingResponse<StockQuoteResponse>> GetQuoteAsync(
        [Query] string symbol,
        CancellationToken cancellationToken = default);

    [Post("/api/trading/orders")]
    Task<ApiTradingResponse<OrderResponse>> PlaceOrderAsync(
        [Body] OrderRequest request,
        CancellationToken cancellationToken = default);

    [Get("/api/trading/orders/{orderId}")]
    Task<ApiTradingResponse<OrderResponse>> GetOrderAsync(
        Guid orderId,
        CancellationToken cancellationToken = default);

    [Get("/api/trading/orders")]
    Task<ApiTradingResponse<List<OrderResponse>>> GetOrdersAsync(
        [Query] Guid accountId,
        [Query] int pageNumber,
        [Query] int pageSize,
        CancellationToken cancellationToken = default);
}