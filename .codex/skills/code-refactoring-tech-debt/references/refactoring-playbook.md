# Refactoring Playbook

Use this playbook when behavior must stay stable while structure improves.

## Sequence

1. define the debt clearly
2. establish a verification baseline
3. isolate a narrow refactor boundary
4. refactor in reviewable steps
5. stop once the next change becomes easier

## Common debt themes

- mixed responsibilities
- duplicated branching logic
- query sprawl
- hidden coupling across layers
- tests that only prove implementation details