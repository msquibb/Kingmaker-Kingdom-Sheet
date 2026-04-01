using KingmakerKingdomSheet.ApiService.Data;
using KingmakerKingdomSheet.Application.Contracts;
using KingmakerKingdomSheet.Shared.DTOs;
using Microsoft.EntityFrameworkCore;

namespace KingmakerKingdomSheet.ApiService.Services;

internal sealed class SqliteSettlementService(KingmakerDbContext dbContext) : ISettlementService
{
    public async Task<IReadOnlyList<TownDto>> ListByKingdomAsync(Guid kingdomId, CancellationToken ct = default)
    {
        return await dbContext.Settlements
            .AsNoTracking()
            .Include(s => s.Hex)
            .Where(s => s.KingdomId == kingdomId)
            .OrderBy(s => s.Name)
            .Select(s => new TownDto(
                s.SettlementId,
                s.Name,
                s.Hex != null ? s.Hex.CoordinateX : 0,
                s.Hex != null ? s.Hex.CoordinateY : 0,
                s.Population ?? 0))
            .ToListAsync(ct);
    }

    public async Task<TownDto?> GetAsync(Guid settlementId, CancellationToken ct = default)
    {
        var settlement = await dbContext.Settlements
            .AsNoTracking()
            .Include(s => s.Hex)
            .SingleOrDefaultAsync(s => s.SettlementId == settlementId, ct);

        if (settlement is null)
            return null;

        return new TownDto(
            settlement.SettlementId,
            settlement.Name,
            settlement.Hex?.CoordinateX ?? 0,
            settlement.Hex?.CoordinateY ?? 0,
            settlement.Population ?? 0);
    }

    public async Task<TownDto> CreateAsync(Guid kingdomId, CreateSettlementDto dto, CancellationToken ct = default)
    {
        var now = DateTime.UtcNow;

        var settlement = new Settlement
        {
            SettlementId = Guid.NewGuid(),
            KingdomId = kingdomId,
            Name = dto.Name,
            SettlementTypeId = dto.SettlementTypeId,
            HexId = dto.HexId,
            CreatedUtc = now,
            ModifiedUtc = now
        };

        dbContext.Settlements.Add(settlement);
        await dbContext.SaveChangesAsync(ct);

        var result = await GetAsync(settlement.SettlementId, ct);
        return result!;
    }

    public async Task<TownDto?> UpdateAsync(Guid settlementId, UpdateSettlementDto dto, CancellationToken ct = default)
    {
        var settlement = await dbContext.Settlements
            .SingleOrDefaultAsync(s => s.SettlementId == settlementId, ct);

        if (settlement is null)
            return null;

        settlement.Name = dto.Name;
        settlement.SettlementTypeId = dto.SettlementTypeId;
        settlement.HexId = dto.HexId;
        settlement.Population = dto.Population;
        settlement.Notes = dto.Notes;
        settlement.ModifiedUtc = DateTime.UtcNow;

        await dbContext.SaveChangesAsync(ct);

        return await GetAsync(settlementId, ct);
    }

    public async Task<bool> DeleteAsync(Guid settlementId, CancellationToken ct = default)
    {
        var settlement = await dbContext.Settlements
            .SingleOrDefaultAsync(s => s.SettlementId == settlementId, ct);

        if (settlement is null)
            return false;

        dbContext.Settlements.Remove(settlement);
        await dbContext.SaveChangesAsync(ct);

        return true;
    }
}
