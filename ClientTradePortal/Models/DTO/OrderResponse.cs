namespace ClientTradePortal.Models.DTO;

public class OrderResponse
{
    public Guid OrderId { get; set; }
    public Guid AccountId { get; set; }
    public string Symbol { get; set; }
    public string OrderType { get; set; }
    public int Quantity { get; set; }
    public decimal PricePerShare { get; set; }
    public decimal TotalAmount { get; set; }
    public string Status { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? ExecutedAt { get; set; }
    public string? ErrorMessage { get; set; }
}
