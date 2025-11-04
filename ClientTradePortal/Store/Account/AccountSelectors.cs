using ClientTradePortal.Models.DTO;
using ClientTradePortal.Store.Account;
using Fluxor;


public static class AccountSelectors
{
    //[FeatureSelector]
    public static AccountState SelectAccountState(IState<AccountState> state)
        => state.Value;

    public static AccountResponse? SelectCurrentAccount(AccountState state)
        => state.CurrentAccount;

    public static bool SelectIsLoading(AccountState state)
        => state.IsLoading;

    public static decimal SelectCashBalance(AccountState state)
        => state.CurrentAccount?.CashBalance ?? 0;
}