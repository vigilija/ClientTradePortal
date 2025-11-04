namespace ClientTradePortal.Services.Http;
public interface IAccountApiClient
{
    [Get("/api/accounts/{accountId}")]
    Task<ApiAccountResponse<AccountResponse>> GetAccountAsync(
        Guid accountId,
        CancellationToken cancellationToken = default);

    [Get("/api/accounts/{accountId}/balance")]
    Task<ApiAccountResponse<AccountBalanceResponse>> GetBalanceAsync(
        Guid accountId,
        CancellationToken cancellationToken = default);

    [Get("/api/accounts/{accountId}/positions")]
    Task<ApiAccountResponse<List<StockPositionResponse>>> GetPositionsAsync(
        Guid accountId,
        CancellationToken cancellationToken = default);
}