using Microsoft.EntityFrameworkCore;

namespace Genzy.Base.Data;

public class BaseDbContext<T>(DbContextOptions<T> options) : DbContext(options)
    where T : DbContext
{
    protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
    {
        // Map ulong to numeric(20,0) since Postgres has no unsigned types
        configurationBuilder.Properties<ulong>()
            .HaveConversion<decimal>()
            .HaveMaxLength(20);
    }
}
