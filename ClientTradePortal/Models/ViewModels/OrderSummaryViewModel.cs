namespace ClientTradePortal.Models.ViewModels;
public class OrderSummaryViewModel
{
    public string Symbol { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public decimal PricePerShare { get; set; }
    public decimal TotalAmount { get; set; }
    public decimal RemainingBalance { get; set; }
}
