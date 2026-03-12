using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace PawCast.Infrastructure.Persistence;

public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<PawCastDbContext>
{
    public PawCastDbContext CreateDbContext(string[] args)
    {
        var connectionString =
            "Host=localhost;Port=5433;Database=pawcast;Username=pawcast;Password=pawcast_dev_password";

        var optionsBuilder = new DbContextOptionsBuilder<PawCastDbContext>();
        optionsBuilder.UseNpgsql(connectionString);

        return new PawCastDbContext(optionsBuilder.Options);
    }
}