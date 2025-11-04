namespace ClientTradePortal.Models.DTO
{
    public class ValidationRequest
    {
        public Guid AccountId { get; set; }
        public string Symbol { get; set; }
        public int Quantity { get; set; }
        public decimal EstimatedPrice { get; set; }
}
}
