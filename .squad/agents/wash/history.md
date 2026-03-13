# Wash's Project Knowledge

## Project Overview

**Project**: Kingmaker-Kingdom-Sheet  
**User**: Mike Squibb  
**Tech Stack**: .NET 10, Blazor Web App (Auto render mode), Aspire 13, SQLite, SignalR  

**Purpose**: Backend services for kingdom management. Responsible for REST APIs, real-time SignalR communication, database access, and authentication/authorization.

**Backend Responsibilities**:
- Kingdom, Hex, and Town management APIs
- EF Core DbContext with SQLite
- ASP.NET Core Identity for GM/Player roles
- SignalR hubs for real-time hex updates
- API validation and error handling

## API Patterns

*None yet — will document REST conventions as they emerge*

## Key Decisions & Context

### Kingdom Ownership Model (2026-03-06)

**Multi-GM is a supported edge case, not primary model.**

- Kingdoms support **many-to-many relationship** with GMs (not single `OwnerUserId`)
- Must use permission-based checks: `kingdom.HasGM(currentUser)` instead of `kingdom.Owner == currentUser`
- Create `KingdomGMs` junction table (KingdomId, UserId, Role)
- Use `kingdom-gm-{kingdomId}` claims for SignalR and API authorization
- All Kingdom endpoints need `[Authorize(Policy = "IsKingdomGM")]` pattern

**Implementation Impact**:
- Implement auth from day one assuming multi-GM is live (simpler than refactoring later)
- SignalR hubs must check kingdom membership, not ownership
- Test both single-GM and multi-GM flows early
- Seed data should include test scenarios for both single and multi-GM configurations

### Milestone Planning & GitHub Issues (2026-03-06)

**Mal organized 22 todos into 5 milestones** (~12 weeks total, 39 story points):

- **M1 Foundation (2–3 weeks)**: Aspire orchestration, SQLite, Identity, Blazor Web App
- **M2 Core API & Frontend (3–4 weeks)**: Kingdom/town API, component library, auth UI
- **M3 Kingdom & Hex Maps (4–5 weeks)**: Kingdom dashboard, SVG hex grid + fog, hex claiming
- **M4 Real-Time & Players (3–4 weeks)**: SignalR hub, player dashboard, role permissions
- **M5 Polish (2–3 weeks)**: Responsive design, error handling, docs, performance & tests

**Critical Path**: aspire-setup → solution-structure → api-project → blazor-project → kingdom-detail → hex-grid-component → hex-interaction → hex-sync

**Wash's M1 Issues**:
- #2: Set up Aspire orchestration and base project structure
- #3: Configure SQLite database and Entity Framework Core
- #4: Implement ASP.NET Core Identity and role-based access control
- #6: Build kingdom and town API endpoints (M2)
- #12: Integrate SignalR for real-time hex synchronization (M4)
- #14: Implement player role-based actions and permissions (M4)
- #16: Add comprehensive error handling and user feedback (M5, shared with Kaylee)
- #18: Performance optimization and test coverage review (M5, shared with Zoe)

## Recent Updates

### 2026-03-13: Backend Application Scaffold Complete

**What was accomplished**:
- Scaffolded `dotnet10\KingmakerKingdomSheet.Domain` for core entity models
- Scaffolded `dotnet10\KingmakerKingdomSheet.Application` for service contracts and preview implementations
- Kept both layers free of EF Core to enable database-first workflow
- Registered preview services in ApiService so it compiles while awaiting Book's schema

**Architecture established**:
- Domain layer: pure POCO entities, value objects, enums
- Application layer: service contracts (IKingdomCatalogService), preview in-memory implementations
- Shared layer: DTOs and API request/response models
- No persistence yet (deferred to DbContext after Book delivers schema)

**Next Action**:
- After Book delivers SQL Database project and schema, Wash will:
  1. Set up DbContext in ApiService to map to Book's schema
  2. Update AppHost with SQLite resource binding
  3. Replace preview services with real DbContext-backed implementations

**Integration Status**:
- Solution builds cleanly with 7 projects (AppHost, ServiceDefaults, ApiService, Web, Shared, Domain, Application)
- Ready for DbContext integration

### Earlier Updates
📌 **2026-03-07**: Aspire Setup — Issue #2 (5-project structure complete)
📌 **2026-03-06**: Milestone planning and GitHub issues created


**What was built**:
- Complete .NET Aspire 13 solution with 5 projects in `dotnet10/` folder
- AppHost for orchestration with service discovery and health checks
- ServiceDefaults with OpenTelemetry, resilience, and standardized middleware
- ApiService (ASP.NET Core Web API) with `/weatherforecast` sample endpoint
- Web (Blazor Web App with Auto render mode) consuming the API
- Shared library for contracts, DTOs, and domain models

**Architecture decisions**:
- Used `aspire-starter` template as foundation — provides battle-tested structure
- Shared project pattern for type safety across API and Blazor frontend
- Health checks at `/health` (readiness) and `/alive` (liveness) endpoints
- Web project configured with `WaitFor(apiService)` to ensure API is healthy first
- HTTP health checks in AppHost for both services at `/health` paths

