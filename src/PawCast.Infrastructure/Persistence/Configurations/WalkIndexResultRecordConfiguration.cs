using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PawCast.Domain.Entities;

namespace PawCast.Infrastructure.Persistence.Configurations;

public class WalkIndexResultRecordConfiguration : IEntityTypeConfiguration<WalkIndexResultRecord>
{
    public void Configure(EntityTypeBuilder<WalkIndexResultRecord> builder)
    {
        builder.ToTable("walk_index_results");

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

        builder.Property(x => x.GeneratedAtUtc)
            .HasColumnName("generated_at_utc")
            .IsRequired();

        builder.Property(x => x.WalkIndex)
            .HasColumnName("walk_index")
            .IsRequired();

        builder.Property(x => x.Grade)
            .HasColumnName("grade")
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(x => x.ReasonsJson)
            .HasColumnName("reasons_json")
            .HasColumnType("jsonb")
            .IsRequired();

        builder.Property(x => x.TemperatureC)
            .HasColumnName("temperature_c")
            .HasPrecision(6, 2)
            .IsRequired();

        builder.Property(x => x.WindKph)
            .HasColumnName("wind_kph")
            .HasPrecision(6, 2)
            .IsRequired();

        builder.Property(x => x.PrecipitationProbability)
            .HasColumnName("precipitation_probability")
            .IsRequired();

        builder.Property(x => x.UvIndex)
            .HasColumnName("uv_index")
            .HasPrecision(6, 2)
            .IsRequired();

        builder.Property(x => x.Pm25)
            .HasColumnName("pm25")
            .HasPrecision(6, 2)
            .IsRequired();

        builder.HasIndex(x => new { x.Latitude, x.Longitude, x.ForecastTimeUtc })
            .IsUnique();
    }
}