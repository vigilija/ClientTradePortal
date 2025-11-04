namespace ClientTradePortal.Store.Account;

[FeatureState]
public record AccountState
{
    public AccountResponse? CurrentAccount { get; init; }
    public bool IsLoading { get; init; }
    public string? ErrorMessage { get; init; }
}
