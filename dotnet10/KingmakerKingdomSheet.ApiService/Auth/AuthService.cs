using KingmakerKingdomSheet.ApiService.Data;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace KingmakerKingdomSheet.ApiService.Auth;

public interface IAuthService
{
    Task<AuthResult> RegisterAsync(RegisterRequest request, CancellationToken ct = default);
    Task<AuthResult> LoginAsync(LoginRequest request, CancellationToken ct = default);
    Task<UserInfo?> GetCurrentUserAsync(Guid userAccountId, CancellationToken ct = default);
}

public sealed class AuthService(KingmakerDbContext dbContext) : IAuthService
{
    private const int MaxFailedAttempts = 5;
    private static readonly TimeSpan LockoutDuration = TimeSpan.FromMinutes(15);

    public async Task<AuthResult> RegisterAsync(RegisterRequest request, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(request.DisplayName))
            return new AuthResult(false, "Display name is required.", null);

        if (string.IsNullOrWhiteSpace(request.Email))
            return new AuthResult(false, "Email is required.", null);

        if (string.IsNullOrWhiteSpace(request.Password))
            return new AuthResult(false, "Password is required.", null);

        if (request.Password.Length < 8)
            return new AuthResult(false, "Password must be at least 8 characters.", null);

        var normalizedEmail = request.Email.Trim().ToUpperInvariant();
        var existingUser = await dbContext.UserAccounts
            .FirstOrDefaultAsync(u => u.NormalizedEmail == normalizedEmail, ct);

        if (existingUser != null)
            return new AuthResult(false, "A user with this email already exists.", null);

        var userAccount = new UserAccount
        {
            DisplayName = request.DisplayName.Trim(),
            Email = request.Email.Trim(),
            IdentityProvider = "local",
            ExternalSubject = Guid.NewGuid().ToString(),
            IsActive = true
        };

        dbContext.UserAccounts.Add(userAccount);
        await dbContext.SaveChangesAsync(ct);

        var passwordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);

        var localCredential = new LocalCredential
        {
            UserAccountId = userAccount.UserAccountId,
            PasswordHash = passwordHash,
            FailedLoginAttempts = 0
        };

        dbContext.LocalCredentials.Add(localCredential);
        await dbContext.SaveChangesAsync(ct);

        return new AuthResult(true, null, userAccount.UserAccountId);
    }

    public async Task<AuthResult> LoginAsync(LoginRequest request, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(request.Email))
            return new AuthResult(false, "Email is required.", null);

        if (string.IsNullOrWhiteSpace(request.Password))
            return new AuthResult(false, "Password is required.", null);

        var normalizedEmail = request.Email.Trim().ToUpperInvariant();
        var userAccount = await dbContext.UserAccounts
            .Include(u => u.LocalCredential)
            .FirstOrDefaultAsync(u => u.NormalizedEmail == normalizedEmail, ct);

        if (userAccount?.LocalCredential == null)
            return new AuthResult(false, "Invalid email or password.", null);

        if (!userAccount.IsActive)
            return new AuthResult(false, "Account is inactive.", null);

        var credential = userAccount.LocalCredential;

        if (credential.LockoutEndUtc.HasValue && credential.LockoutEndUtc.Value > DateTime.UtcNow)
        {
            var remainingMinutes = (int)(credential.LockoutEndUtc.Value - DateTime.UtcNow).TotalMinutes + 1;
            return new AuthResult(false, $"Account is locked. Try again in {remainingMinutes} minute(s).", null);
        }

        if (!BCrypt.Net.BCrypt.Verify(request.Password, credential.PasswordHash))
        {
            credential.FailedLoginAttempts++;
            
            if (credential.FailedLoginAttempts >= MaxFailedAttempts)
            {
                credential.LockoutEndUtc = DateTime.UtcNow.Add(LockoutDuration);
                credential.FailedLoginAttempts = 0;
            }
            
            await dbContext.SaveChangesAsync(ct);
            return new AuthResult(false, "Invalid email or password.", null);
        }

        credential.FailedLoginAttempts = 0;
        credential.LockoutEndUtc = null;
        credential.LastLoginUtc = DateTime.UtcNow;
        await dbContext.SaveChangesAsync(ct);

        return new AuthResult(true, null, userAccount.UserAccountId);
    }

    public async Task<UserInfo?> GetCurrentUserAsync(Guid userAccountId, CancellationToken ct = default)
    {
        var userAccount = await dbContext.UserAccounts
            .FirstOrDefaultAsync(u => u.UserAccountId == userAccountId && u.IsActive, ct);

        if (userAccount == null)
            return null;

        var systemClaims = new List<string> { "authenticated" };

        return new UserInfo(
            userAccount.UserAccountId,
            userAccount.DisplayName,
            userAccount.Email ?? string.Empty,
            systemClaims);
    }
}
