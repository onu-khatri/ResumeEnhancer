---
name: clean-code
description: Keep ResumeEnhancer changes readable, cohesive, and easy to extend without introducing unnecessary abstractions or duplication. Use when Codex is implementing, refactoring, or reviewing code quality across backend or frontend areas.
---

# Clean Code

Use this skill to improve clarity and maintainability while preserving the repository's real architecture and delivery pace.

## Use this skill when

- writing new code that should stay easy to understand and modify
- refactoring code that works but is painful to change
- reviewing whether structure, naming, or abstraction quality is slipping

## Do not use this skill when

- the main concern is security, architecture, or deployment strategy instead
- an abstraction would make simple code harder to follow

## Principles

- name things by responsibility, not implementation accident
- keep units of code focused
- prefer explicit control flow over cleverness
- reduce duplication only when the resulting shape is clearer
- let the repository structure explain intent where possible

## Review lenses

- intention-revealing names
- function and component size
- side effects and hidden coupling
- comments versus expressive code
- complexity added for speculative reuse

## Concrete heuristics

Naming:
- Use intention-revealing names (`elapsedTimeInDays`, not `d`).
- Avoid disinformation and meaningless distinctions (`ProductData` vs `ProductInfo`).
- Class names are nouns; method names are verbs.

Functions and components:
- Keep them small and doing one thing at one level of abstraction.
- Prefer 0-2 arguments; 3+ needs strong justification.
- Avoid hidden side effects and long parameter/flag lists.

Error handling:
- Prefer exceptions over return-code plumbing.
- Do not return or pass `null` where a caller must handle it.
- Write the try/catch boundary first to define the operation scope.

Comments:
- Prefer expressive code over comments; rewrite unclear code instead of explaining it.
- Keep only informative comments (why, not what) and remove redundant ones.

## Implementation checklist

- [ ] Does each function/component do exactly one thing?
- [ ] Are names searchable and intention-revealing?
- [ ] Is the code clear without relying on comments?
- [ ] Are there 3+ arguments, flags, or nullable plumbing that can be simplified?
- [ ] Does the change add duplication or speculative abstraction?

## ResumeEnhancer focus

- handlers that do one business job well
- validators that are declarative and narrow
- API clients and hooks that do not mix UI rendering concerns
- tests whose names explain behavior, not implementation trivia

## Output requirements

- specific readability or cohesion issues
- refactor suggestion with rationale
- risks of over-abstraction when relevant