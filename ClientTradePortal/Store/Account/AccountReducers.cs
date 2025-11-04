public static class AccountReducers
{
    [ReducerMethod]
    public static AccountState ReduceLoadAccountAction(
        AccountState state,
        LoadAccountAction action)
    {
        return state with
        {
            IsLoading = true,
            ErrorMessage = null
        };
    }

    [ReducerMethod]
    public static AccountState ReduceLoadAccountSuccessAction(
        AccountState state,
        LoadAccountSuccessAction action)
    {
        return state with
        {
            CurrentAccount = action.Account,
            IsLoading = false,
            ErrorMessage = null
        };
    }

    [ReducerMethod]
    public static AccountState ReduceLoadAccountFailureAction(
        AccountState state,
        LoadAccountFailureAction action)
    {
        return state with
        {
            IsLoading = false,
            ErrorMessage = action.ErrorMessage
        };
    }

    [ReducerMethod]
    public static AccountState ReduceClearAccountAction(
        AccountState state,
        ClearAccountAction action)
    {
        return new AccountState();
    }
}