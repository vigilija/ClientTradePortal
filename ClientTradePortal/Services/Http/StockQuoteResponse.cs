namespace ClientTradePortal.Services.Http;

public class StockQuoteResponse
{
    public string Symbol { get; set; } = string.Empty;
    public decimal Price { get; set; }
}