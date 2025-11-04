using ClientTradePortal.Models.DTO;
using ClientTradePortal.Services.Http;
using Refit;
namespace ClientTradePortal.Services.Account;
public class AccountService : IAccountService
{
    private readonly IAccountApiClient _apiClient;
    private readonly ILogger<AccountService> _logger;
    private readonly IMemoryCache _cache;

    // Constructor injection (exactly like Angular)
    public AccountService(
        IAccountApiClient apiClient,
        ILogger<AccountService> logger,
        IMemoryCache cache)
    {
        _apiClient = apiClient;
        _logger = logger;
        _cache = cache;
    }

    public async Task<AccountResponse> GetAccountAsync(
        Guid accountId,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var cacheKey = $"account_{accountId}";

          //  Check cache first
            if (_cache.TryGetValue(cacheKey, out AccountResponse? cached) && cached != null)
            {
                _logger.LogDebug("Retrieved account from cache");
                return cached;
            }

            var response = await _apiClient.GetAccountAsync(accountId, cancellationToken);

            if (!response.Success || response.Data == null)
            {
                throw new ApplicationException($"Failed to retrieve account. Status Code: {response.Errors}, Reason: {response.Message}");
            }

            // Cache for 5 minutes
            _cache.Set(cacheKey, response.Data, TimeSpan.FromMinutes(5));

            return response.Data;
        }
        catch (ApiException ex)
        {
            _logger.LogError(ex, "API error while fetching account");
            throw new ApplicationException("Failed to fetch account", ex);
        }
    }

    public async Task<AccountBalanceResponse> GetBalanceAsync(
        Guid accountId,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var response = await _apiClient.GetBalanceAsync(accountId, cancellationToken);

            if (!response.Success || response.Data == null)
            {
                throw new ApplicationException($"Failed to retrieve balance. Status Code: {response.Errors}, Reason: {response.Message}");
            }

            return response.Data;
        }
        catch (ApiException ex)
        {
            _logger.LogError(ex, "API error while fetching balance");
            throw new ApplicationException("Failed to fetch balance", ex);
        }
    }

    public async Task<List<StockPositionResponse>> GetPositionsAsync(
        Guid accountId,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var response = await _apiClient.GetPositionsAsync(accountId, cancellationToken);

            if (!response.Success || response.Data == null)
            {
                throw new ApplicationException($"Failed to retrieve positions. Status Code: {response.Errors}, Reason: {response.Message}");

            }

            return response.Data;
        }
        catch (ApiException ex)
        {
            _logger.LogError(ex, "API error while fetching positions");
            throw new ApplicationException("Failed to fetch positions", ex);
        }
    }
}