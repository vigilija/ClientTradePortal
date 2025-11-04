namespace ClientTradePortal.Store.Account;
public record LoadAccountAction(Guid AccountId);
public record LoadAccountSuccessAction(AccountResponse Account);
public record LoadAccountFailureAction(string ErrorMessage);
public record ClearAccountAction();