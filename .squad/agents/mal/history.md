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

## Learnings

### Project Structure
- Old Blazor WebAssembly project: `Kingmaker-Kingdom-Sheet/Old-Version/` (Client/Server/Shared pattern)
- New .NET 10 work will live in `dotnet10/` directory
- `dotnet10/` currently empty - greenfield start

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
