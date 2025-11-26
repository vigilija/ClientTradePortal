using Blazored.LocalStorage;

namespace ClientTradePortal.Store.Account;

public class AccountEffects
{
    private readonly IAccountService _accountService;
    private readonly ILogger<AccountEffects> _logger;
    private readonly ILocalStorageService _localStorage;
    private const string ACCOUNT_STORAGE_KEY = "AccountState_CurrentAccount";

    public AccountEffects(
        IAccountService accountService,
        ILogger<AccountEffects> logger,
        ILocalStorageService localStorage)
    {
        _accountService = accountService;
        _logger = logger;
        _localStorage = localStorage;
    }

    [EffectMethod]
    public async Task HandleLoadAccountAction(
        LoadAccountAction action,
        IDispatcher dispatcher)
    {
        Console.WriteLine($"EFFECT TRIGGERED AccountEffects: HandleLoadAccountAction called for {action.AccountId}");
        _logger.LogInformation("Loading account {AccountId}", action.AccountId);

        try
        {
            var account = await _accountService.GetAccountAsync(action.AccountId);

            // Save to localStorage for persistence
            await _localStorage.SetItemAsync(ACCOUNT_STORAGE_KEY, account);
            Console.WriteLine("AccountEffects: Saved account to localStorage");

            dispatcher.Dispatch(new LoadAccountSuccessAction(account));
        }
        catch (Exception ex)
        {
            Console.WriteLine($"AccountEffects: Error loading account: {ex.Message}");
            _logger.LogError(ex, "Failed to load account");
            dispatcher.Dispatch(new LoadAccountFailureAction(ex.Message));
        }
    }
}