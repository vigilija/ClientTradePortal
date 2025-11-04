using ClientTradePortal.Models.DTO;
using ClientTradePortal.Services.Http;

namespace ClientTradePortal.Services.Trading;
public interface ITradingService
{
    Task<StockQuoteResponse> GetStockPriceAsync(string symbol, CancellationToken cancellationToken = default);
    Task<OrderResponse> PlaceOrderAsync(OrderRequest request, CancellationToken cancellationToken = default);
    Task<ValidationResponse> ValidateOrderAsync(ValidationRequest request, CancellationToken cancellationToken = default);
    Task<List<OrderResponse>> GetOrdersAsync(Guid accountId, int pageNumber, int pageSize, CancellationToken cancellationToken = default);
}
