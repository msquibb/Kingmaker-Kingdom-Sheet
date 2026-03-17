# Kingmaker Kingdom Sheet - .NET Solution

This is the complete .NET 10 solution for the Kingmaker Kingdom Sheet application, built with .NET Aspire for cloud-ready orchestration.

## Solution Structure

### Projects

#### 1. **KingmakerKingdomSheet.AppHost**
- **Purpose**: Aspire orchestration layer
- **Role**: Defines the application topology and service dependencies
- **Key Features**:
  - Service discovery and registration
  - SQLite development database resource for backend startup
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
  - Hand-written EF Core DbContext aligned to the SQL-first schema
  - SQLite development schema initialization without EF migrations
  - `/database/status` verification endpoint for local integration checks
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
  - DTOs for API requests/responses
  - Service contracts and interfaces
- **Referenced by**: Both API and Web projects for type safety

#### 6. **KingmakerKingdomSheet.Database**
- **Purpose**: Database-first SQL schema project
- **Responsibilities**:
  - Source-controlled schema, constraints, indexes, and reference data
  - Core domain tables for users, kingdoms, memberships, settlements, hexes, and upgrades
  - Buildable `.dacpac` output for repeatable deployments
- **Build**: `dotnet build KingmakerKingdomSheet.Database\KingmakerKingdomSheet.Database.sqlproj`

#### 7. **KingmakerKingdomSheet.Domain**
- **Purpose**: Backend domain model scaffold
- **Contains**:
  - Core backend entities for kingdoms, hexes, towns, and memberships
  - Enums and value objects that do not depend on EF Core or SQL assets
- **Referenced by**: Application layer

#### 8. **KingmakerKingdomSheet.Application**
- **Purpose**: Backend application layer scaffold
- **Contains**:
  - Service abstractions for kingdom-focused backend workflows
  - Preview/in-memory implementations that unblock API composition before SQL integration
  - Dependency injection registration for ApiService
- **Referenced by**: ApiService

#### 9. **KingmakerKingdomSheet.Domain.Tests**
- **Purpose**: Safe unit-test coverage for current domain primitives
- **Current Focus**:
  - Value-object formatting and basic record construction contracts
  - Tests that stay valid before EF Core and SQL-backed persistence arrive

#### 10. **KingmakerKingdomSheet.Application.Tests**
- **Purpose**: Safe unit-test coverage for application-layer wiring
- **Current Focus**:
  - DI registration for preview services
  - Preview catalog service contracts and DTO mapping shape

## Getting Started

### Prerequisites
- .NET 10 SDK (10.0.103 or later)
- Aspire workload installed: `dotnet workload install aspire`

### Build the Solution
```bash
cd dotnet10
dotnet build KingmakerKingdomSheet.sln
```

### Run Tests
```bash
cd dotnet10
dotnet test KingmakerKingdomSheet.sln
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

The AppHost provisions a local SQLite file at `KingmakerKingdomSheet.AppHost\App_Data\kingmaker-dev.db` and passes its connection string to the API service.

### Run Individual Projects (Development)
```bash
# API Service
dotnet run --project KingmakerKingdomSheet.ApiService

# Blazor Web
dotnet run --project KingmakerKingdomSheet.Web
```

When the API service runs on its own in Development, it bootstraps `KingmakerKingdomSheet.ApiService\kingmaker-dev.local.db` from the SQLite initialization script under `Data\Sqlite\`.

## Service Dependencies

```
AppHost (Orchestration)
  ├─ ApiService (REST API + SignalR)
  │   ├─ Application
  │   │   ├─ Domain
  │   │   └─ Shared
  │   ├─ ServiceDefaults
  │   └─ Shared
  └─ Web (Blazor Frontend)
      ├─ ServiceDefaults
      ├─ Shared
      └─ → ApiService (service reference)
```

The AppHost waits for the SQLite resource before starting the API, and the Web project waits for the API before starting.

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

1. **Identity** (Issue #4): Layer ASP.NET Core Identity and GM/Player/RBAC rules onto the new SQLite-backed foundation
2. **API Development** (Issue #6): Build kingdom and town REST endpoints on the DbContext-backed application scaffold
3. **Real-time Sync** (Issue #12): Implement SignalR hubs for hex updates
4. **Expand automated coverage** (Zoe): Add integration tests for the SQL-backed API and future auth flows

## Technology Stack

- **.NET 10**: Latest .NET version
- **Aspire 13**: Cloud-ready orchestration and observability
- **ASP.NET Core**: Web API and Blazor
- **Microsoft.Build.Sql / DacFx**: Database-first schema project and dacpac deployment
- **SignalR**: Real-time bidirectional communication
- **OpenTelemetry**: Distributed tracing and metrics

## Resources

- [Aspire Documentation](https://learn.microsoft.com/dotnet/aspire/)
- [Service Defaults Pattern](https://aka.ms/dotnet/aspire/service-defaults)
- [Blazor Web App](https://learn.microsoft.com/aspnet/core/blazor/)
