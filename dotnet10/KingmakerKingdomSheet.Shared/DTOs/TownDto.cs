namespace KingmakerKingdomSheet.Shared.DTOs;

public sealed record TownDto(
    Guid Id,
    string Name,
    int Column,
    int Row,
    int Population);
