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

## Benefits

- Respects database-first ownership boundaries.
- Avoids premature persistence design.
- Unblocks API composition, contracts, and testing harnesses.
