---
title: Architect Review Capability Expansion
intent: Maintain the conditional distributed-review and specialist-routing contract for architect review.
scope: `$architect-review` capability boundaries and supporting knowledge.
audience: Codex skill maintainers
last_reviewed: 2026-08-23
---

# Architect Review Capability Expansion

## Capability Contract

`$architect-review` reviews modular-monolith changes by default and uses distributed review only for real distributed boundaries. Every review reports architecture impact, affected quality attributes, evidence, corrective direction, residual risk, and verification gaps.

## Evidence Boundary

The skill does not recommend a concrete distributed platform or provider without repository evidence or `$deep-research`. It routes domain, security, performance, research, and ADR work to the specialist that owns that workflow.
