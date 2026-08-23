---
title: .NET Backend API And Application Delivery
intent: Help an agent decide how to evolve backend behavior from a requirement through a compatible API and verified application flow.
scope: Reusable API-boundary, application-delivery, error, cancellation, and test-boundary guidance. Excludes project-specific frameworks, module layouts, persistence infrastructure, and commands.
audience: Autonomous Codex implementation agents
last_reviewed: 2026-08-23
---

# .NET Backend API And Application Delivery

## When To Use This Knowledge

Read this when a backend task changes an externally observable API, input validation, application behavior, mapping, error behavior, cancellation, or the test boundary needed to prove a change.

## Implementation Procedure

1. Classify the change as contract, boundary validation, application invariant, mapping/state transition, or error/cancellation behavior.
2. Preserve additive contract evolution unless consumers and rollout explicitly accept a breaking change.
3. Validate untrusted transport shape at entry; put reusable business preconditions in application or domain behavior.
4. Decide omitted-update semantics before mapping: unchanged, cleared, or invalid.
5. Use unit tests for deterministic rules; use integration tests when HTTP, serialization, composition, authorization, persistence, or transactions determine correctness.

## Delivery Decisions

- Start with the required behavior and existing behavior before choosing an implementation pattern.
- Treat request and response changes as compatibility decisions. Prefer additive evolution; explicitly flag breaking changes and their consumers.
- Validate untrusted transport input at the boundary. Keep business invariants in the application or domain flow so they apply outside HTTP entry points too.
- Keep transport handling, application orchestration, domain behavior, and persistence adapters independently testable.
- Preserve cancellation from the entry point through all asynchronous work. Do not turn cancellation into a successful response or silently ignore it.
- Define error behavior intentionally: validation failures, missing resources, authorization failures, conflicts, and unexpected failures need distinguishable outcomes without leaking internals.

## Mapping And State Changes

- Map external contracts explicitly when the destination has ownership, lifecycle, nested graph, or normalization rules.
- Do not let a mapper infer relationship ownership, authorization state, or destructive update behavior.
- For updates, decide whether omitted data means unchanged, cleared, or invalid before coding; test that choice.

## Boundary Example

Derived from the validated endpoint and handler shape represented by `ApiEndpointExecutor` and `CreateResumeCommandHandler`.

**Good:** validate transport input at the boundary; keep a reusable application precondition behind a narrow contract.

```csharp
if (validationErrors.Count > 0)
{
    return ValidationProblem(validationErrors);
}

if (!await _relatedLookup.ExistsAsync(command.RelatedId, cancellationToken))
{
    throw new InvalidOperationException("Related record was not found.");
}
```

**Bad:** returning an HTTP result from an application handler makes the rule unavailable to non-HTTP callers.

```csharp
if (!await _repository.ExistsAsync(command.Id, cancellationToken))
{
    return Results.BadRequest();
}
```

## Verification Boundary

- Use focused unit tests for deterministic application rules, mapping, and edge cases.
- Use integration tests when correctness depends on HTTP behavior, serialization, dependency composition, authorization wiring, persistence, or transaction behavior.
- Cover changed success, validation, error, and permission paths. Add compatibility coverage when existing consumers could be affected.

## Boundaries

- Do not embed project-specific endpoint helpers, framework choices, repository abstractions, or command paths in this knowledge. Retrieve the relevant project adaptation topic through `KnowledgeBase/INDEX.md`.
- Do not use a transport validator as the only enforcement point for a rule that also applies to background jobs, message handlers, or internal callers.

## Discover Locally Only When

Inspect the changed public contract, consumers, existing error mapping, and changed handler behavior. Do not rediscover the stable validation-versus-invariant split or test-boundary selection process.
