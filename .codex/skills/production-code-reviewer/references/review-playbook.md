# ResumeEnhancer Review Playbook

Use this repository-specific review sequence:

1. Read the relevant story and `AGENTS.md`.
2. Inspect the real diff and enough surrounding code to understand the call path.
3. Check boundary ownership across Web, AM, SL, PL, DM, frontend features, and tests.
4. Validate contract consistency across request models, validators, handlers, repositories, and UI models.
5. Report only actionable findings with severity and impact.