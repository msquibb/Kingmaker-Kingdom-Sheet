### 2026-03-07T23:12:13Z: Database-first design directive
**By:** Mike Squibb (via Copilot)
**What:** Use database-first design approach with a SQL database project. Do NOT use EF Core Migrations. Schema is defined in SQL, EF Core consumes it.
**Why:** User preference — explicit control over database schema, better for complex database design patterns.
