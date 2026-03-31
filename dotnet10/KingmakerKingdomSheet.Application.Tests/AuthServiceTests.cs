using KingmakerKingdomSheet.ApiService.Auth;
using KingmakerKingdomSheet.ApiService.Data;
using Microsoft.EntityFrameworkCore;

namespace KingmakerKingdomSheet.Application.Tests;

public sealed class AuthServiceTests
{
    private static KingmakerDbContext CreateInMemoryContext()
    {
        var options = new DbContextOptionsBuilder<KingmakerDbContext>()
            .UseSqlite("DataSource=:memory:")
            .Options;

        var context = new KingmakerDbContext(options);
        context.Database.OpenConnection();
        context.Database.EnsureCreated();

        return context;
    }

    [Fact]
    public async Task RegisterAsync_CreatesUserAndCredential()
    {
        await using var context = CreateInMemoryContext();
        var authService = new AuthService(context);

        var request = new RegisterRequest("Test User", "test@example.com", "Password123!");
        var result = await authService.RegisterAsync(request);

        Assert.True(result.Succeeded);
        Assert.NotNull(result.UserAccountId);

        var user = await context.UserAccounts
            .Include(u => u.LocalCredential)
            .FirstOrDefaultAsync(u => u.UserAccountId == result.UserAccountId);

        Assert.NotNull(user);
        Assert.Equal("Test User", user.DisplayName);
        Assert.Equal("test@example.com", user.Email);
        Assert.NotNull(user.LocalCredential);
        Assert.NotEmpty(user.LocalCredential.PasswordHash);
    }

    [Fact]
    public async Task RegisterAsync_RejectsDuplicateEmail()
    {
        await using var context = CreateInMemoryContext();
        var authService = new AuthService(context);

        var request1 = new RegisterRequest("User One", "test@example.com", "Password123!");
        await authService.RegisterAsync(request1);

        var request2 = new RegisterRequest("User Two", "test@example.com", "Password456!");
        var result = await authService.RegisterAsync(request2);

        Assert.False(result.Succeeded);
        Assert.Contains("already exists", result.Error);
    }

    [Fact]
    public async Task RegisterAsync_RequiresMinimumPasswordLength()
    {
        await using var context = CreateInMemoryContext();
        var authService = new AuthService(context);

        var request = new RegisterRequest("Test User", "test@example.com", "short");
        var result = await authService.RegisterAsync(request);

        Assert.False(result.Succeeded);
        Assert.Contains("at least 8 characters", result.Error);
    }

    [Fact]
    public async Task LoginAsync_SucceedsWithCorrectPassword()
    {
        await using var context = CreateInMemoryContext();
        var authService = new AuthService(context);

        var registerRequest = new RegisterRequest("Test User", "test@example.com", "Password123!");
        await authService.RegisterAsync(registerRequest);

        var loginRequest = new LoginRequest("test@example.com", "Password123!");
        var result = await authService.LoginAsync(loginRequest);

        Assert.True(result.Succeeded);
        Assert.NotNull(result.UserAccountId);
    }

    [Fact]
    public async Task LoginAsync_FailsWithWrongPassword()
    {
        await using var context = CreateInMemoryContext();
        var authService = new AuthService(context);

        var registerRequest = new RegisterRequest("Test User", "test@example.com", "Password123!");
        await authService.RegisterAsync(registerRequest);

        var loginRequest = new LoginRequest("test@example.com", "WrongPassword!");
        var result = await authService.LoginAsync(loginRequest);

        Assert.False(result.Succeeded);
        Assert.Contains("Invalid email or password", result.Error);
    }

    [Fact]
    public async Task LoginAsync_FailsWithNonExistentEmail()
    {
        await using var context = CreateInMemoryContext();
        var authService = new AuthService(context);

        var loginRequest = new LoginRequest("nonexistent@example.com", "Password123!");
        var result = await authService.LoginAsync(loginRequest);

        Assert.False(result.Succeeded);
        Assert.Contains("Invalid email or password", result.Error);
    }

    [Fact]
    public async Task LoginAsync_LocksOutAfterMaxFailedAttempts()
    {
        await using var context = CreateInMemoryContext();
        var authService = new AuthService(context);

        var registerRequest = new RegisterRequest("Test User", "test@example.com", "Password123!");
        await authService.RegisterAsync(registerRequest);

        for (int i = 0; i < 5; i++)
        {
            var loginRequest = new LoginRequest("test@example.com", "WrongPassword!");
            await authService.LoginAsync(loginRequest);
        }

        var finalLoginRequest = new LoginRequest("test@example.com", "Password123!");
        var result = await authService.LoginAsync(finalLoginRequest);

        Assert.False(result.Succeeded);
        Assert.Contains("locked", result.Error);
    }
}
