namespace KingmakerKingdomSheet.Shared.DTOs;

public sealed record KingdomDetailsDto(
    Guid Id,
    string Name,
    int Level,
    int Size,
    int ControlDc,
    int Unrest,
    DateTimeOffset CreatedAtUtc,
    DateTimeOffset UpdatedAtUtc,
    IReadOnlyList<KingdomMemberDto> Members,
    IReadOnlyList<HexDto> Hexes,
    IReadOnlyList<TownDto> Towns);
