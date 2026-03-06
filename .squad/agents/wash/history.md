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

## Learnings

*Will be filled as backend services are built*
