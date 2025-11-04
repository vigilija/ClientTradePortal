namespace ClientTradePortal.Models.DTO;
 public class AccountBalanceResponse
    {
        public Guid AccountId { get; set; }
        public Guid ClientId { get; set; }
        public decimal CashBalance { get; set; }
        public string Currency { get; set; } = "EUR";
        public List<StockPositionResponse> Positions { get; set; } = new();
    }