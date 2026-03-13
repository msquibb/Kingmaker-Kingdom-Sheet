using KingmakerKingdomSheet.Domain.Enums;

namespace KingmakerKingdomSheet.Domain.Entities;

public sealed record KingdomMember(
    Guid KingdomId,
    string UserId,
    string DisplayName,
    KingdomMemberRole Role);
