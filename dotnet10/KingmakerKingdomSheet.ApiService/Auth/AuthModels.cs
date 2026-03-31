namespace KingmakerKingdomSheet.ApiService.Auth;

public sealed record RegisterRequest(string DisplayName, string Email, string Password);

public sealed record LoginRequest(string Email, string Password);

public sealed record AuthResult(bool Succeeded, string? Error, Guid? UserAccountId);

public sealed record UserInfo(
    Guid UserAccountId,
    string DisplayName,
    string Email,
    IReadOnlyList<string> SystemClaims);
