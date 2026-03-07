# Kingmaker Kingdom Sheet - .NET Solution

This is the complete .NET 10 solution for the Kingmaker Kingdom Sheet application, built with .NET Aspire for cloud-ready orchestration.

## Solution Structure

### Projects

#### 1. **KingmakerKingdomSheet.AppHost**
- **Purpose**: Aspire orchestration layer
- **Role**: Defines the application topology and service dependencies
- **Key Features**:
  - Service discovery and registration
  - Health check monitoring
  - Dependency management (API → Web)
  - Aspire Dashboard integration

**Running the AppHost**: `dotnet run --project KingmakerKingdomSheet.AppHost`

This starts the Aspire Dashboard where you can monitor all services, logs, traces, and metrics in real-time.

#### 2. **KingmakerKingdomSheet.ServiceDefaults**
- **Purpose**: Shared Aspire configuration and middleware
- **Included Features**:
  - OpenTelemetry (logging, metrics, tracing)
  - Health checks (`/health` and `/alive` endpoints)
  - Service discovery configuration
  - HTTP resilience patterns (retry, circuit breaker, timeout)
  - Standardized error handling

**Usage**: Reference this project and call `builder.AddServiceDefaults()` in your service's `Program.cs`.

#### 3. **KingmakerKingdomSheet.ApiService**
- **Purpose**: ASP.NET Core Web API
- **Responsibilities**:
  - REST endpoints for kingdoms, hexes, towns, users
  - Entity Framework Core data access
  - SignalR hubs for real-time sync
  - ASP.NET Core Identity for authentication/authorization

**Endpoint**: Exposed via Aspire service discovery as `apiservice`

#### 4. **KingmakerKingdomSheet.Web**
- **Purpose**: Blazor Web App (Auto render mode)
- **Responsibilities**:
  - Interactive UI components
  - Kingdom management interface
  - Hex grid visualization
  - Real-time player collaboration

**Endpoint**: External endpoint with reference to API service

#### 5. **KingmakerKingdomSheet.Shared**
- **Purpose**: Shared contracts, DTOs, and models
- **Contains**:
  - Domain models (Kingdom, Hex, Town, etc.)
  - DTOs for API requests/responses
  - Service contracts and interfaces
- **Referenced by**: Both API and Web projects for type safety

## Getting Started

### Prerequisites
- .NET 10 SDK (10.0.103 or later)
- Aspire workload installed: `dotnet workload install aspire`

### Build the Solution
```bash
cd dotnet10
dotnet build KingmakerKingdomSheet.sln
```

### Run with Aspire Dashboard
```bash
cd dotnet10/KingmakerKingdomSheet.AppHost
dotnet run
```

The Aspire Dashboard will launch in your browser. From there you can:
- Start/stop individual services
- View logs in real-time
- Monitor traces and metrics
- Check health status

### Run Individual Projects (Development)
```bash
# API Service
dotnet run --project KingmakerKingdomSheet.ApiService

# Blazor Web
dotnet run --project KingmakerKingdomSheet.Web
```

## Service Dependencies

```
AppHost (Orchestration)
  ├─ ApiService (REST API + SignalR)
  │   ├─ ServiceDefaults
  │   └─ Shared
  └─ Web (Blazor Frontend)
      ├─ ServiceDefaults
      ├─ Shared
      └─ → ApiService (service reference)
```

The Web project has a `WaitFor(apiService)` dependency ensuring the API is healthy before the frontend starts.

## Health Checks

All services expose two health endpoints (in development):
- `/health` - All health checks must pass (readiness)
- `/alive` - Only "live" tagged checks must pass (liveness)

HTTP health checks are configured in the AppHost for both API and Web services.

## Observability

ServiceDefaults configures OpenTelemetry for:
- **Logs**: Structured logging with formatted messages and scopes
- **Traces**: Distributed tracing across services (ASP.NET Core + HttpClient)
- **Metrics**: Runtime, ASP.NET Core, and HttpClient metrics

View all telemetry in the Aspire Dashboard when running via AppHost.

## Next Steps

1. **Database Setup** (Issue #3): Configure EF Core with SQLite
2. **Identity** (Issue #4): Implement ASP.NET Core Identity with GM/Player roles
3. **API Development** (Issue #6): Build Kingdom and Town REST endpoints
4. **Blazor Components** (Issue #5): Create component library and layout
5. **Real-time Sync** (Issue #12): Implement SignalR hubs for hex updates

## Technology Stack

- **.NET 10**: Latest .NET version
- **Aspire 13**: Cloud-ready orchestration and observability
- **ASP.NET Core**: Web API and Blazor
- **Entity Framework Core**: ORM with SQLite
- **SignalR**: Real-time bidirectional communication
- **OpenTelemetry**: Distributed tracing and metrics

## Resources

- [Aspire Documentation](https://learn.microsoft.com/dotnet/aspire/)
- [Service Defaults Pattern](https://aka.ms/dotnet/aspire/service-defaults)
- [Blazor Web App](https://learn.microsoft.com/aspnet/core/blazor/)