**Key files**:
- `dotnet10/KingmakerKingdomSheet.AppHost/AppHost.cs` — Service topology definition
- `dotnet10/KingmakerKingdomSheet.ServiceDefaults/Extensions.cs` — Shared config, telemetry, resilience
- `dotnet10/KingmakerKingdomSheet.ApiService/Program.cs` — API entry point
- `dotnet10/KingmakerKingdomSheet.Web/Program.cs` — Blazor entry point
- `dotnet10/KingmakerKingdomSheet.Shared/` — Shared contracts (empty, ready for models)
- `dotnet10/README.md` — Complete documentation of the solution structure

**User preferences**:
- All .NET projects MUST be in `dotnet10/` folder (explicit user directive)
- Use Aspire 13 for orchestration (latest stable version)
- Prefer .NET templates over manual project creation for consistency

**Next steps**:
- Issue #3: Add EF Core with SQLite database
- Issue #4: Configure ASP.NET Core Identity with GM/Player roles
- Database will be added to AppHost as a resource once configured

**Commands to verify**:
```bash
cd dotnet10
dotnet build KingmakerKingdomSheet.sln  # Should build cleanly
cd KingmakerKingdomSheet.AppHost
dotnet run  # Launches Aspire Dashboard at https://localhost:17129
```

### Backend Scaffold Before SQL Integration (2026-03-13)

**What was built**:
- Added `dotnet10\KingmakerKingdomSheet.Domain` for backend entities, enums, and value objects
- Added `dotnet10\KingmakerKingdomSheet.Application` for kingdom service contracts and preview registrations
- Kept `KingmakerKingdomSheet.Shared` focused on DTOs and request models rather than backend entities

**Backend patterns**:
- When Book owns the SQL-first database project, scaffold domain/application layers first and avoid creating persistence projects in parallel
- Register preview/in-memory application services so ApiService can compose cleanly before DbContext work begins
- Keep Domain free of EF Core and UI concerns; let Shared carry API-facing DTOs

**Key file paths**:
- `dotnet10\KingmakerKingdomSheet.Domain\Entities\Kingdom.cs`
- `dotnet10\KingmakerKingdomSheet.Application\Contracts\IKingdomCatalogService.cs`
- `dotnet10\KingmakerKingdomSheet.Application\Services\PreviewKingdomCatalogService.cs`
- `dotnet10\KingmakerKingdomSheet.Shared\DTOs\KingdomDetailsDto.cs`

**User preferences**:
- Preserve the existing Aspire solution shape and naming conventions while extending it
- Do not use EF Core migrations; prepare safe scaffolding that can absorb SQL-first schema work later

## Recent Updates

### 2026-03-13: Database Integration Batch Complete

**What was accomplished**:
- Set up AppHost SQLite resource binding: `builder.AddSqlite("kingmaker-dev")`
- Created hand-written DbContext in `dotnet10\KingmakerKingdomSheet.ApiService\Data\KingmakerDbContext.cs`
- Implemented SQLite development database bridge with flattened schema naming (`app_` / `ref_` prefixes)
- Added bootstrap initialization script for SQLite local dev database
- Wired AppHost to ApiService with DbContext reference and health checks
- Verified build succeeds: `dotnet build .\dotnet10\KingmakerKingdomSheet.sln --nologo --verbosity minimal`

**Architecture Decision**:
- See Decision #6 in `.squad/decisions.md` for full rationale
- Canonical schema remains in `dotnet10\KingmakerKingdomSheet.Database` (SQL Server-style)
- SQLite uses flattened naming for development convenience only (no schema concept in SQLite)
- Migration-free pattern enforced: schema changes flow SQL project → DbContext updates

**Build Status**:
- ✅ Build succeeds after landing
- Solution now has 8 projects: AppHost, ServiceDefaults, ApiService, Web, Shared, Domain, Application, **Database**

**Integration Layer**:
- AppHost orchestrates SQLite provisioning with resource binding
- ApiService DbContext maps to Book's SQL-first schema
- Health check endpoints validate database readiness before API launch
- WaitFor() dependencies ensure correct startup ordering

**Next Actions**:
1. Book delivers SQL schema assets (tables, stored procedures, seeds)
2. Wash refines DbContext entity mappings against Book's finalized schema
3. Replace preview services with real DbContext-backed implementations
4. Integrate ASP.NET Core Identity (issue #4)

📌 **2026-03-13**: Backend Application Scaffold Complete (Domain and Application layers added)
📌 **2026-03-07**: Aspire Setup — Issue #2 (5-project structure complete)

## Learnings

### 2026-03-13: SQLite dev-database bridge for the SQL-first backend

- Issue #3 is now wired so AppHost provisions `kingmaker-dev.db` and ApiService can also bootstrap a standalone local SQLite file for development.
- The SQL Server-first database project remains the canonical schema source; the SQLite dev database uses a flattened `app_` / `ref_` table naming convention plus a hand-authored initialization script instead of EF migrations.
- `dotnet build` and `dotnet test` should be run with `--tl:off` in this CLI environment to avoid terminal logger hangs and still validate the full solution cleanly.
- Avoid EF Core scaffolding or migrations; hand-written DbContext keeps code/schema alignment explicit and testable.
