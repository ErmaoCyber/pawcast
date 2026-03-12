using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PawCast.Domain.Entities;

namespace PawCast.Infrastructure.Persistence.Configurations;

public class FetchRunConfiguration : IEntityTypeConfiguration<FetchRun>
{
    public void Configure(EntityTypeBuilder<FetchRun> builder)
    {
        builder.ToTable("fetch_runs");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .HasColumnName("id");

        builder.Property(x => x.StartedAtUtc)
            .HasColumnName("started_at_utc")
            .IsRequired();

        builder.Property(x => x.EndedAtUtc)
            .HasColumnName("ended_at_utc");

        builder.Property(x => x.Status)
            .HasColumnName("status")
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(x => x.ErrorMessage)
            .HasColumnName("error_message")
            .HasMaxLength(4000);

        builder.Property(x => x.DurationMs)
            .HasColumnName("duration_ms");
    }
}