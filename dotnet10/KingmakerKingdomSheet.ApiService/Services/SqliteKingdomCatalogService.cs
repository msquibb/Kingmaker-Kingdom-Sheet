using KingmakerKingdomSheet.ApiService.Data;
using KingmakerKingdomSheet.Application.Contracts;
using KingmakerKingdomSheet.Shared.DTOs;
using Microsoft.EntityFrameworkCore;

namespace KingmakerKingdomSheet.ApiService.Services;

internal sealed class SqliteKingdomCatalogService(KingmakerDbContext dbContext) : IKingdomCatalogService
{
    public async Task<IReadOnlyList<KingdomSummaryDto>> ListAsync(CancellationToken cancellationToken = default)
    {
        return await dbContext.Kingdoms
            .AsNoTracking()
            .Where(kingdom => !kingdom.IsArchived)
            .OrderBy(kingdom => kingdom.Name)
            .Select(kingdom => new KingdomSummaryDto(
                kingdom.KingdomId,
                kingdom.Name,
                0,
                kingdom.Hexes.Count(),
                0,
                0,
                kingdom.Participants.Count(participant => participant.IsActive),
                kingdom.Hexes.Count(hex => hex.ClaimStatus.IsClaimed),
                kingdom.Settlements.Count()))
            .ToListAsync(cancellationToken);
    }

    public async Task<KingdomDetailsDto?> GetAsync(Guid kingdomId, CancellationToken cancellationToken = default)
    {
        var kingdom = await dbContext.Kingdoms
            .AsNoTracking()
            .Include(item => item.Participants)
                .ThenInclude(item => item.UserAccount)
            .Include(item => item.Participants)
                .ThenInclude(item => item.Roles)
                    .ThenInclude(item => item.RoleType)
            .Include(item => item.Hexes)
                .ThenInclude(item => item.ClaimStatus)
            .Include(item => item.Hexes)
                .ThenInclude(item => item.TerrainType)
            .Include(item => item.Settlements)
                .ThenInclude(item => item.Hex)
            .SingleOrDefaultAsync(item => item.KingdomId == kingdomId && !item.IsArchived, cancellationToken);

        if (kingdom is null)
        {
            return null;
        }

        return new KingdomDetailsDto(
            kingdom.KingdomId,
            kingdom.Name,
            0,
            kingdom.Hexes.Count,
            0,
            0,
            new DateTimeOffset(DateTime.SpecifyKind(kingdom.CreatedUtc, DateTimeKind.Utc)),
            new DateTimeOffset(DateTime.SpecifyKind(kingdom.ModifiedUtc, DateTimeKind.Utc)),
            kingdom.Participants
                .Where(item => item.IsActive)
                .OrderBy(item => item.DisplayNameOverride ?? item.UserAccount.DisplayName)
                .Select(item => new KingdomMemberDto(
                    item.UserAccountId.ToString("D"),
                    item.DisplayNameOverride ?? item.UserAccount.DisplayName,
                    item.Roles
                        .OrderByDescending(role => role.IsPrimaryRole)
                        .ThenBy(role => role.RoleType.SortOrder)
                        .Select(role => role.RoleType.DisplayName)
                        .FirstOrDefault() ?? "Participant"))
                .ToArray(),
            kingdom.Hexes
                .OrderBy(item => item.CoordinateY)
                .ThenBy(item => item.CoordinateX)
                .Select(item => new HexDto(
                    item.HexId,
                    item.CoordinateX,
                    item.CoordinateY,
                    item.TerrainType.DisplayName,
                    item.ClaimStatus.IsClaimed,
                    null))
                .ToArray(),
            kingdom.Settlements
                .OrderBy(item => item.Name)
                .Select(item => new TownDto(
                    item.SettlementId,
                    item.Name,
                    item.Hex?.CoordinateX ?? 0,
                    item.Hex?.CoordinateY ?? 0,
                    item.Population ?? 0))
                .ToArray());
    }
}
