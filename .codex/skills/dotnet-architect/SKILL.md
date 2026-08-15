---
name: dotnet-architect
description: Design and review ResumeEnhancer solutions as a .NET modular monolith with clean layering, Minimal APIs, Mediator, EF Core, and explicit composition. Use when Codex needs framework-aware architecture guidance for backend or cross-layer changes.
---

# Dotnet Architect

Use this skill when architecture guidance should be grounded in the actual .NET stack and conventions used in this repository.

## Use this skill when

- a backend or cross-layer change needs .NET-specific architecture judgment
- the question touches composition, endpoint style, service boundaries, or persistence integration
- you need design guidance that fits the existing modular monolith

## Do not use this skill when

- the task is not meaningfully .NET or backend related
- a smaller implementation-focused skill is enough

## Architecture workflow

1. Clarify the goal, constraints, and affected layers.
2. Apply repository-specific rules before generic framework advice.
3. Check dependency direction, composition root behavior, and lifetime management.
4. Review contracts, validation, handlers, mapping, and persistence boundaries together.
5. Open `dotnet-backend-patterns` references when deeper implementation patterns are needed.

## Review lenses

- Minimal API endpoint discipline
- handler and service-layer cohesion
- EF Core and repository integration
- explicit composition and testability
- performance and operational tradeoffs

## ResumeEnhancer focus

- module registration and startup composition
- separation of Web, AM, SL, PL, and DM
- migration and seeding consequences
- contract and frontend integration stability