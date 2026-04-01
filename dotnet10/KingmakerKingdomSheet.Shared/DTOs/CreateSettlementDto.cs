namespace KingmakerKingdomSheet.Shared.DTOs;

public sealed record CreateSettlementDto(string Name, byte SettlementTypeId, Guid? HexId);
