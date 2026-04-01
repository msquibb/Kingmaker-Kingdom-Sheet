using KingmakerKingdomSheet.ApiService.Data;
using Microsoft.EntityFrameworkCore;

namespace KingmakerKingdomSheet.ApiService.Auth;

public interface IKingdomAuthorizationService
{
    Task<bool> HasPermissionAsync(Guid userAccountId, Guid kingdomId, string permissionName, CancellationToken ct = default);
    Task<IReadOnlyList<string>> GetEffectivePermissionsAsync(Guid userAccountId, Guid kingdomId, CancellationToken ct = default);
}

public sealed class KingdomAuthorizationService(KingmakerDbContext dbContext) : IKingdomAuthorizationService
{
    public async Task<bool> HasPermissionAsync(Guid userAccountId, Guid kingdomId, string permissionName, CancellationToken ct = default)
    {
        var effectivePermissions = await GetEffectivePermissionsAsync(userAccountId, kingdomId, ct);
        return effectivePermissions.Contains(permissionName);
    }

    public async Task<IReadOnlyList<string>> GetEffectivePermissionsAsync(Guid userAccountId, Guid kingdomId, CancellationToken ct = default)
    {
        var participant = await dbContext.KingdomParticipants
            .Include(p => p.Roles)
                .ThenInclude(r => r.RoleType)
                .ThenInclude(rt => rt.DefaultPermissions)
                .ThenInclude(dp => dp.PermissionType)
            .Include(p => p.Permissions)
                .ThenInclude(p => p.PermissionType)
            .FirstOrDefaultAsync(p => p.UserAccountId == userAccountId && p.KingdomId == kingdomId && p.IsActive, ct);

        if (participant == null)
            return Array.Empty<string>();

        var roleDefaultPermissions = participant.Roles
            .SelectMany(r => r.RoleType.DefaultPermissions)
            .Select(dp => dp.PermissionType.Name)
            .Distinct()
            .ToHashSet();

        var explicitGrants = participant.Permissions
            .Where(p => p.PermissionState == "G")
            .Select(p => p.PermissionType.Name)
            .ToHashSet();

        var explicitDenies = participant.Permissions
            .Where(p => p.PermissionState == "D")
            .Select(p => p.PermissionType.Name)
            .ToHashSet();

        foreach (var grant in explicitGrants)
        {
            roleDefaultPermissions.Add(grant);
        }

        foreach (var deny in explicitDenies)
        {
            roleDefaultPermissions.Remove(deny);
        }

        return roleDefaultPermissions.ToList();
    }
}
