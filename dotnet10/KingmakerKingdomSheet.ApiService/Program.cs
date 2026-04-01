using KingmakerKingdomSheet.ApiService.Auth;
using KingmakerKingdomSheet.ApiService.Data;
using KingmakerKingdomSheet.ApiService.Services;
using KingmakerKingdomSheet.Application.Contracts;
using KingmakerKingdomSheet.Shared.DTOs;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System.Security.Claims;

var builder = WebApplication.CreateBuilder(args);

// Add service defaults & Aspire client integrations.
builder.AddServiceDefaults();

// Add services to the container.
builder.Services.AddProblemDetails();
builder.Services.AddKingdomApplication();

var databaseConnectionString = SqliteConnectionStringNormalizer.Normalize(
    builder.Configuration.GetConnectionString("kingmaker-dev")
        ?? throw new InvalidOperationException("Connection string 'kingmaker-dev' is not configured."),
    builder.Environment.ContentRootPath);

builder.Services.Configure<KingmakerDatabaseOptions>(options => options.ConnectionString = databaseConnectionString);

builder.Services.AddDbContext<KingmakerDbContext>((serviceProvider, options) =>
{
    var connectionString = serviceProvider.GetRequiredService<IOptions<KingmakerDatabaseOptions>>().Value.ConnectionString;

    options.UseSqlite(connectionString);

    if (builder.Environment.IsDevelopment())
    {
        options.EnableDetailedErrors();
        options.EnableSensitiveDataLogging();
    }
});

builder.Services.AddScoped<IKingdomCatalogService, SqliteKingdomCatalogService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IKingdomAuthorizationService, KingdomAuthorizationService>();
builder.Services.AddSingleton<SqliteDevelopmentDatabaseInitializer>();
builder.Services.AddHealthChecks()
    .AddCheck<KingmakerDatabaseHealthCheck>("kingmaker-database");

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/api/auth/login";
        options.Cookie.Name = "KingmakerAuth";
        options.Cookie.HttpOnly = true;
        options.Cookie.SameSite = SameSiteMode.Lax;
        options.Events.OnRedirectToLogin = context =>
        {
            context.Response.StatusCode = 401;
            return Task.CompletedTask;
        };
    });
builder.Services.AddAuthorization();

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseExceptionHandler();

app.UseAuthentication();
app.UseAuthorization();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

await using (var scope = app.Services.CreateAsyncScope())
{
    var databaseInitializer = scope.ServiceProvider.GetRequiredService<SqliteDevelopmentDatabaseInitializer>();
    await databaseInitializer.InitializeAsync();
}

string[] summaries = ["Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"];

app.MapGet("/", () => "API service is running. Navigate to /database/status to verify the SQLite development database or /weatherforecast to see sample data.");

app.MapGet("/database/status", async (KingmakerDbContext dbContext, CancellationToken cancellationToken) =>
{
    var connection = dbContext.Database.GetDbConnection();

    return Results.Ok(new
    {
        Provider = dbContext.Database.ProviderName,
        Database = connection.DataSource,
        KingdomCount = await dbContext.Kingdoms.CountAsync(cancellationToken),
        ParticipantCount = await dbContext.KingdomParticipants.CountAsync(cancellationToken),
        HexCount = await dbContext.Hexes.CountAsync(cancellationToken),
        SettlementCount = await dbContext.Settlements.CountAsync(cancellationToken),
        ReferenceRows = new
        {
            ClaimStatuses = await dbContext.ClaimStatuses.CountAsync(cancellationToken),
            FogStates = await dbContext.FogStates.CountAsync(cancellationToken),
            TerrainTypes = await dbContext.HexTerrainTypes.CountAsync(cancellationToken),
            SettlementTypes = await dbContext.SettlementTypes.CountAsync(cancellationToken),
            KingdomRoles = await dbContext.KingdomRoleTypes.CountAsync(cancellationToken),
            KingdomPermissions = await dbContext.KingdomPermissionTypes.CountAsync(cancellationToken)
        }
    });
});

app.MapGet("/weatherforecast", () =>
{
    var forecast = Enumerable.Range(1, 5).Select(index =>
        new WeatherForecast
        (
            DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            Random.Shared.Next(-20, 55),
            summaries[Random.Shared.Next(summaries.Length)]
        ))
        .ToArray();
    return forecast;
})
.WithName("GetWeatherForecast");

app.MapPost("/api/auth/register", async (RegisterRequestDto dto, IAuthService authService, HttpContext httpContext) =>
{
    var request = new RegisterRequest(dto.DisplayName, dto.Email, dto.Password);
    var result = await authService.RegisterAsync(request);

    if (!result.Succeeded)
        return Results.BadRequest(new { error = result.Error });

    var claims = new List<Claim>
    {
        new(ClaimTypes.NameIdentifier, result.UserAccountId!.Value.ToString()),
        new(ClaimTypes.Email, dto.Email),
        new(ClaimTypes.Name, dto.DisplayName)
    };

    var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
    var principal = new ClaimsPrincipal(identity);

    await httpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

    return Results.Ok(new { userAccountId = result.UserAccountId });
});

app.MapPost("/api/auth/login", async (LoginRequestDto dto, IAuthService authService, HttpContext httpContext) =>
{
    var request = new LoginRequest(dto.Email, dto.Password);
    var result = await authService.LoginAsync(request);

    if (!result.Succeeded)
        return Results.BadRequest(new { error = result.Error });

    var userInfo = await authService.GetCurrentUserAsync(result.UserAccountId!.Value);
    if (userInfo == null)
        return Results.BadRequest(new { error = "User not found." });

    var claims = new List<Claim>
    {
        new(ClaimTypes.NameIdentifier, userInfo.UserAccountId.ToString()),
        new(ClaimTypes.Email, userInfo.Email),
        new(ClaimTypes.Name, userInfo.DisplayName)
    };

    var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
    var principal = new ClaimsPrincipal(identity);

    await httpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

    return Results.Ok(new { userAccountId = userInfo.UserAccountId });
});

app.MapPost("/api/auth/logout", async (HttpContext httpContext) =>
{
    await httpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
    return Results.Ok();
});

app.MapGet("/api/auth/me", async (HttpContext httpContext, IAuthService authService) =>
{
    var userIdClaim = httpContext.User.FindFirst(ClaimTypes.NameIdentifier);
    if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out var userId))
        return Results.Unauthorized();

    var userInfo = await authService.GetCurrentUserAsync(userId);
    if (userInfo == null)
        return Results.Unauthorized();

    var dto = new UserInfoDto(
        userInfo.UserAccountId,
        userInfo.DisplayName,
        userInfo.Email,
        userInfo.SystemClaims);

    return Results.Ok(dto);
}).RequireAuthorization();

app.MapDefaultEndpoints();

app.Run();

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
