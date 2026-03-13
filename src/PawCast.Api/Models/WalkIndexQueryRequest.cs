using System.ComponentModel.DataAnnotations;

namespace PawCast.Api.Models;

public class WalkIndexCurrentQueryRequest
{
    [Range(-90, 90)]
    public decimal Lat { get; set; }

    [Range(-180, 180)]
    public decimal Lon { get; set; }
}

public class WalkIndexForecastQueryRequest
{
    [Range(-90, 90)]
    public decimal Lat { get; set; }

    [Range(-180, 180)]
    public decimal Lon { get; set; }

    [Range(1, 72)]
    public int Hours { get; set; } = 24;
}