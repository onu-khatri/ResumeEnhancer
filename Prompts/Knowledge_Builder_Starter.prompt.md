Use `$project-knowledge-builder` to investigate ResumeEnhancer and build a reusable knowledge artifact about one concrete topic through the approval workflow.

Topic:
- Trace the resume create/update flow end to end.

Expected outcome:
- Follow the skill's gates A–H, including the second user interview (Gate D) after composing.
- First ask interview questions needed to shape the knowledge.
- Then create `KnowledgeBase/<topic-name>.kb_plan.md` with sections, placeholders, and scope.
- Wait for approval before creating `KnowledgeBase/<topic-name>.pre-knowledge.md`.
- Wait for approval again before saving the final `KnowledgeBase/<topic-name>.knowledge.md`.
- Explain the flow from business intent to frontend form state, API client, Minimal API endpoint, validation, service-layer handler, persistence adapter, and tests.
- Separate observed facts from inferred conclusions.
- Call out invariants, extension points, and likely pitfalls for future implementers.

Collaboration guidance:
- If the topic spans many files, delegate broad evidence gathering to the `knowledge-researcher` agent from `.codex/agents/knowledge-researcher.toml`.
- After gathering evidence, synthesize the final knowledge yourself instead of pasting raw notes.

Verification:
- Confirm the interview questions asked.
- Confirm the plan path.
- Confirm the pre-knowledge path.
- Confirm the final saved artifact path only after explicit approval.
- State what was verified directly from repository contents.
