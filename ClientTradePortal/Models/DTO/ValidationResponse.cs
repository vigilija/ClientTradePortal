namespace ClientTradePortal.Models.DTO;

public class ValidationResponse
{
    public bool IsValid { get; set; }
    public List<string> Errors { get; set; } = new();
    public decimal? CurrentPrice { get; set; }
    public decimal? TotalAmount { get; set; }
}
