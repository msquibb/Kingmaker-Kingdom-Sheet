# KingmakerKingdomSheet.Shared

This project contains shared contracts, DTOs, and models used by both the API and Blazor frontend.

## Structure

- `Models/` - Domain entities and data models
- `DTOs/` - Data Transfer Objects for API requests/responses
- `Contracts/` - Interfaces and service contracts

## Usage

Reference this project from:
- `KingmakerKingdomSheet.ApiService` - API endpoints
- `KingmakerKingdomSheet.Web` - Blazor components

This ensures type safety and shared domain knowledge across the full stack.
