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

## Learnings

*Will be filled as backend services are built*
