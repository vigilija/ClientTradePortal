using ClientTradePortal.Models.DTO;
using Refit;

namespace ClientTradePortal.Services.Http;
public interface IAccountApiClient
{
    [Get("/api/accounts/{accountId}")]
    Task<Refit.ApiResponse<AccountResponse>> GetAccountAsync(
        Guid accountId,
        CancellationToken cancellationToken = default);

    [Get("/api/accounts/{accountId}/balance")]
    Task<Refit.ApiResponse<AccountBalanceResponse>> GetBalanceAsync(
        Guid accountId,
        CancellationToken cancellationToken = default);

    [Get("/api/accounts/{accountId}/positions")]
    Task<Refit.ApiResponse<List<StockPositionResponse>>> GetPositionsAsync(
        Guid accountId,
        CancellationToken cancellationToken = default);
}