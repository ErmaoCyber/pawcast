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

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(PawCastDbContext).Assembly);
    }
}