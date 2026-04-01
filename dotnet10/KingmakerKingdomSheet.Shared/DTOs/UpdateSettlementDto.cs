namespace KingmakerKingdomSheet.Shared.DTOs;

public sealed record UpdateSettlementDto(string Name, byte SettlementTypeId, Guid? HexId, int? Population, string? Notes);
