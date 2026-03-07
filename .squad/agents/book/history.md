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
