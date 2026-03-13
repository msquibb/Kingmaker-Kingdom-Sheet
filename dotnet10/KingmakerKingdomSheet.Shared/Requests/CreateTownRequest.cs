namespace KingmakerKingdomSheet.Shared.Requests;

public sealed record CreateTownRequest(
    Guid KingdomId,
    string Name,
    int Column,
    int Row,
    int Population);
