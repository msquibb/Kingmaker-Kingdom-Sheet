using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace KingmakerKingdomSheet.ApiService.Data;

public sealed class KingmakerDatabaseHealthCheck(KingmakerDbContext dbContext) : IHealthCheck
{
    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        if (!await dbContext.Database.CanConnectAsync(cancellationToken))
        {
            return HealthCheckResult.Unhealthy("The SQLite development database is not reachable.");
        }

        var hasReferenceData = await dbContext.ClaimStatuses.AnyAsync(cancellationToken);

        return hasReferenceData
            ? HealthCheckResult.Healthy("SQLite development database is reachable and seeded.")
            : HealthCheckResult.Degraded("SQLite development database is reachable, but reference data has not been seeded yet.");
    }
}
