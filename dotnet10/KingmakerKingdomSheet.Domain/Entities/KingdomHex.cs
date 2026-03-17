using KingmakerKingdomSheet.Domain.Enums;
using KingmakerKingdomSheet.Domain.ValueObjects;

namespace KingmakerKingdomSheet.Domain.Entities;

public sealed record KingdomHex(
    Guid Id,
    Guid KingdomId,
    HexCoordinate Coordinate,
    HexTerrain Terrain,
    bool IsClaimed,
    string? ClaimedByUserId,
    DateTimeOffset? LastUpdatedAtUtc);
