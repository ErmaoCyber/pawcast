using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PawCast.Domain.Entities;

namespace PawCast.Infrastructure.Persistence.Configurations;

public class ForecastSnapshotConfiguration : IEntityTypeConfiguration<ForecastSnapshot>
{
    public void Configure(EntityTypeBuilder<ForecastSnapshot> builder)
    {
        builder.ToTable("forecast_snapshots");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id).HasColumnName("id");

        builder.Property(x => x.Latitude)
            .HasColumnName("latitude")
            .HasPrecision(9, 6)
            .IsRequired();

        builder.Property(x => x.Longitude)
            .HasColumnName("longitude")
            .HasPrecision(9, 6)
            .IsRequired();

        builder.Property(x => x.ForecastTimeUtc)
            .HasColumnName("forecast_time_utc")
            .IsRequired();

        builder.Property(x => x.FetchedAtUtc)
            .HasColumnName("fetched_at_utc")
            .IsRequired();

        builder.Property(x => x.Source)
            .HasColumnName("source")
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(x => x.RawWeatherJson)
            .HasColumnName("raw_weather_json")
            .HasColumnType("jsonb")
            .IsRequired();

        builder.Property(x => x.RawAirQualityJson)
            .HasColumnName("raw_air_quality_json")
            .HasColumnType("jsonb")
            .IsRequired();

        builder.HasIndex(x => new { x.Latitude, x.Longitude, x.ForecastTimeUtc });
    }
}