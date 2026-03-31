namespace KingmakerKingdomSheet.Shared.DTOs;

public sealed record HexDto(
    Guid Id,
    int Column,
    int Row,
    string Terrain,
    bool IsClaimed,
    string? ClaimedByUserId);
