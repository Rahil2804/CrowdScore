using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace CrowdScore.Api.Data;

public sealed class CrowdScoreDbContextFactory : IDesignTimeDbContextFactory<CrowdScoreDbContext>
{
    public CrowdScoreDbContext CreateDbContext(string[] args)
    {
        var configuration = new ConfigurationBuilder()
            .SetBasePath(AppContext.BaseDirectory)
            .AddJsonFile("appsettings.Development.json")
            .AddEnvironmentVariables()
            .Build();
        var connectionString = configuration.GetConnectionString("CrowdScore")
            ?? throw new InvalidOperationException(
                "The connection string 'ConnectionStrings:CrowdScore' is not configured.");

        var options = new DbContextOptionsBuilder<CrowdScoreDbContext>()
            .UseNpgsql(connectionString)
            .Options;

        return new CrowdScoreDbContext(options);
    }
}
