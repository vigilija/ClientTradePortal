using ClientTradePortal.Models.DTO;
using Refit;

namespace ClientTradePortal.Services.Http;

public interface ITradingApiClient
{
    [Get("/api/trading/quote")]
    Task<Refit.ApiResponse<StockQuoteResponse>> GetQuoteAsync(
        [Query] string symbol,
        CancellationToken cancellationToken = default);

    [Post("/api/trading/orders")]
    Task<Refit.ApiResponse<OrderResponse>> PlaceOrderAsync(
        [Body] OrderRequest request,
        CancellationToken cancellationToken = default);

    [Get("/api/trading/orders/{orderId}")]
    Task<Refit.ApiResponse<OrderResponse>> GetOrderAsync(
        Guid orderId,
        CancellationToken cancellationToken = default);

    [Get("/api/trading/orders")]
    Task<Refit.ApiResponse<List<OrderResponse>>> GetOrdersAsync(
        [Query] Guid accountId,
        [Query] int pageNumber = 1,
        [Query] int pageSize = 20,
        CancellationToken cancellationToken = default);
}