namespace ClientTradePortal.Services.Trading;

public class TradingService : ITradingService
{
    private readonly ITradingApiClient _tradingApiClient;
    private readonly IValidationApiClient _validationApiClient;
    private readonly ILogger<TradingService> _logger;
    private readonly IMemoryCache _cache;

    public TradingService(
        ITradingApiClient tradingApiClient,
        IValidationApiClient validationApiClient,
        ILogger<TradingService> logger,
        IMemoryCache cache)
    {
        _tradingApiClient = tradingApiClient;
        _validationApiClient = validationApiClient;
        _logger = logger;
        _cache = cache;
    }

    public async Task<StockQuoteResponse> GetStockPriceAsync(
        string symbol,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var cacheKey = $"stock_price_{symbol}";

            if (_cache.TryGetValue(cacheKey, out StockQuoteResponse cachedPrice))
            {
                return cachedPrice;
            }

            var response = await _tradingApiClient.GetQuoteAsync(symbol, cancellationToken);

            if (!response.Success || response.Data == null)
            {
                //throw new ApplicationException(response.Message ?? "Failed to retrieve stock price");
                throw new ApplicationException("Failed to retrieve stock price");
            }

            // Cache for 30 seconds
            _cache.Set(cacheKey, response.Data.Price, TimeSpan.FromSeconds(30));

            return response.Data;
        }
        catch (ApiException ex)
        {
            _logger.LogError(ex, "API error while fetching stock price");
            throw new ApplicationException("Failed to fetch stock price", ex);
        }
    }

    public async Task<OrderResponse> PlaceOrderAsync(
        OrderRequest request,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var response = await _tradingApiClient.PlaceOrderAsync(request, cancellationToken);

            if (!response.Success || response.Data == null)
            {
                // throw new ApplicationException(response.Message ?? "Failed to place order");
                throw new ApplicationException("Failed to place order");
            }

            return response.Data;
        }
        catch (ApiException ex)
        {
            _logger.LogError(ex, "API error while placing order");
            throw new ApplicationException("Failed to place order", ex);
        }
    }

    public async Task<ValidationResponse> ValidateOrderAsync(
        ValidationRequest request,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var response = await _validationApiClient.ValidateOrderAsync(request, cancellationToken);

            return response.Content ?? new ValidationResponse
            {
                IsValid = false,
                Errors = new List<string> { "Validation failed" }
            };
        }
        catch (ApiException ex)
        {
            _logger.LogError(ex, "API error while validating order");
            return new ValidationResponse
            {
                IsValid = false,
                Errors = new List<string> { "Validation service unavailable" }
            };
        }
    }

    public async Task<List<OrderResponse>> GetOrdersAsync(
        Guid accountId,
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var response = await _tradingApiClient.GetOrdersAsync(
                accountId,
                pageNumber,
                pageSize,
                cancellationToken);

            return response.Data ?? new List<OrderResponse>();
        }
        catch (ApiException ex)
        {
            _logger.LogError(ex, "API error while fetching orders");
            return new List<OrderResponse>();
        }
    }
}