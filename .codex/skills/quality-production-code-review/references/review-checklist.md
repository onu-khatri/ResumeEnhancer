# ResumeEnhancer Review Checklist

Use this checklist when a review should be systematic rather than intuitive.

## Context

- confirm the user story or requirement being implemented
- confirm the change scope and whether unrelated files are mixed in
- confirm the changed layers: frontend, Web, AM, SL, PL, DM, tests

## Correctness

- does the change actually satisfy the acceptance criteria
- are edge cases and null or empty states handled
- are validators, handlers, and mappers consistent with each other
- are response shapes and UI expectations still aligned

## Security

- are ownership and authorization checks present where needed
- does the change expose secrets or sensitive user data
- are user inputs validated and safely rendered or persisted
- does logging avoid leaking protected information

## Performance

- are repository queries shaped appropriately
- is there unnecessary duplicate fetching or heavy client re-rendering
- are paging, filtering, or caching implications understood

## Maintainability

- are responsibilities in the right layer
- are names and abstractions clear enough for future changes
- does the diff add avoidable coupling or duplication

## Tests

- are tests present at the right boundary
- do they prove the changed behavior rather than implementation trivia
- are there obvious gaps in error, permission, or integration coverage