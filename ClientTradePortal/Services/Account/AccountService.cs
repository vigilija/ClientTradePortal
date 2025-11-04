using ClientTradePortal.Models.DTO;

namespace ClientTradePortal.Services.Account;

public class AccountService : IAccountService
{
    private readonly ILogger<AccountService> _logger;

    public AccountService(ILogger<AccountService> logger)
    {
        _logger = logger;
    }

    public Task<AccountResponse> GetAccountAsync(Guid accountId, CancellationToken cancellationToken = default)
    {
        Console.WriteLine($" AccountService: GetAccountAsync called for {accountId}");
        _logger.LogInformation("Getting mock account data for {AccountId}", accountId);

        // Return mock data
        var account = new AccountResponse
        {
            AccountId = accountId,
            ClientId = Guid.NewGuid(),
            CashBalance = 50000.00m,
            Currency = "EUR",
            Positions = new List<StockPositionResponse>
            {
                new StockPositionResponse
                {
                    Symbol = "AAPL",
                    Quantity = 10,
                    AveragePrice = 150.00m,
                    CurrentPrice = 175.50m
                },
                new StockPositionResponse
                {
                    Symbol = "AAPL",
                    Quantity = 15,
                    AveragePrice = 190.00m,
                    CurrentPrice = 195.50m
                }
            }
        };

        return Task.FromResult(account);
    }

    public Task<AccountBalanceResponse> GetBalanceAsync(Guid accountId, CancellationToken cancellationToken = default)
    {
        var balance = new AccountBalanceResponse
        {
            AccountId = accountId,
            CashBalance = 50000.00m,
            Currency = "EUR"
        };

        return Task.FromResult(balance);
    }

    public Task<List<StockPositionResponse>> GetPositionsAsync(Guid accountId, CancellationToken cancellationToken = default)
    {
        var positions = new List<StockPositionResponse>
        {
            new StockPositionResponse
            {
                Symbol = "AAPL",
                Quantity = 10,
                AveragePrice = 150.00m,
                CurrentPrice = 175.50m
            }
        };

        return Task.FromResult(positions);
    }
}
//using ClientTradePortal.Models.DTO;
//using ClientTradePortal.Services.Http;
//using Refit;
//using Microsoft.Extensions.Caching.Memory; 

//namespace ClientTradePortal.Services.Account;
//public class AccountService : IAccountService
//{
//    private readonly IAccountApiClient _apiClient;
//    private readonly ILogger<AccountService> _logger;
//    private readonly IMemoryCache _cache;

//    // Constructor injection (exactly like Angular)
//    public AccountService(
//        IAccountApiClient apiClient,
//        ILogger<AccountService> logger,
//        IMemoryCache cache)
//    {
//        _apiClient = apiClient;
//        _logger = logger;
//        _cache = cache;
//    }

//    public async Task<AccountResponse> GetAccountAsync(
//        Guid accountId,
//        CancellationToken cancellationToken = default)
//    {
//        try
//        {
//            var cacheKey = $"account_{accountId}";

//            // Check cache first
//            if (_cache.TryGetValue(cacheKey, out AccountResponse? cached) && cached != null)
//            {
//                _logger.LogDebug("Retrieved account from cache");
//                return cached;
//            }

//            var response = await _apiClient.GetAccountAsync(accountId, cancellationToken);

//            if (!response.IsSuccessful|| response.Content == null)
//            {
//                throw new ApplicationException($"Failed to retrieve account. Status Code: {response.StatusCode}, Reason: {response.ReasonPhrase}");
//            }

//            // Cache for 5 minutes
//            _cache.Set(cacheKey, response.Content, TimeSpan.FromMinutes(5));

//            return response.Content;
//        }
//        catch (ApiException ex)
//        {
//            _logger.LogError(ex, "API error while fetching account");
//            throw new ApplicationException("Failed to fetch account", ex);
//        }
//    }

//    public async Task<AccountBalanceResponse> GetBalanceAsync(
//        Guid accountId,
//        CancellationToken cancellationToken = default)
//    {
//        try
//        {
//            var response = await _apiClient.GetBalanceAsync(accountId, cancellationToken);

//            if (!response.IsSuccessful || response.Content == null)
//            {
//                throw new ApplicationException($"Failed to retrieve balance. Status Code: {response.StatusCode}, Reason: {response.ReasonPhrase}");
//            }

//            return response.Content;
//        }
//        catch (ApiException ex)
//        {
//            _logger.LogError(ex, "API error while fetching balance");
//            throw new ApplicationException("Failed to fetch balance", ex);
//        }
//    }

//    public async Task<List<StockPositionResponse>> GetPositionsAsync(
//        Guid accountId,
//        CancellationToken cancellationToken = default)
//    {
//        try
//        {
//            var response = await _apiClient.GetPositionsAsync(accountId, cancellationToken);

//            if (!response.IsSuccessful || response.Content == null)
//            {
//                throw new ApplicationException($"Failed to retrieve positions. Status Code: {response.StatusCode}, Reason: {response.ReasonPhrase}");

//            }

//            return response.Content;
//        }
//        catch (ApiException ex)
//        {
//            _logger.LogError(ex, "API error while fetching positions");
//            throw new ApplicationException("Failed to fetch positions", ex);
//        }
//    }
//}