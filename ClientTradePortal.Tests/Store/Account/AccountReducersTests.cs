namespace ClientTradePortal.Tests.Store.Account;

public class AccountReducersTests
{
    [Fact]
    public void ReduceLoadAccountAction_SetsIsLoadingAndClearsError()
    {
        // Arrange
        var initialState = new AccountState
        {
            IsLoading = false,
            ErrorMessage = "Previous error"
        };
        var action = new LoadAccountAction(Guid.NewGuid());

        // Act
        var newState = AccountReducers.ReduceLoadAccountAction(initialState, action);

        // Assert
        newState.Should().NotBeSameAs(initialState);
        newState.IsLoading.Should().BeTrue();
        newState.ErrorMessage.Should().BeNull();
    }

    [Fact]
    public void ReduceLoadAccountSuccessAction_SetsAccountAndClearsLoadingAndError()
    {
        // Arrange
        var initialState = new AccountState
        {
            IsLoading = true,
            CurrentAccount = null,
            ErrorMessage = null
        };
        var accountData = new AccountResponse
        {
            AccountId = Guid.NewGuid(),
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
        var action = new LoadAccountSuccessAction(accountData);

        // Act
        var newState = AccountReducers.ReduceLoadAccountSuccessAction(initialState, action);

        // Assert
        newState.Should().NotBeSameAs(initialState);
        newState.IsLoading.Should().BeFalse();
        newState.CurrentAccount.Should().NotBeNull();
        newState.CurrentAccount!.AccountId.Should().Be(accountData.AccountId);
        newState.CurrentAccount.CashBalance.Should().Be(10000m);
        newState.CurrentAccount.Positions.Should().HaveCount(1);
        newState.ErrorMessage.Should().BeNull();
    }

    [Fact]
    public void ReduceLoadAccountFailureAction_SetsErrorAndClearsLoading()
    {
        // Arrange
        var initialState = new AccountState
        {
            IsLoading = true,
            CurrentAccount = null,
            ErrorMessage = null
        };
        var action = new LoadAccountFailureAction("Failed to load account");

        // Act
        var newState = AccountReducers.ReduceLoadAccountFailureAction(initialState, action);

        // Assert
        newState.Should().NotBeSameAs(initialState);
        newState.IsLoading.Should().BeFalse();
        newState.ErrorMessage.Should().Be("Failed to load account");
    }

    [Fact]
    public void ReduceClearAccountAction_ResetsStateToDefault()
    {
        // Arrange
        var initialState = new AccountState
        {
            CurrentAccount = new AccountResponse
            {
                AccountId = Guid.NewGuid(),
                ClientId = Guid.NewGuid(),
                CashBalance = 5000m,
                Currency = "EUR"
            },
            IsLoading = false,
            ErrorMessage = "Some error"
        };
        var action = new ClearAccountAction();

        // Act
        var newState = AccountReducers.ReduceClearAccountAction(initialState, action);

        // Assert
        newState.Should().NotBeSameAs(initialState);
        newState.CurrentAccount.Should().BeNull();
        newState.IsLoading.Should().BeFalse();
        newState.ErrorMessage.Should().BeNull();
    }

    [Fact]
    public void Reducers_ShouldReturnNewStateInstance_NotMutateOriginal()
    {
        // Arrange
        var originalState = new AccountState
        {
            IsLoading = false,
            CurrentAccount = null
        };
        var action = new LoadAccountAction(Guid.NewGuid());

        // Act
        var newState = AccountReducers.ReduceLoadAccountAction(originalState, action);

        // Assert
        newState.Should().NotBeSameAs(originalState);
        originalState.IsLoading.Should().BeFalse();
        newState.IsLoading.Should().BeTrue();
    }

    [Fact]
    public void ReduceLoadAccountFailureAction_PreservesCurrentAccount()
    {
        // Arrange
        var accountData = new AccountResponse
        {
            AccountId = Guid.NewGuid(),
            ClientId = Guid.NewGuid(),
            CashBalance = 5000m,
            Currency = "EUR"
        };
        var initialState = new AccountState
        {
            CurrentAccount = accountData,
            IsLoading = true
        };
        var action = new LoadAccountFailureAction("Network error");

        // Act
        var newState = AccountReducers.ReduceLoadAccountFailureAction(initialState, action);

        // Assert
        newState.Should().NotBeSameAs(initialState);
        newState.CurrentAccount.Should().Be(accountData); // Previous account data preserved
        newState.IsLoading.Should().BeFalse();
        newState.ErrorMessage.Should().Be("Network error");
    }
}
