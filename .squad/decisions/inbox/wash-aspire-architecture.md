# Aspire Architecture Pattern

**Date**: 2026-03-07  
**Author**: Wash  
**Status**: Implemented  

## Decision

Use .NET Aspire 13 as the orchestration layer for the Kingmaker Kingdom Sheet application with a 5-project structure:

1. **AppHost** — Defines service topology and dependencies
2. **ServiceDefaults** — Shared configuration, telemetry, and resilience
3. **ApiService** — ASP.NET Core Web API backend
4. **Web** — Blazor Web App frontend (Auto render mode)
5. **Shared** — Contracts, DTOs, and domain models

## Rationale

- **Aspire provides out-of-the-box**:
  - Service discovery and orchestration
  - OpenTelemetry (logs, traces, metrics) with zero config
  - Health checks and resilience patterns
  - Developer dashboard for real-time monitoring
  - Cloud-ready architecture from day one

- **ServiceDefaults pattern**:
  - Ensures consistency across all services
  - Single place to configure telemetry, health checks, resilience
  - Easy to extend with new middleware or configuration

- **Shared project**:
  - Type safety between API and frontend
  - Single source of truth for domain models
  - Prevents API contract drift

- **Dependency management**:
  - `WaitFor(apiService)` ensures Web doesn't start until API is healthy
  - HTTP health checks monitor both services
  - Graceful startup ordering

## Alternatives Considered

1. **Manual service management** — Rejected: too much boilerplate, no observability
2. **Docker Compose** — Rejected: less integrated with .NET tooling, manual telemetry setup
3. **Kubernetes directly** — Rejected: overkill for development, steep learning curve

## Impact

- All services must reference ServiceDefaults and call `builder.AddServiceDefaults()`
- Health check endpoints (`/health`, `/alive`) are standard across all services
- AppHost is the entry point for development (`dotnet run` from AppHost)
- Future services (SignalR, additional APIs) follow this pattern

## Team Notes

- This unblocks issues #3, #4, #5, #6 which all depend on the base structure
- Database will be added to AppHost as a resource in issue #3
- SignalR hub will be configured in AppHost in issue #12
