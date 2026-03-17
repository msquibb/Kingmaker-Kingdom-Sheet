---
name: "sdk-style-sql-project-foundation"
description: "Create a database-first SQL project with Microsoft.Build.Sql and seeded lookup data"
domain: "database"
confidence: "high"
source: "project-implementation"
---

## Context

Use this pattern when the team wants schema-first development, no EF Core migrations, and a buildable database artifact that fits normal `dotnet` workflows.

## Pattern

1. Scaffold an SDK-style SQL project with `Microsoft.Build.Sql`.
2. Separate mutable tables and seeded reference tables into different schemas.
3. Model access-heavy many-to-many concerns with junction tables instead of single foreign keys.
4. Seed lookup data from post-deployment SQLCMD includes and remove those seed fragments from model compilation.
5. Document the handoff so application code maps to deployed schema objects rather than generating migrations.
6. Validate both the `.sqlproj` and the full solution build after adding the project.

## Benefits

- Keeps schema history in source control.
- Produces a `.dacpac` for repeatable deployment.
- Makes fixed lookup IDs explicit for permissions, fog states, and claim states.

## Gotchas

- `Microsoft.Build.Sql` targets the SQL Server-family dialect through DacFx, so any prior SQLite plans need an explicit integration decision.
- Files referenced by `:r` in post-deployment scripts must be excluded from normal model build input.
