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

### 2. Database-First Architecture Foundation

**Date**: 2026-03-13  
**Author**: Mal  
**Status**: Proposed  
**Milestone**: M1 Foundation  
**Related Issues**: #3, #4  

Implement a **hybrid SQL + Aspire pattern** with these components:

1. **SQL Database Project (SSDT)** — `KingmakerKingdomSheet.Database`
   - Single source of truth for schema, constraints, and relationships
   - Split into `app` (mutable gameplay entities) and `ref` (lookup/reference data) schemas
   - Outputs `.dacpac` on build via `Microsoft.Build.Sql`

2. **AppHost Database Resource**
   - Add SQLite resource to AppHost: `builder.AddSqlite("kingmaker-dev")`
   - Bind to ApiService via `WithReference()`
   - Health check endpoint for database readiness

3. **ApiService Hand-Written DbContext** (Code-First on Top of Schema)
   - Not scaffolded; explicit entity mappings maintain code/schema alignment
   - No migrations used; schema changes flow schema-first → DbContext updates
   - Fluent API to enforce constraints and relationships

4. **Multi-GM Ownership Pattern**
   - Junction table: `KingdomParticipant` (KingdomId, UserId, Role)
   - Permission model: Default grants in `ref.KingdomRoleDefaultPermission` + per-participant overrides
   - Access check: `WHERE KingdomId IN (SELECT KingdomId FROM KingdomParticipant WHERE UserId = @userId)`

5. **Development Workflow**
   - Schema changes: Edit SQL project → regenerate `.dacpac`
   - Run dev: `dotnet run --project AppHost` → Aspire launches SQLite, initializes schema
   - Seed data: Stored procedures/scripts bundled in Database/Scripts/

**Rationale**:
- Honors the database-first directive and no-migrations constraint
- SQL project is canonical; code follows schema
- Aspire integration manages SQLite resource with health/discovery patterns
- Hand-written DbContext maintains code/schema alignment without scaffolding
- Multi-GM ready from day 1

**Handoff**:
- **Book**: Design and script core schema (tables, constraints, seeds)
- **Wash**: Set up DbContext and AppHost integration after Book delivers schema

---

### 3. Backend Application Scaffold Before SQL Integration

**Date**: 2026-03-13  
**Author**: Wash  
**Status**: Implemented  

Add `KingmakerKingdomSheet.Domain` and `KingmakerKingdomSheet.Application` under `dotnet10/` now, while explicitly deferring persistence work until Book lands SQL-first assets.

**Rationale**:
- Keeps backend progress moving without violating the database-first/no-migrations decision
- Separates domain entities from shared API DTOs
- Gives ApiService a stable application-layer reference point for future endpoints

**Key Decisions**:
- Register preview/in-memory application services so ApiService compiles before DbContext work begins
- Keep Domain free of EF Core and UI concerns; let Shared carry API-facing DTOs

**Follow-up**:
1. Book adds schema assets
2. Wash wires AppHost + ApiService to delivered schema
3. Replace preview services with DbContext-backed implementations

---

### 4. Schema Foundation with SDK-Style SQL Project

**Date**: 2026-03-13  
**Author**: Book  
**Status**: Implemented  

Use `Microsoft.Build.Sql` database project as the canonical schema source under `dotnet10\KingmakerKingdomSheet.Database`.

**Design**:
- `app` schema for mutable domain tables (Kingdom, KingdomParticipant, Hex, Settlement, HexUpgrade)
- `ref` schema for lookup/reference data (KingdomRole, KingdomRoleDefaultPermission)
- Explicit multi-GM support through participant + role assignment tables
- Role-default permissions plus participant-level permission overrides
- Kingdom-owned hexes, settlements, and upgrades with foreign keys and targeted indexes

**Delivery**:
- Generated `.dacpac` on build
- Schema relationships documented in `Database/README.md`
- Seeds and scripts in `Database/PostDeployment/` and `Database/Scripts/`

**Note**: Local development will consume `.dacpac` via AppHost SQLite resource initialization.

---

### 5. Test Foundation for Database-First Scaffold

**Date**: 2026-03-13  
**Author**: Zoe  
**Status**: Implemented  

For the current database-first scaffold state, automated coverage should start with two lightweight unit-test projects:

- `KingmakerKingdomSheet.Domain.Tests` — Domain primitives, immutable record construction, value objects
- `KingmakerKingdomSheet.Application.Tests` — Application DI registration, preview service contracts, DTO mapping shape

**Rationale**:
- Matches the database-first/no-migrations architecture already chosen
- Avoids locking the team into speculative persistence behavior
- Gives the solution a clean test entry point now without inventing production rules

**Scope**:
- ✅ Domain primitives and immutable record construction contracts
- ✅ Application DI registration
- ✅ Preview/in-memory service contracts and DTO mapping shape
- ❌ Do **not** add persistence-oriented or endpoint-oriented tests until SQL-backed implementation and API flows are in place

**Follow-up**:
1. Replace or expand preview-service tests when the real DbContext-backed application services land
2. Add integration tests for ApiService endpoints after auth and database wiring exist

---

### 6. SQLite Development Integration for the SQL-First Schema

**Date**: 2026-03-13  
**Author**: Wash  
**Status**: Implemented  

Local development database support that bridges the SQL Server-style schema into SQLite without modifying Book's SQL-first assets.

**Pattern**:
- Keep `dotnet10\KingmakerKingdomSheet.Database` as the **only canonical schema source** (SQL Server-style); do not edit Book's SQL assets just to satisfy SQLite development needs
- For local runtime, use **separate handwritten initialization script** under `KingmakerKingdomSheet.ApiService\Data\Sqlite\`
- **Flatten SQL schemas into table-name prefixes** (`app_` and `ref_`) for the SQLite development database so EF Core can map cleanly without relying on unsupported SQLite schemas
- **Keep EF Core migration-free**: Schema changes must continue to flow from SQL project updates first, then matching DbContext/bootstrap-script updates

**Implementation**:
- AppHost SQLite resource binding: `builder.AddSqlite("kingmaker-dev")`
- ApiService hand-written DbContext in `Data\KingmakerDbContext.cs` (not scaffolded)
- Health check endpoints validate database readiness
- Bootstrap script initializes schema on first run

**Rationale**:
- Honors the database-first directive and no-migrations constraint
- SQL project remains canonical; SQLite is a development convenience only
- Avoids EF Core scaffolding/migration coupling
- Clean separation of concerns: Book owns schema logic, Wash owns integration layer

---

## Governance

- All meaningful changes require team consensus
- Document architectural decisions here
- Keep history focused on work, decisions focused on direction
