// Models/DTO/StockPositionResponse.cs
namespace ClientTradePortal.Models.DTO;

public class StockPositionResponse
{
    public string Symbol { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public decimal AveragePrice { get; set; }
    public decimal CurrentPrice { get; set; }

    // Calculated properties
    public decimal TotalValue => Quantity * CurrentPrice;
    public decimal TotalCost => Quantity * AveragePrice;  // ← Add this
    public decimal ProfitLoss => TotalValue - TotalCost;
    public decimal ProfitLossPercentage =>
        TotalCost > 0 ? ((TotalValue - TotalCost) / TotalCost) * 100 : 0;
}