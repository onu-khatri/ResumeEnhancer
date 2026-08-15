# Knowledge Artifact Template

Use this template as the starting shape for every `*.pre-knowledge.md` and final `*.knowledge.md` artifact. Keep the structure lightweight by default. Start with the core sections below, then add only the extra sections that the user explicitly approved after you discovered they would improve the artifact.

## Frontmatter (required)

```yaml
---
title: <short title>
intent: <what this knowledge helps an agent DO, not what the topic IS>
scope: <what is in and out; name the topic boundary>
audience: <implementer | reviewer | planner | onboarder | combination>
last_reviewed: <YYYY-MM-DD>
status: draft | reviewed | stable
source_plan: KnowledgeBase/<topic-name>.kb_plan.md
validation:
  gates:
    A_grounding: PASS | FAIL
    B_specificity: PASS | FAIL
    C_reproducibility: PASS | FAIL
    D_user_interview: PASS | FAIL
    E_consistency: PASS | FAIL
    F_boundary_discipline: PASS | FAIL
    G_currency: PASS | FAIL
    H_validation_record: PASS | FAIL
  cross_examination: clean | acceptable-with-gaps | not-ready
  assumptions: []
  known_gaps: []
  verified_this_session:
    - <what was re-checked against current code>
---
```

Keep the frontmatter short. Only expand `assumptions` or `known_gaps` when they are real.

## Core sections (start here)

### Intent

One to three sentences on what an agent can accomplish after reading this. Action-oriented.

### When to use this knowledge

The trigger conditions: the agent situation that makes this artifact the right one to read.

### Core concepts

Repo-specific terms, symbols, and responsibilities, defined at first use.
Expectation: include short code snippets for the key abstractions so an agent can understand the role of the concept without reopening the repository immediately.

### Main workflows

Numbered, concrete flows from entry point to outcome. Each step names a real symbol, route, or file.
Expectation: each workflow should embed enough code to make the execution path understandable from the artifact alone.

### Rules and invariants

Non-negotiable constraints. State each as a rule with the architectural reason it exists.
Expectation: include the validating or enforcing code snippet when possible, not only prose.

### Extension pattern

The safe, ordered sequence for adding a new instance of whatever the topic describes. Point at an existing example to copy.
Expectation: show the copy shape with real code snippets, not only "go inspect example X".

### Verification and testing

Runnable commands and the specific test projects/files that prove the behavior. State what was actually run this session.
Expectation: include short assertion snippets from the most relevant tests so the reader can see what behavior is being proven.

### Pitfalls and boundaries

Anti-patterns, "do not" rules, and the limits of the topic. Concrete failure modes, not hedges.
Expectation: include snippets that show the safe registration or extension point so the boundary is visible, not only described.

### Clarifications (from Gate D user interview)

A record of every user interview question and its answer, plus any confirmed assumption. This section proves the agent did not assume; it is the written trace of Gate D.

## Optional extra sections (add only after user approval)

- `Architectural placement` — use when the topic spans layers or composition roots.
- `Key symbols and responsibilities` — use when a compact type/function inventory would materially help.
- `Type references` — use when the user wants named interface/type links without a large file inventory.
- `Evidence map` — use when the user explicitly wants a claim-to-source appendix.
- `Additional discovered section` — you may propose a new section name when the topic clearly needs it. Ask the user for approval before adding it to the artifact.

## Evidence map (optional)

Keep a private scratch list while investigating; include a public evidence map only when the user wants it or when the artifact needs it for clarity. If the user prefers snippet-first evidence, the artifact can rely on embedded snippets plus a compact reference section instead.

```markdown
## Evidence map
| Claim | Source (snippet or file) | Label |
|-------|--------------------|-------|
| ...   | ...                | Observed / Inferred / Recommended |
```

## Validation block (required in frontmatter)

Complete after running the sequential gates (`knowledge-quality-gates.md`), the checklist (`artifact-validator-checklist.md`), and the artifact cross-examination (`artifact-cross-examination.md`). The gates run in order A→H; Gate D is the user interview and must pass before E–H are attempted. A `status` of `stable` is only valid if you re-ran the validator against current code this session. Keep `status: draft` for unapproved `.pre-knowledge.md` artifacts.
