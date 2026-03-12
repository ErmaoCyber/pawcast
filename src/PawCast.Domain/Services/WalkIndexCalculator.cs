using PawCast.Domain.Enums;
using PawCast.Domain.ValueObjects;

namespace PawCast.Domain.Services;

public class WalkIndexCalculator
{
    public WalkIndexResult Calculate(WalkIndexInput input)
    {
        var reasons = new List<string>();
        var totalPenalty = 0;

        totalPenalty += CalculateTemperaturePenalty(input.TemperatureC, reasons);
        totalPenalty += CalculateWindPenalty(input.WindKph, reasons);
        totalPenalty += CalculatePrecipitationPenalty(input.PrecipitationProbability, reasons);
        totalPenalty += CalculateUvPenalty(input.UvIndex, reasons);
        totalPenalty += CalculatePm25Penalty(input.Pm25, reasons);

        var score = Math.Clamp(100 - totalPenalty, 0, 100);
        var grade = DetermineGrade(score);

        return new WalkIndexResult(score, grade, reasons);
    }

    private static int CalculateTemperaturePenalty(decimal temperatureC, List<string> reasons)
    {
        if (temperatureC < 0)
        {
            reasons.Add("Extreme cold reduced score by 35");
            return 35;
        }

        if (temperatureC < 5)
        {
            reasons.Add("Cold temperature reduced score by 20");
            return 20;
        }

        if (temperatureC < 10)
        {
            reasons.Add("Cool temperature reduced score by 8");
            return 8;
        }

        if (temperatureC <= 24)
        {
            return 0;
        }

        if (temperatureC <= 28)
        {
            reasons.Add("Warm temperature reduced score by 8");
            return 8;
        }

        if (temperatureC <= 32)
        {
            reasons.Add("Hot temperature reduced score by 20");
            return 20;
        }

        reasons.Add("Extreme heat reduced score by 35");
        return 35;
    }

    private static int CalculateWindPenalty(decimal windKph, List<string> reasons)
    {
        if (windKph < 20)
        {
            return 0;
        }

        if (windKph < 30)
        {
            reasons.Add("Moderate wind reduced score by 5");
            return 5;
        }

        if (windKph < 40)
        {
            reasons.Add("Strong wind reduced score by 12");
            return 12;
        }

        reasons.Add("Very strong wind reduced score by 20");
        return 20;
    }

    private static int CalculatePrecipitationPenalty(int precipitationProbability, List<string> reasons)
    {
        if (precipitationProbability < 20)
        {
            return 0;
        }

        if (precipitationProbability < 50)
        {
            reasons.Add("Some rain risk reduced score by 5");
            return 5;
        }

        if (precipitationProbability < 70)
        {
            reasons.Add("High rain chance reduced score by 12");
            return 12;
        }

        reasons.Add("Very high rain chance reduced score by 20");
        return 20;
    }

    private static int CalculateUvPenalty(decimal uvIndex, List<string> reasons)
    {
        if (uvIndex < 3)
        {
            return 0;
        }

        if (uvIndex < 6)
        {
            reasons.Add("Moderate UV reduced score by 4");
            return 4;
        }

        if (uvIndex < 8)
        {
            reasons.Add("High UV reduced score by 10");
            return 10;
        }

        if (uvIndex < 11)
        {
            reasons.Add("Very high UV reduced score by 18");
            return 18;
        }

        reasons.Add("Extreme UV reduced score by 25");
        return 25;
    }

    private static int CalculatePm25Penalty(decimal pm25, List<string> reasons)
    {
        if (pm25 < 12)
        {
            return 0;
        }

        if (pm25 < 25)
        {
            reasons.Add("Moderate PM2.5 reduced score by 5");
            return 5;
        }

        if (pm25 < 35)
        {
            reasons.Add("Elevated PM2.5 reduced score by 12");
            return 12;
        }

        if (pm25 < 55)
        {
            reasons.Add("Poor air quality reduced score by 20");
            return 20;
        }

        reasons.Add("Very poor air quality reduced score by 30");
        return 30;
    }

    private static WalkIndexGrade DetermineGrade(int score)
    {
        if (score >= 85) return WalkIndexGrade.Excellent;
        if (score >= 70) return WalkIndexGrade.Good;
        if (score >= 50) return WalkIndexGrade.Fair;
        if (score >= 30) return WalkIndexGrade.Poor;
        return WalkIndexGrade.Avoid;
    }
}