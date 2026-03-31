namespace KingmakerKingdomSheet.Shared.DTOs;

public sealed record RegisterRequestDto(string DisplayName, string Email, string Password);

public sealed record LoginRequestDto(string Email, string Password);

public sealed record UserInfoDto(
    Guid UserAccountId,
    string DisplayName,
    string Email,
    IReadOnlyList<string> SystemClaims);
