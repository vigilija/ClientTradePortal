namespace ClientTradePortal.Models.DTO;

public enum OrderType
{
    Buy,
    Sell
}

public enum OrderStatus
{
    Pending,
    Executed,
    Failed,
    Cancelled
}