using KingmakerKingdomSheet.Shared.DTOs;

namespace KingmakerKingdomSheet.Application.Contracts;

public interface IKingdomCatalogService
{
    Task<IReadOnlyList<KingdomSummaryDto>> ListAsync(CancellationToken cancellationToken = default);

    Task<KingdomDetailsDto?> GetAsync(Guid kingdomId, CancellationToken cancellationToken = default);
}
