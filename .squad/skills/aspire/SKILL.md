---
name: "aspire"
description: ".NET Aspire 13 orchestration patterns and practices"
domain: "aspire"
confidence: "high"
source: "project-implementation"
---

## Context

This project uses .NET Aspire 13 for cloud-ready orchestration. Aspire provides service discovery, health checks, OpenTelemetry, and resilience patterns out of the box.

## Patterns

### Project Structure

Aspire projects follow a standard layout:

```
dotnet10/
├── KingmakerKingdomSheet.AppHost/         # Orchestration (DistributedApplication)
├── KingmakerKingdomSheet.ServiceDefaults/ # Shared config, telemetry, resilience
├── KingmakerKingdomSheet.ApiService/      # ASP.NET Core Web API
├── KingmakerKingdomSheet.Web/             # Blazor Web App
└── KingmakerKingdomSheet.Shared/          # Contracts, DTOs, models
```

### AppHost Configuration

**File**: `AppHost.cs`

```csharp
var builder = DistributedApplication.CreateBuilder(args);

// Register API service with health check
var apiService = builder.AddProject<Projects.KingmakerKingdomSheet_ApiService>("apiservice")
    .WithHttpHealthCheck("/health");

// Register Web with dependency on API
builder.AddProject<Projects.KingmakerKingdomSheet_Web>("webfrontend")
    .WithExternalHttpEndpoints()  // Expose to external traffic
    .WithHttpHealthCheck("/health")
    .WithReference(apiService)    // Service discovery
    .WaitFor(apiService);         // Don't start until API is healthy

builder.Build().Run();
```

**Key methods**:
- `.WithHttpHealthCheck(path)` — Monitor service health
- `.WithExternalHttpEndpoints()` — Expose service to external traffic
- `.WithReference(service)` — Enable service discovery (injects endpoint URL)
- `.WaitFor(service)` — Dependency ordering (wait until healthy)

### ServiceDefaults Pattern

**File**: `ServiceDefaults/Extensions.cs`

Every service should call `builder.AddServiceDefaults()` to get:
- OpenTelemetry (logs, traces, metrics)
- Health checks (`/health`, `/alive`)
- Service discovery
- HTTP resilience (retry, circuit breaker, timeout)

**Usage in service Program.cs**:
```csharp
var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();  // <- Add this first

// ... other service configuration ...

var app = builder.Build();

app.MapDefaultEndpoints();  // <- Exposes /health and /alive

app.Run();
```

### Health Checks

Two endpoints are automatically configured:
- `/health` — All health checks must pass (readiness probe)
- `/alive` — Only "live" tagged checks must pass (liveness probe)

**Adding custom health checks**:
```csharp
builder.Services.AddHealthChecks()
    .AddCheck("database", () => /* check DB connection */, ["live"])
    .AddCheck("external-api", () => /* check external dependency */);
```

### Service Discovery

When you use `.WithReference(service)`, Aspire injects the service endpoint as configuration.

**API service discovery in Web project**:
```csharp
// Automatic service discovery via HttpClient
builder.Services.AddHttpClient<IWeatherClient, WeatherClient>(
    client => client.BaseAddress = new("https+http://apiservice"));  // <- Service name
```

The `https+http://` scheme means "try HTTPS, fall back to HTTP".

### Running the Application

**Development**:
```bash
cd dotnet10/KingmakerKingdomSheet.AppHost
dotnet run
```

This starts the Aspire Dashboard (typically https://localhost:17129) where you can:
- View all services and their status
- Monitor logs in real-time
- View distributed traces
- Check metrics

### Adding New Services

1. Create the service project
2. Add project to solution: `dotnet sln add NewService/NewService.csproj`
3. Reference ServiceDefaults: `dotnet add NewService reference ServiceDefaults`
4. Call `builder.AddServiceDefaults()` in service's `Program.cs`
5. Call `app.MapDefaultEndpoints()` before `app.Run()`
6. Register in AppHost:
   ```csharp
   var newService = builder.AddProject<Projects.NewService>("newservice")
       .WithHttpHealthCheck("/health");
   ```

### Adding Database Resources

Aspire provides built-in support for databases:

```csharp
// SQLite (for local development)
var db = builder.AddSqlite("sqlite")
    .WithDataVolume();  // Persist data

// SQL Server
var db = builder.AddSqlServer("sql")
    .AddDatabase("kingmakerdb");

// PostgreSQL
var db = builder.AddPostgres("postgres")
    .AddDatabase("kingmakerdb");

// Reference database in services
builder.AddProject<Projects.ApiService>("apiservice")
    .WithReference(db);  // Injects connection string
```

### OpenTelemetry

ServiceDefaults configures OpenTelemetry with:
- **Logs**: Formatted messages with scopes
- **Traces**: ASP.NET Core + HttpClient instrumentation
- **Metrics**: Runtime, ASP.NET Core, HttpClient metrics

All telemetry flows to the Aspire Dashboard automatically. No manual configuration needed.

## Anti-Patterns

- **Don't** manually configure service URLs — use service discovery with `.WithReference()`
- **Don't** skip health checks — every service should have `/health` endpoint
- **Don't** run services individually in development — always use AppHost for consistent experience
- **Don't** forget to call `builder.AddServiceDefaults()` — this is critical for observability
- **Don't** hard-code connection strings — use Aspire resource injection via `.WithReference()`

## Examples

### Complete Service Configuration

```csharp
// Program.cs for any ASP.NET Core service
var builder = WebApplication.CreateBuilder(args);

// 1. Add ServiceDefaults first (telemetry, resilience, health)
builder.AddServiceDefaults();

// 2. Configure your services
builder.Services.AddControllers();
builder.Services.AddDbContext<AppDbContext>();

// 3. Build the app
var app = builder.Build();

// 4. Configure middleware
app.UseExceptionHandler();
app.MapControllers();

// 5. Map default endpoints (/health, /alive)
app.MapDefaultEndpoints();

// 6. Run
app.Run();
```

### Adding SignalR Hub to AppHost

```csharp
var signalrHub = builder.AddProject<Projects.SignalRHub>("signalrhub")
    .WithHttpHealthCheck("/health")
    .WithExternalHttpEndpoints();  // Clients connect directly

builder.AddProject<Projects.Web>("webfrontend")
    .WithReference(signalrHub);  // Inject SignalR URL
```

## Resources

- [Aspire Documentation](https://learn.microsoft.com/dotnet/aspire/)
- [Service Defaults Pattern](https://aka.ms/dotnet/aspire/service-defaults)
- [Health Checks](https://aka.ms/dotnet/aspire/healthchecks)
