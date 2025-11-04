namespace ClientTradePortal.Models.DTO;

// Models/DTOs/Responses.cs (shared with API)
public class AccountResponse
{
    public Guid AccountId { get; set; }
    public Guid ClientId { get; set; }
    public decimal CashBalance { get; set; }
    public string Currency { get; set; } = "EUR";
    public List<StockPositionResponse> Positions { get; set; } = new();
}
