using Microsoft.EntityFrameworkCore;
using PawCast.Domain.Entities;

namespace PawCast.Infrastructure.Persistence;

public class PawCastDbContext : DbContext
{
    public PawCastDbContext(DbContextOptions<PawCastDbContext> options)
        : base(options)
    {
    }

    public DbSet<FetchRun> FetchRuns => Set<FetchRun>();
    public DbSet<ForecastSnapshot> ForecastSnapshots => Set<ForecastSnapshot>();
    public DbSet<WalkIndexResultRecord> WalkIndexResults => Set<WalkIndexResultRecord>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(PawCastDbContext).Assembly);
    }
}