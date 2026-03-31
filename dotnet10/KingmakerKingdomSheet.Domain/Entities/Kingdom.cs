namespace KingmakerKingdomSheet.Domain.Entities;

public sealed record Kingdom(
    Guid Id,
    string Name,
    int Level,
    int Size,
    int ControlDc,
    int Unrest,
    DateTimeOffset CreatedAtUtc,
    DateTimeOffset UpdatedAtUtc,
    IReadOnlyList<KingdomMember> Members,
    IReadOnlyList<KingdomHex> Hexes,
    IReadOnlyList<Town> Towns);
