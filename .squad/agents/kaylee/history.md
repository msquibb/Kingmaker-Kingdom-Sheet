# Kaylee's Project Knowledge

## Project Overview

**Project**: Kingmaker-Kingdom-Sheet  
**User**: Mike Squibb  
**Tech Stack**: .NET 10, Blazor Web App (Auto render mode), Aspire 13, SQLite, SignalR  

**Purpose**: Kingdom management tool for Pathfinder 2 Kingmaker. Frontend features include SVG hex grid map with fog of war, kingdom/town management pages, authentication UI, and real-time collaboration between GM and players.

**Frontend Responsibilities**:
- Blazor components for kingdom list, detail, and editor pages
- SVG hex grid with interactive fog of war, claiming, and upgrades
- Authentication pages (login, register, profile, role assignment)
- SignalR client integration for real-time updates
- Responsive design for desktop/tablet/mobile

## Component Patterns

*None yet — will document reusable patterns as they emerge*

## Milestone Planning & GitHub Issues (2026-03-06)

**Mal organized 22 todos into 5 milestones** (~12 weeks total, 39 story points):

- **M1 Foundation (2–3 weeks)**: Aspire orchestration, SQLite, Identity, Blazor Web App
- **M2 Core API & Frontend (3–4 weeks)**: Kingdom/town API, component library, auth UI
- **M3 Kingdom & Hex Maps (4–5 weeks)**: Kingdom dashboard, SVG hex grid + fog, hex claiming
- **M4 Real-Time & Players (3–4 weeks)**: SignalR hub, player dashboard, role permissions
- **M5 Polish (2–3 weeks)**: Responsive design, error handling, docs, performance & tests

**Critical Path**: aspire-setup → solution-structure → api-project → blazor-project → kingdom-detail → hex-grid-component → hex-interaction → hex-sync

**Kaylee's M1–M5 Issues**:
- #5: Create Blazor Web App project with Auto render mode (M1)
- #7: Create Blazor component library and layout system (M2)
- #8: Implement authentication UI (login, register, logout) (M2)
- #9: Build kingdom management dashboard and town list UI (M3)
- #10: Render SVG hex grid with fog of war (M3)
- #11: Implement hex claiming and upgrade system (M3)
- #13: Build player-read-only kingdom view and dashboard (M4)
- #15: Implement responsive design and mobile support (M5)
- #16: Add comprehensive error handling and user feedback (M5, shared with Wash)

## Learnings

*Will be filled as components are built*
