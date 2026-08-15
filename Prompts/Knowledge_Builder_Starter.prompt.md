Use `$project-knowledge-builder` to investigate ResumeEnhancer and create a reusable knowledge artifact about one concrete topic.

Topic:
- Trace the resume create/update flow end to end.

Expected outcome:
- Produce a saved knowledge artifact under `Prompts/KnowledgeBase/`.
- Explain the flow from business intent to frontend form state, API client, Minimal API endpoint, validation, service-layer handler, persistence adapter, and tests.
- Separate observed facts from inferred conclusions.
- Call out invariants, extension points, and likely pitfalls for future implementers.

Collaboration guidance:
- If the topic spans many files, delegate broad evidence gathering to the `knowledge-researcher` agent from `.codex/agents/knowledge-researcher.toml`.
- After gathering evidence, synthesize the final knowledge yourself instead of pasting raw notes.

Verification:
- Confirm the saved artifact path.
- State what was verified directly from repository contents.
