namespace ClientTradePortal.Tests.Models.DTO;

public class StockPositionResponseTests
{
    [Fact]
    public void TotalValue_CalculatesCorrectly()
    {
        // Arrange
        var position = new StockPositionResponse
        {
            Symbol = "AAPL",
            Quantity = 10,
            AveragePrice = 150m,
            CurrentPrice = 175m
        };

        // Act
        var totalValue = position.TotalValue;

        // Assert
        totalValue.Should().Be(1750m);
    }

    [Fact]
    public void TotalCost_CalculatesCorrectly()
    {
        // Arrange
        var position = new StockPositionResponse
        {
            Symbol = "AAPL",
            Quantity = 10,
            AveragePrice = 150m,
            CurrentPrice = 175m
        };

        // Act
        var totalCost = position.TotalCost;

        // Assert
        totalCost.Should().Be(1500m);
    }

    [Fact]
    public void ProfitLoss_WhenPriceIncreases_ReturnsPositiveValue()
    {
        // Arrange
        var position = new StockPositionResponse
        {
            Symbol = "AAPL",
            Quantity = 10,
            AveragePrice = 150m,
            CurrentPrice = 175m
        };

        // Act
        var profitLoss = position.ProfitLoss;

        // Assert
        profitLoss.Should().Be(250m); // (175 - 150) * 10
    }

    [Fact]
    public void ProfitLoss_WhenPriceDecreases_ReturnsNegativeValue()
    {
        // Arrange
        var position = new StockPositionResponse
        {
            Symbol = "AAPL",
            Quantity = 10,
            AveragePrice = 150m,
            CurrentPrice = 125m
        };

        // Act
        var profitLoss = position.ProfitLoss;

        // Assert
        profitLoss.Should().Be(-250m); // (125 - 150) * 10
    }

    [Fact]
    public void ProfitLoss_WhenPriceUnchanged_ReturnsZero()
    {
        // Arrange
        var position = new StockPositionResponse
        {
            Symbol = "AAPL",
            Quantity = 10,
            AveragePrice = 150m,
            CurrentPrice = 150m
        };

        // Act
        var profitLoss = position.ProfitLoss;

        // Assert
        profitLoss.Should().Be(0m);
    }

    [Fact]
    public void ProfitLossPercentage_WhenPriceIncreases_ReturnsPositivePercentage()
    {
        // Arrange
        var position = new StockPositionResponse
        {
            Symbol = "AAPL",
            Quantity = 10,
            AveragePrice = 100m,
            CurrentPrice = 150m
        };

        // Act
        var profitLossPercentage = position.ProfitLossPercentage;

        // Assert
        profitLossPercentage.Should().Be(50m); // ((150 - 100) / 100) * 100 = 50%
    }

    [Fact]
    public void ProfitLossPercentage_WhenPriceDecreases_ReturnsNegativePercentage()
    {
        // Arrange
        var position = new StockPositionResponse
        {
            Symbol = "AAPL",
            Quantity = 10,
            AveragePrice = 100m,
            CurrentPrice = 75m
        };

        // Act
        var profitLossPercentage = position.ProfitLossPercentage;

        // Assert
        profitLossPercentage.Should().Be(-25m); // ((75 - 100) / 100) * 100 = -25%
    }

    [Fact]
    public void ProfitLossPercentage_WhenTotalCostIsZero_ReturnsZero()
    {
        // Arrange
        var position = new StockPositionResponse
        {
            Symbol = "AAPL",
            Quantity = 0,
            AveragePrice = 100m,
            CurrentPrice = 150m
        };

        // Act
        var profitLossPercentage = position.ProfitLossPercentage;

        // Assert
        profitLossPercentage.Should().Be(0m);
    }

    [Fact]
    public void ProfitLossPercentage_WhenAveragePriceIsZero_ReturnsZero()
    {
        // Arrange
        var position = new StockPositionResponse
        {
            Symbol = "AAPL",
            Quantity = 10,
            AveragePrice = 0m,
            CurrentPrice = 150m
        };

        // Act
        var profitLossPercentage = position.ProfitLossPercentage;

        // Assert
        profitLossPercentage.Should().Be(0m);
    }

    [Fact]
    public void AllCalculations_WithDecimalValues_CalculateAccurately()
    {
        // Arrange
        var position = new StockPositionResponse
        {
            Symbol = "AAPL",
            Quantity = 7,
            AveragePrice = 123.45m,
            CurrentPrice = 156.78m
        };

        // Act & Assert
        position.TotalValue.Should().Be(1097.46m); // 7 * 156.78
        position.TotalCost.Should().Be(864.15m); // 7 * 123.45
        position.ProfitLoss.Should().Be(233.31m); // 1097.46 - 864.15
        position.ProfitLossPercentage.Should().BeApproximately(27.0m, 0.1m); // ((1097.46 - 864.15) / 864.15) * 100
    }
}
