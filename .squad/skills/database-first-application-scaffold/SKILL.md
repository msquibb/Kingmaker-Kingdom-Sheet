---
name: "database-first-application-scaffold"
description: "Scaffold domain and application projects ahead of SQL-first integration"
domain: "backend"
confidence: "medium"
source: "project-implementation"
---

## Context

Use this pattern when a .NET solution must keep moving on backend structure, but the database/schema project is owned by a separate SQL-first workflow and EF Core migrations are not allowed.

## Pattern

1. Keep the SQL/database project out of scope until schema assets are delivered.
2. Add a `Domain` class library for entities, enums, and value objects with no EF Core or UI dependencies.
3. Add an `Application` class library that references `Domain` plus shared DTO/contracts.
4. Register preview or in-memory services in the application layer so the API composition root can build before persistence exists.
5. Keep `Shared` focused on API-facing DTOs/requests, not backend domain entities.
6. Document the exact handoff needed to swap preview services for DbContext-backed implementations after schema delivery.
7. When the canonical SQL project targets SQL Server but local development needs SQLite, add a separate handwritten SQLite bootstrap script and flatten schema-qualified names (for example `app.Kingdom` → `app_Kingdom`) instead of modifying the source-of-truth SQL assets.
8. Keep DbContext mappings handwritten and migration-free; wire them to the SQLite bootstrap script plus AppHost resource injection.

## Benefits

- Respects database-first ownership boundaries.
- Avoids premature persistence design.
- Unblocks API composition, contracts, and testing harnesses.
- Lets local development run against a lightweight SQLite database while preserving the SQL project as the schema authority.

## Testing Notes

When production persistence is still deferred, add only low-risk tests that validate current contracts:

1. Domain value objects and immutable record construction
2. Dependency injection registration for preview services
3. Preview service outputs and DTO mapping shape

Avoid speculative tests for repositories, EF mappings, migrations, or endpoint behavior until the SQL-backed implementation exists.
