# Book — Project History

## Project Context

**Project:** Kingmaker-Kingdom-Sheet  
**Tech Stack:** .NET 10, Blazor Web App (Auto render mode), Aspire 13, SQLite (dev), SQL Server (prod), SignalR  
**Description:** Pathfinder 2 Kingmaker kingdom management tool for GMs and players. Features hex grid map with fog of war, kingdom stats tracking, role-based access control, and real-time updates.  
**User:** Mike Squibb  
**Role:** SQL Design Expert — Database schema design, SQL projects, data modeling

## Key Decisions

- **Database-First Approach**: Schema defined in SQL Server Database Project (`.sqlproj`), EF Core scaffolds from database. No EF Core Migrations.
- **Kingdom Ownership**: Many-to-many relationship between Users and Kingdoms via `KingdomGameMasters` junction table (supports multiple GMs per kingdom, multiple kingdoms per GM). Include role/permission fields in junction table.
- **Development Database**: SQLite for local development, SQL Server for production.
- **Aspire Orchestration**: All projects managed by Aspire AppHost for service discovery and health monitoring.

## Learnings

*Learnings from project work will be added here as the agent completes tasks.*
# Book's Project Knowledge

## Project Overview

**Project**: Kingmaker-Kingdom-Sheet  
**User**: Mike Squibb  
**Tech Stack**: .NET 10, Blazor Web App (Auto render mode), Aspire 13, SQLite, SignalR  

**Purpose**: Design the database-first foundation for the Kingmaker Kingdom Sheet, including schema assets for kingdoms, hexes, towns, user permissions, and shared kingdom management.

## Key Decisions & Context

### Database-First Directive (2026-03-13)

- The project will use a database-first workflow
- Do not use EF Core Migrations
- Create and maintain schema assets in a SQL database project under `dotnet10/`
- Plan for multi-GM ownership with explicit relationship tables and permission data

### Kingdom Ownership Model (2026-03-06)

- Kingdoms support many-to-many relationship with GMs
- A junction table is required for kingdom-to-GM assignments
- Role/permission data belongs with the assignment, not just the user

## Recent Updates

### 2026-03-13: SQL Database Project & Schema Foundation Delivered

**What was built**:
- Created `dotnet10\KingmakerKingdomSheet.Database` as SQL-first schema source of truth
- Microsoft.Build.Sql project outputs `.dacpac` on build
- Designed multi-GM-ready schema with app + ref schema split

**Schema Design**:
- **app schema (mutable gameplay)**:
  - `Kingdom` — kingdoms with base metadata
  - `KingdomParticipant` — GM/Player assignments (many-to-many)
  - `KingdomParticipantRole` — role assignments for participants
  - `Hex` — hex grid coordinates, terrain, ownership state
  - `Settlement` — towns/cities on hexes
  - `HexUpgrade` — improvements to hexes

- **ref schema (lookup/reference)**:
  - `KingdomRole` — standard roles (Owner, Editor, Viewer)
  - `KingdomRoleDefaultPermission` — default grants per role
  - `KingdomParticipantPermission` — per-participant permission overrides

**Key Implementation Paths**:
- Table definitions: `dotnet10\KingmakerKingdomSheet.Database\Tables\`
- Seed data: `dotnet10\KingmakerKingdomSheet.Database\PostDeployment\Seeds\ReferenceData.sql`
- Documentation: `dotnet10\KingmakerKingdomSheet.Database\README.md`

**Status**:
- Solution builds cleanly with 7 projects
- Schema canonical and ready for Wash to build DbContext against
- Multi-GM ownership modeled via KingdomParticipant + KingdomParticipantRole

**Next Action**:
- Wash will create DbContext and AppHost SQLite integration using this schema as the source of truth

### Earlier Updates
📌 **2026-03-13**: Decided to use database-first pattern with no EF Core migrations (Decision #2)
📌 **2026-03-07**: Designed multi-GM ownership model and core schema structure

- Split SQL objects into `app` (mutable gameplay entities) and `ref` (seeded lookup/reference data) schemas to keep application mappings predictable.
- Multi-GM support is modeled through `app.KingdomParticipant` + `app.KingdomParticipantRole`, with default grants in `ref.KingdomRoleDefaultPermission` and per-participant overrides in `app.KingdomParticipantPermission`.
- Kingdom map ownership is anchored by `app.Hex`, with `app.Settlement` enforcing same-kingdom placement through a composite foreign key and `app.HexUpgrade` carrying improvement state.
- Key implementation paths: `dotnet10\KingmakerKingdomSheet.Database\Tables\`, `dotnet10\KingmakerKingdomSheet.Database\PostDeployment\Seeds\ReferenceData.sql`, and `dotnet10\KingmakerKingdomSheet.Database\README.md`.
