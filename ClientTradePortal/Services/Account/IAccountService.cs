namespace ClientTradePortal.Services.Account;
public interface IAccountService
{
    Task<AccountResponse> GetAccountAsync(
        Guid accountId,
        CancellationToken cancellationToken = default);

    Task<AccountBalanceResponse> GetBalanceAsync(
        Guid accountId,
        CancellationToken cancellationToken = default);

    Task<List<StockPositionResponse>> GetPositionsAsync(
        Guid accountId,
        CancellationToken cancellationToken = default);
}