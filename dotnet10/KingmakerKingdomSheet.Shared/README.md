# KingmakerKingdomSheet.Shared

This project contains shared API contracts and DTOs used by both the API and Blazor frontend.

## Structure

- `DTOs/` - Data Transfer Objects for API requests/responses
- `Requests/` - Input models for future API commands

Domain entities now live in `KingmakerKingdomSheet.Domain` so backend modeling can evolve without forcing UI-specific references onto the domain layer.

## Usage

Reference this project from:
- `KingmakerKingdomSheet.ApiService` - API endpoints
- `KingmakerKingdomSheet.Web` - Blazor components

This ensures type safety and shared domain knowledge across the full stack.
