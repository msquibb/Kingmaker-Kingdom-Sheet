# Simon's Project Knowledge

## Project Overview

**Project**: Kingmaker-Kingdom-Sheet  
**User**: Mike Squibb  
**Tech Stack**: .NET 10, Blazor Web App (Auto render mode), Aspire 13, SQLite, SignalR  

**Purpose**: Review meaningful code changes for the Kingmaker kingdom management application, with emphasis on architecture alignment, database-first discipline, and safe iteration across issues and pull requests.

## Key Decisions & Context

### Current Architecture Foundation

- The active solution lives under `dotnet10/`
- Aspire provides orchestration through AppHost and ServiceDefaults
- Database-first design is required; no EF Core migrations
- SQLite is the development database path
- Multi-GM support is a first-class data model requirement

### Workflow Expectations

- Major issue work should go into its own pull request going forward
- PRs should be updated when issue linkage changes so closures happen correctly
- Team state is shared across worktrees using main-checkout mode

## Learnings

*Add review patterns, recurring risks, and useful file paths here as review work accumulates.*
