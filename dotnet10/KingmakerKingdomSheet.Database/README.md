# KingmakerKingdomSheet.Database

SDK-style SQL database project using `Microsoft.Build.Sql` as the source-controlled schema authority for the application's core domain.

## What lives here

- `Schemas/` - logical database schemas (`app`, `ref`)
- `Tables/app/` - mutable gameplay entities and relationship tables
- `Tables/ref/` - seeded lookup/reference tables
- `PostDeployment/` - idempotent seed scripts for fixed reference data

## Core model

The foundation currently covers:

- users via `app.UserAccount`
- kingdoms via `app.Kingdom`
- explicit multi-GM and player membership via `app.KingdomParticipant`, `app.KingdomParticipantRole`, and `app.KingdomParticipantPermission`
- map hexes, fog, claim state, and terrain via `app.Hex` plus `ref` lookup tables
- settlements/towns via `app.Settlement`
- hex upgrades via `app.HexUpgrade` and `ref.HexUpgradeType`

Reference data is seeded post-deployment for:

- claim states
- fog states
- terrain types
- hex upgrade types
- settlement types
- kingdom roles
- kingdom permissions
- default role-to-permission mappings

## Build

```powershell
dotnet build .\KingmakerKingdomSheet.Database.sqlproj
```

Successful builds produce a `.dacpac` under `bin\Debug\`.

## Publish

Deploy the generated `.dacpac` with `SqlPackage`:

```powershell
sqlpackage /Action:Publish /SourceFile:bin\Debug\KingmakerKingdomSheet.Database.dacpac /TargetConnectionString:"Server=(localdb)\MSSQLLocalDB;Integrated Security=true;Database=KingmakerKingdomSheet"
```

Install `SqlPackage` if needed:

```powershell
dotnet tool install -g microsoft.sqlpackage
```

## Wash integration note

- Treat this SQL project as the only schema source of truth; do not create EF Core migrations.
- App code should map to the deployed `app` and `ref` objects exactly, including constraint-sensitive names and composite relationships.
- `ref` tables are seeded lookup data and should be treated as read-only enum/reference sources in the API.
- Effective kingdom access should be resolved from participant role defaults plus `app.KingdomParticipantPermission` overrides.
- `ModifiedUtc` columns are application-maintained on updates; the schema only supplies creation defaults.
