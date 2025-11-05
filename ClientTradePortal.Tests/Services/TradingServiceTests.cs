using ClientTradePortal.Models.DTO;

namespace ClientTradePortal.Tests.Services;

public class TradingServiceTests
{
    [Fact]
    public async Task PlaceOrderAsync_Should_Return_OrderResponse_On_Success()
    {
        // arrange
        var apiMock = new Mock<ITradingApiClient>();
        var orderResponse = new OrderResponse { OrderId = Guid.NewGuid() };
        apiMock.Setup(a => a.PlaceOrderAsync(It.IsAny<OrderRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ApiTradingResponse<OrderResponse> { Success = true, Data = orderResponse });

        var validationMock = new Mock<IValidationApiClient>();
        var logger = new Mock<ILogger<TradingService>>().Object;
        var cache = new MemoryCache(new MemoryCacheOptions());

        var service = new TradingService(apiMock.Object, validationMock.Object, logger, cache);

        var request = new OrderRequest { AccountId = Guid.NewGuid(), Symbol = "AAPL", Quantity = 5 };

        // act
        var result = await service.PlaceOrderAsync(request);

        // assert
        result.OrderId.Should().Be(orderResponse.OrderId);
        apiMock.Verify(a => a.PlaceOrderAsync(It.Is<OrderRequest>(r => r.Symbol == "AAPL" && r.Quantity == 5), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GetStockPriceAsyncShouldReturnSuccess()
    {
        // arrange
        var apiMock = new Mock<ITradingApiClient>();
        apiMock.Setup(a => a.GetQuoteAsync("AAPL", It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ApiTradingResponse<StockQuoteResponse>
            {
                Success = true,
                Data = new StockQuoteResponse
                {
                    Symbol = "AAPL",
                    Price = 100.00m
                }
            });

        var validationMock = new Mock<IValidationApiClient>();
        var logger = new Mock<ILogger<TradingService>>().Object;
        var cache = new MemoryCache(new MemoryCacheOptions());

        var service = new TradingService(apiMock.Object, validationMock.Object, logger, cache);

        // act
        var result = await service.GetStockPriceAsync("AAPL");

        // assert
        result.Should().NotBeNull();
        result.Symbol.Should().Be("AAPL");
        apiMock.Verify(a => a.GetQuoteAsync("AAPL", It.IsAny<CancellationToken>()), Times.Once);
    }
}
