namespace KingmakerKingdomSheet.Shared.Requests;

public sealed record ClaimHexRequest(
    Guid KingdomId,
    int Column,
    int Row,
    string? ClaimedByUserId);
