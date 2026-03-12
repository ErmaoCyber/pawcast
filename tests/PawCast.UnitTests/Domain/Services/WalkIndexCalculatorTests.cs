using PawCast.Domain.Enums;
using PawCast.Domain.Services;
using PawCast.Domain.ValueObjects;

namespace PawCast.UnitTests.Domain.Services;

public class WalkIndexCalculatorTests
{
    private readonly WalkIndexCalculator _calculator = new();

    [Fact]
    public void Calculate_ShouldReturnExcellent_WhenConditionsAreIdeal()
    {
        var input = new WalkIndexInput(
            TemperatureC: 18,
            WindKph: 10,
            PrecipitationProbability: 5,
            UvIndex: 2,
            Pm25: 5);

        var result = _calculator.Calculate(input);

        Assert.Equal(100, result.Score);
        Assert.Equal(WalkIndexGrade.Excellent, result.Grade);
        Assert.Empty(result.Reasons);
    }

    [Fact]
    public void Calculate_ShouldReturnGood_WithModeratePenalties()
    {
        var input = new WalkIndexInput(
            TemperatureC: 21,
            WindKph: 25,
            PrecipitationProbability: 30,
            UvIndex: 7,
            Pm25: 10);

        var result = _calculator.Calculate(input);

        Assert.Equal(80, result.Score);
        Assert.Equal(WalkIndexGrade.Good, result.Grade);
        Assert.Contains("Moderate wind reduced score by 5", result.Reasons);
        Assert.Contains("Some rain risk reduced score by 5", result.Reasons);
        Assert.Contains("High UV reduced score by 10", result.Reasons);
    }

    [Fact]
    public void Calculate_ShouldReturnAvoid_WhenConditionsAreSevere()
    {
        var input = new WalkIndexInput(
            TemperatureC: 34,
            WindKph: 45,
            PrecipitationProbability: 80,
            UvIndex: 11,
            Pm25: 60);

        var result = _calculator.Calculate(input);

        Assert.Equal(0, result.Score);
        Assert.Equal(WalkIndexGrade.Avoid, result.Grade);
        Assert.Contains("Extreme heat reduced score by 35", result.Reasons);
        Assert.Contains("Very strong wind reduced score by 20", result.Reasons);
        Assert.Contains("Very high rain chance reduced score by 20", result.Reasons);
        Assert.Contains("Extreme UV reduced score by 25", result.Reasons);
        Assert.Contains("Very poor air quality reduced score by 30", result.Reasons);
    }

    [Fact]
    public void Calculate_ShouldClampScoreToZero_WhenPenaltyExceedsHundred()
    {
        var input = new WalkIndexInput(
            TemperatureC: 40,
            WindKph: 60,
            PrecipitationProbability: 100,
            UvIndex: 14,
            Pm25: 100);

        var result = _calculator.Calculate(input);

        Assert.Equal(0, result.Score);
        Assert.Equal(WalkIndexGrade.Avoid, result.Grade);
    }
}