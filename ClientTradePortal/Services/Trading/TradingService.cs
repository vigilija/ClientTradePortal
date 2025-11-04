using ClientTradePortal.Models.DTO;

namespace ClientTradePortal.Services.Trading;

public class TradingService : ITradingService
{
    private readonly ILogger<TradingService> _logger;

    public TradingService(ILogger<TradingService> logger)
    {
        _logger = logger;
    }

    public Task<decimal> GetStockPriceAsync(string symbol, CancellationToken cancellationToken = default)
    {
        Console.WriteLine($" TradingService: GetStockPriceAsync called for {symbol}");
        _logger.LogInformation("Getting mock stock price for {Symbol}", symbol);

        // Return mock price
        var price = symbol switch
        {
            "AAPL" => 175.50m,
            "MSFT" => 380.25m,
            "GOOGL" => 140.75m,
            _ => 100.00m
        };

        return Task.FromResult(price);
    }

    public Task<OrderResponse> PlaceOrderAsync(OrderRequest request, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Placing mock order for {Quantity} shares of {Symbol}",
            request.Quantity, request.Symbol);

        // Simulate order execution
        var order = new OrderResponse
        {
            OrderId = Guid.NewGuid(),
            AccountId = request.AccountId,
            Symbol = request.Symbol,
            OrderType = "OrderType.Buy",
            Quantity = request.Quantity,
            PricePerShare = 175.50m,
            TotalAmount = 175.50m * request.Quantity,
            Status = "OrderStatus.Executed",
            CreatedAt = DateTime.UtcNow,
            ExecutedAt = DateTime.UtcNow
        };

        return Task.FromResult(order);
    }

    public Task<ValidationResponse> ValidateOrderAsync(ValidationRequest request, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Validating mock order");

        var errors = new List<string>();

        if (request.Quantity <= 0)
            errors.Add("Quantity must be greater than zero");

        if (request.Quantity > 10000)
            errors.Add("Quantity cannot exceed 10,000 shares");

        var totalAmount = request.EstimatedPrice * request.Quantity;

        // Mock: assume account has 50,000 EUR
        if (totalAmount > 50000)
            errors.Add($"Insufficient funds. Required: €{totalAmount:N2}, Available: €50,000.00");

        var response = new ValidationResponse
        {
            IsValid = errors.Count == 0,
            Errors = errors,
            CurrentPrice = request.EstimatedPrice,
            TotalAmount = totalAmount
        };

        return Task.FromResult(response);
    }

    public Task<List<OrderResponse>> GetOrdersAsync(Guid accountId, int pageNumber, int pageSize, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Getting mock order history");

        var orders = new List<OrderResponse>
        {
            new OrderResponse
            {
                OrderId = Guid.NewGuid(),
                AccountId = accountId,
                Symbol = "AAPL",
                OrderType = "OrderType.Buy",
                Quantity = 5,
                PricePerShare = 150.00m,
                TotalAmount = 750.00m,
                Status = "OrderStatus.Executed",
                CreatedAt = DateTime.UtcNow.AddDays(-7),
                ExecutedAt = DateTime.UtcNow.AddDays(-7)
            },
            new OrderResponse
            {
                OrderId = Guid.NewGuid(),
                AccountId = accountId,
                Symbol = "AAPL",
                OrderType = "OrderType.Buy",
                Quantity = 5,
                PricePerShare = 170.00m,
                TotalAmount = 850.00m,
                Status = "OrderStatus.Executed",
                CreatedAt = DateTime.UtcNow.AddDays(-3),
                ExecutedAt = DateTime.UtcNow.AddDays(-3)
            }
        };

        return Task.FromResult(orders);
    }
}
//using ClientTradePortal.Models.DTO;
//using ClientTradePortal.Services.Http;
//using Microsoft.Extensions.Caching.Memory;
//using Refit;

//namespace ClientTradePortal.Services.Trading;

//public class TradingService : ITradingService
//{
//    private readonly ITradingApiClient _tradingApiClient;
//    private readonly IValidationApiClient _validationApiClient;
//    private readonly ILogger<TradingService> _logger;
//    private readonly IMemoryCache _cache;

//    public TradingService(
//        ITradingApiClient tradingApiClient,
//        IValidationApiClient validationApiClient,
//        ILogger<TradingService> logger,
//        IMemoryCache cache)
//    {
//        _tradingApiClient = tradingApiClient;
//        _validationApiClient = validationApiClient;
//        _logger = logger;
//        _cache = cache;
//    }

//    public async Task<decimal> GetStockPriceAsync(
//        string symbol,
//        CancellationToken cancellationToken = default)
//    {
//        try
//        {
//            var cacheKey = $"stock_price_{symbol}";

//            if (_cache.TryGetValue(cacheKey, out decimal cachedPrice))
//            {
//                return cachedPrice;
//            }

//            var response = await _tradingApiClient.GetQuoteAsync(symbol, cancellationToken);

//            if (!response.IsSuccessful || response.Content == null)
//            {
//                //throw new ApplicationException(response.Message ?? "Failed to retrieve stock price");
//                throw new ApplicationException("Failed to retrieve stock price");
//            }

//            // Cache for 30 seconds
//            _cache.Set(cacheKey, response.Content.Price, TimeSpan.FromSeconds(30));

//            return response.Content.Price;
//        }
//        catch (ApiException ex)
//        {
//            _logger.LogError(ex, "API error while fetching stock price");
//            throw new ApplicationException("Failed to fetch stock price", ex);
//        }
//    }

//    public async Task<OrderResponse> PlaceOrderAsync(
//        OrderRequest request,
//        CancellationToken cancellationToken = default)
//    {
//        try
//        {
//            var response = await _tradingApiClient.PlaceOrderAsync(request, cancellationToken);

//            if (!response.IsSuccessful || response.Content == null)
//            {
//                // throw new ApplicationException(response.Message ?? "Failed to place order");
//                throw new ApplicationException( "Failed to place order");
//            }

//            return response.Content;
//        }
//        catch (ApiException ex)
//        {
//            _logger.LogError(ex, "API error while placing order");
//            throw new ApplicationException("Failed to place order", ex);
//        }
//    }

//    public async Task<ValidationResponse> ValidateOrderAsync(
//        ValidationRequest request,
//        CancellationToken cancellationToken = default)
//    {
//        try
//        {
//            var response = await _validationApiClient.ValidateOrderAsync(request, cancellationToken);

//            return response.Content ?? new ValidationResponse
//            {
//                IsValid = false,
//                Errors = new List<string> { "Validation failed" }
//            };
//        }
//        catch (ApiException ex)
//        {
//            _logger.LogError(ex, "API error while validating order");
//            return new ValidationResponse
//            {
//                IsValid = false,
//                Errors = new List<string> { "Validation service unavailable" }
//            };
//        }
//    }

//    public async Task<List<OrderResponse>> GetOrdersAsync(
//        Guid accountId,
//        int pageNumber,
//        int pageSize,
//        CancellationToken cancellationToken = default)
//    {
//        try
//        {
//            var response = await _tradingApiClient.GetOrdersAsync(
//                accountId,
//                pageNumber,
//                pageSize,
//                cancellationToken);

//            return response.Content ?? new List<OrderResponse>();
//        }
//        catch (ApiException ex)
//        {
//            _logger.LogError(ex, "API error while fetching orders");
//            return new List<OrderResponse>();
//        }
//    }
//    Task<List<OrderResponse>> ITradingService.GetOrdersAsync(Guid accountId, int pageNumber, int pageSize, CancellationToken cancellationToken)
//    {
//        throw new NotImplementedException();
//    }
//}