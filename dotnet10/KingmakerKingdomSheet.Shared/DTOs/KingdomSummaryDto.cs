namespace KingmakerKingdomSheet.Shared.DTOs;

public sealed record KingdomSummaryDto(
    Guid Id,
    string Name,
    int Level,
    int Size,
    int ControlDc,
    int Unrest,
    int MemberCount,
    int ClaimedHexCount,
    int TownCount);
