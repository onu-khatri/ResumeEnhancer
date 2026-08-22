---
name: user-story-creator
description: Create or revise evidence-grounded, implementation-ready ResumeEnhancer user-story packs from business requirements, product decisions, research, and repository context. Use for feature, defect, technical-debt, or research stories in User-Stories/*.US.md, *.SI.md, and *.Research.md; use us-kickoff after approval and readiness.
---

# User Story Creator

Create small, traceable story packs that let engineering start work without rediscovering product intent. A pack is a delivery artifact, not a generic product brief: it states the user outcome, scope, observable acceptance criteria, dependencies, constraints, and implementation context needed for one coherent slice.

## Boundaries

- Create or revise story artifacts under `User-Stories/`; do not implement application code, open branches, or start delivery work.
- Always create and present an authoring plan before editing a business requirement, story, supporting-information, or research artifact. Wait for explicit user approval before making any such edit.
- After approval, implement only the approved plan. If evidence changes the affected BR, scope, slices, dependencies, or acceptance criteria materially, present a revised plan and wait for approval again.
- Treat `Business-Requirements/`, approved product decisions, existing stories, and the current repository as evidence. Do not turn a plausible design choice into a stated requirement.
- Do not fabricate research findings, user quotes, baselines, targets, or competitor claims. When evidence is missing, record the gap and create a bounded research/spike slice only when resolving it is necessary to plan implementation.
- Use `openspec-propose` for a new OpenSpec change, `openspec-update-change` for a planned change, and `us-kickoff` only after an approved story is ready to implement.
- Use `project-knowledge-builder` when the requested outcome is durable repository knowledge rather than a delivery story.

## Story Pack

Create these files together for each implementation slice, using one shared `<epic>.<n> <slug>` stem:

- `User-Stories/<stem>.US.md` - the authoritative user story, scope, acceptance criteria, rules, readiness, and definition of done.
- `User-Stories/<stem>.SI.md` - supporting implementation context: affected components, workflows, states, interfaces, edge cases, and focused test coverage.
- `User-Stories/<stem>.Research.md` - relevant repository or external evidence, source classification, open questions, and conclusions that informed the slice.

Keep a story, its supporting information, and its research mutually consistent. If a slice does not need external research, record the local evidence used and explicitly say that no external research was needed; do not manufacture competitor or market claims.

## Workflow

### 1. Establish the authoring scope and BR alignment

Inspect `AGENTS.md`, the relevant `Business-Requirements/` documents, neighboring `User-Stories/` packs, and the affected code or tests when the request concerns existing behavior. Determine whether the request creates a new slice or revises an existing one.

Before writing, establish from evidence:

- target persona and outcome
- business requirement or approved decision being served
- delivery shape: frontend, backend, full-stack, architecture, or research
- affected product/module boundaries and likely dependencies
- what the user can observe to verify success

Record each important input as `Observed`, `Inferred`, or `Recommended`, with its source and delivery implication. Separate facts from interpretation: an observation is not automatically a user need, and an insight is not automatically an approved requirement. Read [evidence-and-discovery.md](references/evidence-and-discovery.md) when source material must be synthesized, user research is needed, or success measures are unclear.

Search the relevant `Business-Requirements/*.BR.md` files before declaring a requested requirement missing. If an approved requested requirement is absent, identify the single most specific authoritative BR that should own it and propose the exact requirement, boundary, traceability, and any affected story slices. Read [business-requirement-alignment.md](references/business-requirement-alignment.md) when determining BR ownership or reconciling a gap.

Use `$user-interview` to ask focused questions for material product decisions that evidence cannot resolve. State unknowns as open questions instead of silently selecting behavior, entitlement rules, data retention, success metrics, or technical architecture.

### 2. Present the authoring plan and wait

Before changing files, present a concise plan containing:

- the decision, evidence, and open questions
- BR assessment: existing source(s), or the proposed BR file and requirement change needed to close a gap
- affected story-pack stems, story types, dependencies, and delivery shape
- intended acceptance-criteria and verification focus
- artifacts to create or revise

Wait for explicit user approval. A request to create a story or BR does not itself authorize the edits; approval of this plan does. Do not create draft files before approval unless the user explicitly asks for a draft artifact.

### 3. Reconcile the approved BR change

When the approved plan includes a missing business requirement, update the identified authoritative `.BR.md` first. Preserve the document's established structure and terminology, add the approved requirement with its business intent and boundary, and update related traceability or open-decision sections when they exist. Do not use a user story to silently introduce a business requirement that the BR does not contain.

### 4. Slice the approved work before drafting

Make each story independently understandable, testable, and releasable where practical. Split when distinct user outcomes, dependency order, approval boundaries, or high-conflict surfaces would otherwise be conflated.

For a full-stack capability, choose the slice order from its real dependency graph. Keep shared API contracts, schema migrations, seed data, and shared UI primitives in an explicit coordinating slice rather than assigning overlapping ownership. Do not assume every epic needs both frontend and backend work.

Give every slice a stable, unique story ID and a filename consistent with nearby story packs. Preserve existing IDs and traceability when revising an existing story.

Read [story-shaping.md](references/story-shaping.md) when selecting a story type, decomposing a broad initiative, defining a research/spike slice, or choosing an acceptance-criteria form.

### 5. Write the approved user story

Use the established frontmatter fields used by `us-kickoff`:

```yaml
id: <stable-story-id>
title: <clear outcome-oriented title>
status:
branch:
worktree_path:
base_branch: <current repository default branch>
pr_url:
is_architectural: <true when boundaries or contracts change>
approach_summary: <one-sentence delivery intent>
created: <YYYY-MM-DD>
updated:
```

Write the `.US.md` with the headings that materially apply:

```markdown
## [<id>] - <title>

**Module:** <product capability>
**Layer:** <Frontend | Backend | Full Stack | Architecture | Research>
**Priority:** <business priority or explicitly unknown>
**Persona:** <actor>
**Dependency:** <None | story IDs or named prerequisites>
**Source Classification:** <Observed | Inferred | Recommended>

### User Story
As a <persona>, I want <capability>, so that <measurable or observable outcome>.

### Description
### Scope
### Preconditions
### Detailed Workflow
### Functional Requirements
### Acceptance Criteria
### Business Rules
### Validation Rules
### Error Handling
### Security Considerations
### Analytics / Tracking
### API Dependencies
### Data Requirements
### UX Notes
### Definition Of Ready For Engineering
### Definition of Done
### Related Documents
```

Omit headings that have no meaningful content only when that omission does not obscure a decision. Keep requirements declarative and acceptance criteria observable. Separate business rules from UI or technical implementation suggestions.

### 6. Add delivery context and evidence

In `.SI.md`, document only the implementation context that another engineer needs: integration points, state/error matrix, component or API surfaces, validation behavior, edge cases, test focus, and any migration, authorization, accessibility, responsiveness, performance, or observability concerns that apply.

In `.Research.md`, distinguish:

- `Observed` - directly supported by a repository artifact or an authoritative external source.
- `Inferred` - a conclusion drawn from cited evidence.
- `Recommended` - a proposed decision awaiting approval.

Use repository evidence first. Research externally only when required to resolve a real product, domain, standard, or current-market question. Cite sources precisely enough that a reviewer can verify the claim, record the research date for time-sensitive evidence, and preserve unresolved questions. For user-research evidence, retain the method, participant or data segment, collection date, observation, and interpretation separately.

### 7. Run the readiness review

Before presenting the pack, verify that:

- the three files share the same title, slice, dependencies, and source conclusions
- the story serves a named business requirement or explicitly records the missing product decision
- every newly introduced approved business requirement is present in its authoritative `.BR.md` and linked from affected story packs
- scope and out-of-scope boundaries are clear
- acceptance criteria cover successful behavior, important failure or empty states, and relevant authorization or privacy behavior
- preconditions, APIs, data, and dependencies do not contradict one another
- the problem, evidence, proposed outcome, requirements, and validation remain traceable without treating an inference as fact
- the story passes INVEST proportionately: it has independent value where possible, leaves nonessential implementation negotiable, states value, has enough detail to estimate, is bounded enough to deliver, and is objectively testable
- Definition Of Ready names any decision, contract, design, or dependency that must be resolved before implementation
- Definition of Done includes demonstrable behavior and proportionate frontend, backend, contract, migration, and test expectations
- the proposed slice respects module boundaries from `AGENTS.md` and does not prescribe unsupported implementation details

If material decisions remain unresolved, leave the story in a draft/planning state and report the exact questions. Do not mark it `Ready_To_Implement` by implication.

## Quality Bar

- Prefer one user outcome per story over broad feature narratives.
- Make dependencies directional and actionable; do not use vague labels such as "backend support needed."
- Reference existing artifacts by their real names and paths.
- Preserve product intent without copying competitor behavior as a requirement.
- Describe only the acceptance criteria that can be verified; identify measurements or instrumentation that still need definition.
- Keep implementation choices in supporting information unless they are required business or architectural constraints.
- When revising a pack, update all affected `.US.md`, `.SI.md`, and `.Research.md` artifacts together and retain a traceable explanation of the changed decision.

## Reference

- [evidence-and-discovery.md](references/evidence-and-discovery.md) - use for evidence classification, synthesis, research gaps, and success measures.
- [business-requirement-alignment.md](references/business-requirement-alignment.md) - use for BR ownership, gap detection, and traceability.
- [story-shaping.md](references/story-shaping.md) - use for story types, vertical slicing, acceptance criteria, and INVEST review.

## Handoff

Present the completed, approved-scope story pack with its BR traceability, slices, dependencies, open questions, and readiness status. Obtain any required product or architecture approval before marking a story ready. Once the story pack is approved and its Definition Of Ready is met, hand it to `$us-kickoff` for delivery planning.
