using ClientTradePortal.Models.DTO;

public record UpdateOrderQuantityAction(int Quantity);
public record FetchStockPriceAction(string Symbol);
public record FetchStockPriceSuccessAction(decimal Price);
public record FetchStockPriceFailureAction(string ErrorMessage);
public record ValidateOrderAction(Guid AccountId);
public record ValidateOrderSuccessAction(ValidationResponse Result);
public record ValidateOrderFailureAction(string ErrorMessage);
public record ExecuteOrderAction(Guid AccountId);
public record ExecuteOrderSuccessAction(OrderResponse Order);
public record ExecuteOrderFailureAction(string ErrorMessage);
public record LoadOrderHistoryAction(Guid AccountId);
public record LoadOrderHistorySuccessAction(List<OrderResponse> Orders);
public record ResetOrderAction();