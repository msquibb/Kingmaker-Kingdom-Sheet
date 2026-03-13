using KingmakerKingdomSheet.Domain.ValueObjects;

namespace KingmakerKingdomSheet.Domain.Entities;

public sealed record Town(
    Guid Id,
    Guid KingdomId,
    string Name,
    HexCoordinate Location,
    int Population,
    DateTimeOffset EstablishedAtUtc);
