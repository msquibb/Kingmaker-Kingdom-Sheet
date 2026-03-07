# Squad Decisions

## Active Decisions

### 1. Aspire Architecture Pattern

**Date**: 2026-03-07  
**Author**: Wash  
**Status**: Implemented  

Use .NET Aspire 13 as the orchestration layer for the Kingmaker Kingdom Sheet application with a 5-project structure:

1. **AppHost** — Defines service topology and dependencies
2. **ServiceDefaults** — Shared configuration, telemetry, and resilience
3. **ApiService** — ASP.NET Core Web API backend
4. **Web** — Blazor Web App frontend (Auto render mode)
5. **Shared** — Contracts, DTOs, and domain models

**Rationale**:
- Aspire provides out-of-the-box service discovery, OpenTelemetry (logs, traces, metrics), health checks, and resilience patterns
- ServiceDefaults pattern ensures consistency across all services
- Shared project provides type safety between API and frontend
- WaitFor() dependencies ensure graceful startup ordering

**Alternatives Considered**:
1. Manual service management — Rejected: too much boilerplate, no observability
2. Docker Compose — Rejected: less integrated with .NET tooling, manual telemetry setup
3. Kubernetes directly — Rejected: overkill for development, steep learning curve

**Impact**:
- All services must reference ServiceDefaults and call `builder.AddServiceDefaults()`
- Health check endpoints (`/health`, `/alive`) are standard across all services
- AppHost is the entry point for development (`dotnet run` from AppHost)
- Unblocks issues #3, #4, #5, #6 which depend on the base structure
- Database will be added to AppHost as a resource in issue #3
- SignalR hub will be configured in AppHost in issue #12

## Governance

- All meaningful changes require team consensus
- Document architectural decisions here
- Keep history focused on work, decisions focused on direction
