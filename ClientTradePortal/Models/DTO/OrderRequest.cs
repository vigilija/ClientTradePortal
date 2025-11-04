namespace ClientTradePortal.Models.DTO;

public record OrderRequest
{
    public Guid AccountId { get; set; }
    public string Symbol { get; set; } = "AAPL";
    public int Quantity { get; set; }
    public Guid IdempotencyKey { get; set; } = Guid.NewGuid();
}
