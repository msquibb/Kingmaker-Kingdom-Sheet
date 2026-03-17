using KingmakerKingdomSheet.ApiService.Data;
using KingmakerKingdomSheet.ApiService.Services;
using KingmakerKingdomSheet.Application.Contracts;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

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
builder.Services.AddSingleton<SqliteDevelopmentDatabaseInitializer>();
builder.Services.AddHealthChecks()
    .AddCheck<KingmakerDatabaseHealthCheck>("kingmaker-database");

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseExceptionHandler();

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

app.MapDefaultEndpoints();

app.Run();

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
