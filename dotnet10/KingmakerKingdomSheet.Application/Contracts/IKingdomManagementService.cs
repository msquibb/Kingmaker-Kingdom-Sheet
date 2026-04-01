using KingmakerKingdomSheet.Shared.DTOs;

namespace KingmakerKingdomSheet.Application.Contracts;

public interface IKingdomManagementService
{
    Task<KingdomDetailsDto> CreateAsync(Guid createdByUserId, CreateKingdomDto dto, CancellationToken ct = default);
    Task<KingdomDetailsDto?> UpdateAsync(Guid kingdomId, UpdateKingdomDto dto, CancellationToken ct = default);
    Task<bool> ArchiveAsync(Guid kingdomId, CancellationToken ct = default);
}
