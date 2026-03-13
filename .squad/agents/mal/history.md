# Mal's Project Knowledge

## Project Overview

**Project**: Kingmaker-Kingdom-Sheet  
**User**: Mike Squibb  
**Tech Stack**: .NET 10, Blazor Web App (Auto render mode), Aspire 13, SQLite, SignalR  

**Purpose**: A kingdom management tool for Pathfinder 2 Kingmaker GMs and players. GMs create and manage kingdoms with hex-based maps featuring fog of war. Players can view shared kingdoms (read-only, with limited role-based actions). The centerpiece is an interactive SVG hex grid where GMs can toggle fog, claim hexes, and apply upgrades.

**Key Features**:
- GM/Player role-based authentication (ASP.NET Core Identity)
- Kingdom and town tracking with stats
- Large SVG hex grid map with fog of war effect
- Hex claiming and upgrade system
- Real-time updates via SignalR
- SQLite database for simplicity

## Architecture Decisions

### 2026-03-06: Initial Plan Review
- **Approved**: .NET 10 + Aspire 13 + Blazor Auto + SQLite + SignalR stack
- **Approved**: SVG-based hex grid (good trade-off for <500 hexes typical in Kingmaker)
- **Approved**: 6-project structure separating concerns cleanly
- **Noted**: Old-Version exists at `Kingmaker-Kingdom-Sheet/Old-Version/` for domain reference

## Recent Updates

### 2026-03-13: Foundation Batch Completion

**Mal's Contribution**:
- Documented database-first SQL + Aspire pattern (Decision #2)
- Defined handoff to Book (schema design) and Wash (DbContext + AppHost integration)

**Wash's Contribution**:
- Scaffolded Domain layer with core entity models
- Scaffolded Application layer with service contracts and preview implementations
- Solution now builds with 6 projects (AppHost, ServiceDefaults, ApiService, Web, Shared, Domain, Application)
- Ready to integrate DbContext after Book delivers schema

**Book's Contribution**:
- Created SQL Database project with Microsoft.Build.Sql
- Designed multi-GM schema with app + ref schema split
- Tables: Kingdom, KingdomParticipant, KingdomParticipantRole, Hex, Settlement, HexUpgrade
- Solution builds cleanly with 7 projects total

**Next Steps**:
- Wash to set up DbContext and AppHost database integration using Book's schema
- Database will be tied to AppHost as a SQLite resource

### Earlier Updates
📌 **2026-03-07**: Aspire Architecture Pattern implemented (Decision #1)
📌 **2026-03-06**: Team initialized; 22 todos organized into 5 milestones

- **SQL Database Project (SSDT)** is the canonical schema repository; EF Core DbContext maps to it (no migrations)
- **AppHost integration**: SQLite resource added via `AddSqlite()`, bound to ApiService with `WithReference()`
- **Multi-GM pattern**: `KingdomGms` junction table with role column (`owner|editor|viewer`) enables flexible ownership
- **Development seeding**: Stored scripts in Database/Scripts/, initialized at AppHost startup
- **Hand-written DbContext**: Not scaffolded; explicit entity mappings maintain code/schema alignment

### Project Structure
- Old Blazor WebAssembly project: `Kingmaker-Kingdom-Sheet/Old-Version/` (Client/Server/Shared pattern)
- New .NET 10 work will live in `dotnet10/` directory
- `dotnet10/` currently scaffolded (5 projects): AppHost, ServiceDefaults, ApiService, Web, Shared
- Database project will be added alongside existing 5; all under `dotnet10/`

### Priority Recommendations
1. Foundation first: aspire-setup → solution-structure → domain-models
2. Vertical slice approach: Get one hex visible before building all APIs
3. SignalR complexity deferred to Phase 2 (hex-sync)

### Risks Identified
- Blazor Auto render mode adds debugging complexity (server vs WASM context)
- Identity + custom roles may need careful planning for GM ownership model

### Kingdom Ownership Clarification (2026-03-06)
- **User Input (Mike Squibb)**: Primarily single-owner kingdoms are the norm, but multi-GM management must be supported as an edge case
- **Architectural Implication**: Identity system must treat kingdom ownership as a many-to-many relationship (Kingdom ↔ GMs) rather than one-to-one
- **Auth Model**: Permission checks will be: "Is the current user a GM of *this* kingdom?" rather than "Is this the owner?"
- **Identity-Setup Impact**: Affects seed data generation, user context binding, and claim setup in ASP.NET Core Identity

### Milestone Planning (2026-03-06)
- **Organized 22 todos into 5 milestones** (M1 Foundation → M5 Polish, ~12 weeks total)
- **M1 (2–3 weeks)**: Project scaffold, SQLite, Identity configured
- **M2 (3–4 weeks)**: API & Blazor foundation, authentication flows
- **M3 (4–5 weeks)**: Kingdom management UI, hex grid rendering & interaction
- **M4 (3–4 weeks)**: Real-time updates (SignalR), player role actions
- **M5 (2–3 weeks)**: Responsive design, error handling, test coverage
- **17 GitHub issues recommended** (user-facing features + critical infrastructure); 8 kept as internal implementation todos
- **Critical path**: aspire-setup → solution-structure → api-project → blazor-project → hex-grid-component → hex-sync
- **Risks flagged**: Identity complexity (multi-GM model) + Blazor Auto mode debugging + SVG coordinate system clarity needed upfront

### GitHub Milestones Created (2026-03-06)
- **M1: Foundation (Weeks 1-2)** — Milestone #1
- **M2: Core API & Frontend (Weeks 3-5)** — Milestone #2
- **M3: Kingdom & Hex Maps (Weeks 6-8)** — Milestone #3
- **M4: Real-Time & Players (Weeks 9-10)** — Milestone #4
- **M5: Polish (Weeks 11-12)** — Milestone #5

### GitHub Issues Created (2026-03-06)
**M1 Foundation:**
- #2: Set up Aspire orchestration and base project structure (squad:wash)
- #3: Configure SQLite database and Entity Framework Core (squad:wash)
- #4: Implement ASP.NET Core Identity and role-based access control (squad:wash)
- #5: Create Blazor Web App project with Auto render mode (squad:kaylee)

**M2 Core API & Frontend:**
- #6: Build kingdom and town API endpoints (squad:wash)
- #7: Create Blazor component library and layout system (squad:kaylee)
- #8: Implement authentication UI (login, register, logout) (squad:kaylee)

**M3 Kingdom & Hex Maps:**
- #9: Build kingdom management dashboard and town list UI (squad:kaylee)
- #10: Render SVG hex grid with fog of war (squad:kaylee)
- #11: Implement hex claiming and upgrade system (squad:kaylee)

**M4 Real-Time & Players:**
- #12: Integrate SignalR for real-time hex synchronization (squad:wash)
- #13: Build player-read-only kingdom view and dashboard (squad:kaylee)
- #14: Implement player role-based actions and permissions (squad:wash)

**M5 Polish:**
- #15: Implement responsive design and mobile support (squad:kaylee)
- #16: Add comprehensive error handling and user feedback (squad:wash,squad:kaylee)
- #17: Create API documentation and user guide (squad:mal)
- #18: Performance optimization and test coverage review (squad:zoe,squad:wash)
