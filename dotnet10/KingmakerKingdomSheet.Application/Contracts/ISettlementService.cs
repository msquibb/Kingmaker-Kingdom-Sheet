using KingmakerKingdomSheet.Shared.DTOs;

namespace KingmakerKingdomSheet.Application.Contracts;

public interface ISettlementService
{
    Task<IReadOnlyList<TownDto>> ListByKingdomAsync(Guid kingdomId, CancellationToken ct = default);
    Task<TownDto?> GetAsync(Guid settlementId, CancellationToken ct = default);
    Task<TownDto> CreateAsync(Guid kingdomId, CreateSettlementDto dto, CancellationToken ct = default);
    Task<TownDto?> UpdateAsync(Guid settlementId, UpdateSettlementDto dto, CancellationToken ct = default);
    Task<bool> DeleteAsync(Guid settlementId, CancellationToken ct = default);
}
