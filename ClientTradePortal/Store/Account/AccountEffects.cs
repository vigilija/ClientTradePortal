using ClientTradePortal.Services.Account;
using ClientTradePortal.Store.Account;
using Fluxor;

namespace ClientTradePortal.Store.Account;

public class AccountEffects
{
    private readonly IAccountService _accountService;
    private readonly ILogger<AccountEffects> _logger;

    public AccountEffects(
        IAccountService accountService,
        ILogger<AccountEffects> logger)
    {
        _accountService = accountService;
        _logger = logger;
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
            Console.WriteLine($"AccountEffects: Account loaded successfully. Balance: {account.Positions.FirstOrDefault().Symbol}");
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