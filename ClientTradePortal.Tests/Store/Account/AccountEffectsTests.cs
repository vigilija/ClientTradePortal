using Fluxor;

namespace ClientTradePortal.Tests.Store.Account;

public class AccountEffectsTests
{
    private readonly Mock<IAccountService> _mockAccountService;
    private readonly Mock<ILogger<AccountEffects>> _mockLogger;
    private readonly Mock<IDispatcher> _mockDispatcher;
    private readonly AccountEffects _effects;

    public AccountEffectsTests()
    {
        _mockAccountService = new Mock<IAccountService>();
        _mockLogger = new Mock<ILogger<AccountEffects>>();
        _mockDispatcher = new Mock<IDispatcher>();
        _effects = new AccountEffects(
            _mockAccountService.Object,
            _mockLogger.Object);
    }

    [Fact]
    public async Task HandleLoadAccountAction_WhenSuccessful_DispatchesSuccessAction()
    {
        // Arrange
        var accountId = Guid.NewGuid();
        var action = new LoadAccountAction(accountId);
        var accountData = new AccountResponse
        {
            AccountId = accountId,
            ClientId = Guid.NewGuid(),
            CashBalance = 10000m,
            Currency = "EUR",
            Positions = new List<StockPositionResponse>
            {
                new StockPositionResponse
                {
                    Symbol = "AAPL",
                    Quantity = 10,
                    AveragePrice = 150m,
                    CurrentPrice = 175m
                }
            }
        };
        _mockAccountService.Setup(x => x.GetAccountAsync(accountId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(accountData);

        // Act
        await _effects.HandleLoadAccountAction(action, _mockDispatcher.Object);

        // Assert
        _mockAccountService.Verify(x => x.GetAccountAsync(accountId, It.IsAny<CancellationToken>()), Times.Once);
        _mockDispatcher.Verify(x => x.Dispatch(It.Is<LoadAccountSuccessAction>(
            a => a.Account.AccountId == accountId &&
                 a.Account.CashBalance == 10000m &&
                 a.Account.Positions.Count == 1)), Times.Once);
    }

    [Fact]
    public async Task HandleLoadAccountAction_WhenFails_DispatchesFailureAction()
    {
        // Arrange
        var accountId = Guid.NewGuid();
        var action = new LoadAccountAction(accountId);
        var exception = new ApplicationException("Account not found");
        _mockAccountService.Setup(x => x.GetAccountAsync(accountId, It.IsAny<CancellationToken>()))
            .ThrowsAsync(exception);

        // Act
        await _effects.HandleLoadAccountAction(action, _mockDispatcher.Object);

        // Assert
        _mockAccountService.Verify(x => x.GetAccountAsync(accountId, It.IsAny<CancellationToken>()), Times.Once);
        _mockDispatcher.Verify(x => x.Dispatch(It.Is<LoadAccountFailureAction>(
            a => a.ErrorMessage == "Account not found")), Times.Once);
    }

    [Fact]
    public async Task HandleLoadAccountAction_WhenServiceThrowsException_LogsError()
    {
        // Arrange
        var accountId = Guid.NewGuid();
        var action = new LoadAccountAction(accountId);
        var exception = new ApplicationException("Network error");
        _mockAccountService.Setup(x => x.GetAccountAsync(accountId, It.IsAny<CancellationToken>()))
            .ThrowsAsync(exception);

        // Act
        await _effects.HandleLoadAccountAction(action, _mockDispatcher.Object);

        // Assert
        _mockLogger.Verify(
            x => x.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => true),
                It.IsAny<Exception>(),
                It.Is<Func<It.IsAnyType, Exception?, string>>((v, t) => true)),
            Times.Once);
    }

    [Fact]
    public async Task HandleLoadAccountAction_WhenSuccessful_LogsInformation()
    {
        // Arrange
        var accountId = Guid.NewGuid();
        var action = new LoadAccountAction(accountId);
        var accountData = new AccountResponse
        {
            AccountId = accountId,
            ClientId = Guid.NewGuid(),
            CashBalance = 5000m,
            Currency = "EUR"
        };
        _mockAccountService.Setup(x => x.GetAccountAsync(accountId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(accountData);

        // Act
        await _effects.HandleLoadAccountAction(action, _mockDispatcher.Object);

        // Assert
        _mockLogger.Verify(
            x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => true),
                It.IsAny<Exception>(),
                It.Is<Func<It.IsAnyType, Exception?, string>>((v, t) => true)),
            Times.Once);
    }
}
