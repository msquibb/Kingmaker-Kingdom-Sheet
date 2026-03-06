# Decisions

## 2026-03-06: Kingdom Ownership Model

**Author**: Mal (Lead)  
**Input**: Mike Squibb (User)  
**Stakeholder**: Wash (Backend/Identity Implementation)  
**Status**: Approved

### Clarification

Multi-GM kingdom management is a **supported edge case**, not the primary model.

- **Primary**: Single GM owns and manages a kingdom
- **Edge Case**: Multiple GMs collaboratively manage the same kingdom (exception to norm)

### Architectural Model

#### Ownership ≠ Single Owner

Kingdoms must support **many-to-many relationship with GMs**:

```
Kingdom ↔ (many) ↔ GM User
```

This means:
- A user can own/manage multiple kingdoms
- A kingdom can have multiple GMs with management rights
- Players can join kingdoms as read-only observers

#### Identity & Authorization

**Permission Check Pattern** (replace "ownership"):

```csharp
// NOT: kingdom.Owner == currentUser
// YES: kingdom.HasGM(currentUser)
```

All auth checks in the app must ask: *"Is the current user a GM for this kingdom?"* rather than *"Does this user own the kingdom?"*

#### Data Model Implications

- **Kingdom table**: Remove single `OwnerUserId` column
- **New junction table**: `KingdomGMs` (KingdomId, UserId, Role)
- **User context binding**: Load all kingdoms where `CurrentUser` is a GM on app startup
- **Claims**: Use `kingdom-gm-{kingdomId}` claims for SignalR and permission checks

### Impact on Identity-Setup Todo

The `identity-setup` todo must now include:

1. **Seed data**: Create test GMs with kingdoms (test single-GM and multi-GM scenarios)
2. **Junction table migration**: Plan `KingdomGMs` with optional role column (for future: Owner, Editor, Viewer)
3. **User context refresh**: When user logs in, load and cache all kingdom IDs where they're a GM
4. **Claim generation**: Issue kingdom-gm claims in token for authorization

### Why This Matters for Wash

- **No shortcuts**: Implement auth from day one assuming multi-GM is live (simpler than refactoring later)
- **SignalR security**: Hub method authorization must check kingdom membership, not ownership
- **API guards**: All Kingdom endpoints need `[Authorize(Policy = "IsKingdomGM")]` or similar
- **Testing**: Test both single-GM and multi-GM flows early to catch design gaps

### Risk Mitigation

- **Scope**: Multi-GM UI (team selection, permission management) deferred to Phase 2; implementation handles it gracefully
- **Migration**: Moving from single-owner to multi-owner post-launch is data-breaking; get this right now

---

## 2026-03-06: Initial Plan Architecture Approval

**Author**: Mal  
**Status**: Approved with recommendations

### Summary

The implementation plan for Kingmaker Kingdom Sheet is architecturally sound and ready for implementation with minor adjustments to priority and approach.

### What's Approved

1. **Tech Stack**: .NET 10, Aspire 13, Blazor Auto, SQLite, SignalR, EF Core
2. **Project Structure**: 6-project separation (AppHost, ServiceDefaults, Web, Api, Domain, Infrastructure, Shared)
3. **SVG Hex Grid**: Correct choice for Kingmaker-scale maps (<500 hexes)
4. **22 Todos**: Well-structured with reasonable dependencies

### Recommendations

#### Priority Order (Start Here)
1. `aspire-setup` — Wash should own this
2. `solution-structure` — Wash continues
3. `domain-models` — Wash models, Mal reviews
4. `database-context` + `identity-setup` — Can parallelize once domain is stable

#### Suggested Change: Add Vertical Slice Todo
After `blazor-project`, consider a thin vertical slice: one page that renders one hex. This proves the SVG approach before investing in full hex-grid-component complexity.

#### Risk Mitigations
- **Blazor Auto complexity**: Keep auth pages server-rendered, map WASM-only. Clear separation.
- **Identity GM/Player model**: Clarify if GM "owns" kingdoms or if it's permission-based. ✓ **RESOLVED** (see Kingdom Ownership Model decision)
- **SignalR deferred**: `hex-sync` should wait until basic hex interaction works locally.

### Dependencies to Watch

The plan's dependency graph is correct. Critical path:
```
aspire-setup → solution-structure → domain-models → database-context → api-project → blazor-project → hex-grid-component
```

Real-time (`realtime-hub`, `hex-sync`) can run parallel to hex rendering work.

### Team Assignments (Suggested)

- **Wash**: aspire-setup, solution-structure, database-context, api-project, kingdom-api, hex-api, realtime-hub
- **Kaylee**: blazor-project, auth-ui, kingdom-list, kingdom-detail, hex-grid-component, hex-rendering, hex-interaction
- **Zoe**: Testing strategy, domain-models validation, integration tests
- **Mal**: Architecture review, code review, blocking decisions

---

## 2026-03-06: User Directive — Project Organization

**Author**: Mike Squibb (via Copilot)  
**Status**: Active

### Directive

All projects and associated assets must be created within the `dotnet10\` folder. The solution should live within this repo folder, not at the root.

### Rationale

User request — captured for team memory. This ensures the .NET 10 implementation is properly organized within the repository structure.
