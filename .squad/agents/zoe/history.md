# Zoe's Project Knowledge

## Project Overview

**Project**: Kingmaker-Kingdom-Sheet  
**User**: Mike Squibb  
**Tech Stack**: .NET 10, Blazor Web App (Auto render mode), Aspire 13, SQLite, SignalR  

**Purpose**: Testing and quality assurance for the kingdom management tool. Ensure all features work correctly through unit and integration tests.

**Testing Responsibilities**:
- Unit tests for domain models (Kingdom, Hex, Town, KingdomRole)
- Integration tests for API endpoints
- SignalR hub connection and broadcast tests
- Edge case validation (boundary conditions, error scenarios)
- Test maintenance as features evolve

## Test Patterns

*None yet — will document testing conventions as they emerge*

## Milestone Planning & GitHub Issues (2026-03-06)

**Mal organized 22 todos into 5 milestones** (~12 weeks total, 39 story points):

- **M1 Foundation (2–3 weeks)**: Aspire orchestration, SQLite, Identity, Blazor Web App
- **M2 Core API & Frontend (3–4 weeks)**: Kingdom/town API, component library, auth UI
- **M3 Kingdom & Hex Maps (4–5 weeks)**: Kingdom dashboard, SVG hex grid + fog, hex claiming
- **M4 Real-Time & Players (3–4 weeks)**: SignalR hub, player dashboard, role permissions
- **M5 Polish (2–3 weeks)**: Responsive design, error handling, docs, performance & tests

**Critical Path**: aspire-setup → solution-structure → api-project → blazor-project → kingdom-detail → hex-grid-component → hex-interaction → hex-sync

**Zoe's M5 Issues**:
- #18: Performance optimization and test coverage review (M5, shared with Wash)

**Testing Priorities** (from milestone planning):
- Unit tests for domain models (Kingdom, Hex, Town, KingdomRole) — foundation in M5
- Integration tests for API endpoints — paired with each API issue (M2–M4)
- SignalR hub connection and broadcast tests — with #12 (SignalR integration)
- Multi-GM edge case validation — critical path for identity-setup (#4) and hex-sync (#12)
- Edge cases: boundary conditions (hex grid limits), error scenarios (permission failures)

## Architecture Decisions Affecting Zoe

**Aspire Architecture Pattern** (Wash, 2026-03-07):
- All services must integrate with ServiceDefaults for telemetry and health checks
- Test infrastructure should leverage Aspire's OpenTelemetry for observability
- Health check endpoints are standard: `/health` (detailed), `/alive` (liveness)
- Integration tests should validate WaitFor() dependency ordering

## Learnings

*Will be filled as tests are written*
