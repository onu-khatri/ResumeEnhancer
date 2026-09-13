# Feature Delivery Playbook

Use this note when a feature spans frontend, backend, persistence, and release preparation.

## Delivery sequence

1. confirm the story and acceptance criteria
2. identify cross-layer contract risks
3. plan frontend, backend, and test changes together
4. call out rollout, migration, and verification needs early

## Common risks

- API drift between layers
- shared-file conflicts during parallel work
- persistence changes without rollback notes
- incomplete test coverage at integration boundaries