---
name: project-knowledge-builder
description: Build reusable, repo-specific knowledge for ResumeEnhancer by investigating code, product documents, and tests, then saving structured guidance another Codex agent can apply later. Use when Codex needs to understand architecture, feature flows, conventions, responsibilities, or requirement-to-implementation traceability before coding, reviewing, debugging, onboarding, or planning changes in this project.
---

# Project Knowledge Builder

## Overview

Use this skill to turn ResumeEnhancer investigation into durable knowledge artifacts that future Codex sessions can reuse. Focus on evidence gathering, architecture comprehension, implementation patterns, and practical guidance rather than code changes.

Read `references/repository-topography.md` when the topic spans multiple areas or you need a quick map of where to gather evidence.

## Working Rules

- Read file contents before drawing conclusions.
- Distinguish clearly between observed facts, inferences, and recommendations.
- Do not modify application code, tests, package references, migrations, or runtime configuration while using this skill.
- Save knowledge artifacts under `KnowledgeBase/` unless the user explicitly chooses another location.
- Write the final knowledge in terms of symbols, responsibilities, workflows, and constraints rather than file-tour narration.
- If evidence is incomplete, say so instead of filling gaps from generic framework knowledge.

## Best-Fit Requests

Use this skill for requests such as:

- "Build project knowledge for the resume builder flow."
- "Document how backend validation works in this repo."
- "Explain how a <ModuleName> feature moves from user story to implementation."
- "Create onboarding knowledge for new agents working on persistence."
- "Capture the conventions for adding a new Minimal API endpoint."

Do not use this skill when the user only wants a quick one-off answer and does not need a saved knowledge artifact.

## Workflow

### 1. Check for existing knowledge

Look in `KnowledgeBase/` for related artifacts before creating a new one.

If relevant knowledge exists:

- summarize what overlaps
- decide whether to extend it or create a new artifact

If nothing relevant exists, proceed from scratch.

### 2. Frame the topic

Read enough repository evidence to identify the distinct topic clusters behind the user's request.

Examples in this repository include:

- module composition and dependency direction
- Minimal API endpoint patterns
- FluentValidation request rules
- service-layer command and query handling
- Mapster mapping conventions
- persistence and unit-of-work behavior
- frontend resume-builder state and forms
- testing and integration-host setup
- requirement-to-implementation traceability

If the request is broad, present the clusters and narrow the scope before deep investigation.

### 3. Clarify output expectations

Determine:

- intended audience
- desired depth
- whether the artifact should optimize for onboarding, implementation, review, or planning
- whether code examples, edge cases, and verification steps are expected

Base these questions on what you already found in the codebase.

### 4. Investigate deeply

Perform focused reconnaissance guided by the chosen topic.

Collect evidence for:

- core symbols and contracts
- request and data flow
- responsibilities across layers
- invariants and validation rules
- extension points
- test coverage and verification patterns
- links between product documents and implementation when relevant

Use subagents only for evidence gathering when the topic spans many areas. Keep prompts neutral and task-focused.

### 5. Draft the structure

When the topic is broad or the output will be reused heavily, create a structural draft first:

- `KnowledgeBase/<topic-name>.draft.md`

Use sections that fit the topic. Common sections include:

- Title
- Intent
- When to use this knowledge
- Core concepts
- Architectural placement
- Main workflows
- Key symbols and responsibilities
- Rules and invariants
- Extension pattern
- Verification and testing
- Pitfalls and boundaries

### 6. Write the final knowledge

Save the completed artifact as:

- `KnowledgeBase/<topic-name>.knowledge.md`

Use this frontmatter:

```yaml
---
title: <short title>
intent: <what this knowledge helps with>
scope: <topic boundaries>
audience: <intended agent or contributor>
last_reviewed: <YYYY-MM-DD>
status: draft
---
```

The body should:

- explain the topic using repository concepts and symbols
- connect business intent to implementation where that connection matters
- capture reusable patterns
- name important boundaries and anti-patterns
- include practical examples or checklists when helpful
- include verification guidance

## Evidence Labels

Use these labels where they improve clarity:

- `Observed` for statements directly supported by repository contents
- `Inferred` for conclusions drawn from multiple pieces of evidence
- `Recommended` for practices that fit the current architecture but are not yet repository facts

## ResumeEnhancer-Specific Guidance

Prefer these evidence sources in order:

1. `README.md`
2. `Business-Requirements/`
3. `User-Stories/`
4. `application/`
5. `test/`

When tracing implementation across layers, expect to move through patterns like:

- frontend page or hook
- API client call
- Minimal API endpoint
- request validator
- service-layer contract and handler
- persistence abstraction and repository
- mapping and response shaping
- unit or integration tests

## Final Checks

Before finishing:

- confirm the artifact is grounded in repository evidence
- confirm it is specific to ResumeEnhancer rather than generic .NET or React advice
- confirm another agent could use it without this conversation
- confirm saved locations and file names are explicit