# How To Use Codex Agents

This repository includes custom Codex agents under `.codex/agents`.

Use this guide when you want Codex to take on a more specialized role instead of relying only on the default general-purpose behavior.

## Where agents live

- Custom agents: `.codex/agents/*.toml`
- Skills: `.codex/skills/*`
- Global repo guidance: `AGENTS.md`

## What agents are for

Agents are best for:

- taking on a specialist role
- following a focused delivery style
- applying a consistent review or implementation mindset
- coordinating work for a specific kind of task

Examples:

- backend implementation
- frontend implementation
- code review
- security review
- story orchestration
- knowledge gathering

## What skills are for

Skills are different from agents.

Use skills for:

- reusable workflows
- repository-specific practices
- step-by-step playbooks
- reference material and templates

In this repo:

- agents live in `.codex/agents`
- skills live in `.codex/skills`

Do not move skills into `.agents` or `.codex/agents` if you want Codex to keep treating them as skills.

## How to invoke an agent

The safest way is to explicitly name the agent in your prompt and describe the task clearly.

Use patterns like:

```text
Use the `backend-implementer` agent to add a new endpoint for resume duplication.
```

```text
Use the `code-reviewer` agent to review the current diff for bugs, regressions, and missing tests.
```

```text
Use the `story-orchestrator` agent to plan implementation for the approved user stories.
```

## Recommended prompt format

Use this structure:

```text
Use the `<agent-name>` agent to <task>.

Context:
- <important repo area>
- <story or requirement>
- <constraints>

Expected output:
- <what you want back>
```

Example:

```text
Use the `backend-implementer` agent to add resume template filtering to the search endpoint.

Context:
- backend change only
- keep existing module layering
- update tests if behavior changes

Expected output:
- code changes
- impacted layers
- verification results
```

## Agents currently available in this repo

### `backend-implementer`

Use for backend feature work across:

- Minimal APIs
- FluentValidation
- Mediator handlers
- mapping
- repositories
- backend tests

Example:

```text
Use the `backend-implementer` agent to add bulk archive support for resumes.
```

### `frontend-implementer`

Use for frontend feature work across:

- routes
- feature components
- hooks
- forms
- API integration
- frontend states

Example:

```text
Use the `frontend-implementer` agent to build a resume archive management page.
```

### `code-reviewer`

Use for review tasks with a bug-first mindset.

Best for:

- pull request review
- diff review
- regression checks
- missing test detection

Example:

```text
Use the `code-reviewer` agent to review the current branch for correctness, risks, and missing tests.
```

### `knowledge-researcher`

Use for repository knowledge-building and deep investigation.

Best for:

- architecture tracing
- project knowledge artifacts
- codebase research
- requirement-to-code mapping

Example:

```text
Use the `knowledge-researcher` agent to document how resume search flows from frontend to persistence.
```

For approval-driven knowledge work, pair it with the knowledge skill:

```text
Use the `knowledge-researcher` agent and `$project-knowledge-builder` to investigate persistence patterns.
Ask interview questions first, then create `KnowledgeBase/persistence-patterns.kb_plan.md`.
After I approve the plan, create `KnowledgeBase/persistence-patterns.pre-knowledge.md`.
Only save the final `.knowledge.md` after I approve the draft.
```

### `security-auditor`

Use for secure coding and security review tasks.

Best for:

- endpoint security review
- access-control review
- data exposure review
- abuse-path analysis

Example:

```text
Use the `security-auditor` agent to review delete and search flows for authorization gaps.
```

### `story-orchestrator`

Use for multi-story planning and kickoff.

Best for:

- dependency ordering
- parallel-group planning
- branch/worktree planning
- implementation kickoff

Example:

```text
Use the `story-orchestrator` agent to plan kickoff for the approved stories in User-Stories/.
```

## Agent usage tips

- Always say which agent to use.
- Always describe the exact task.
- Add constraints if the task must stay backend-only, frontend-only, or review-only.
- Mention the user story or requirement if one exists.
- Ask for the kind of output you want: code, plan, review findings, or knowledge artifact.

## Good prompt examples

```text
Use the `backend-implementer` agent to add updated-date filtering to resume search and update the relevant tests.
```

```text
Use the `frontend-implementer` agent to improve the resume dashboard empty and error states without changing the backend.
```

```text
Use the `code-reviewer` agent to review the staged changes and report only actionable findings.
```

```text
Use the `knowledge-researcher` agent to create a knowledge artifact for the shared persistence layer under KnowledgeBase/.
```

```text
Use the `story-orchestrator` agent to classify these stories into backend, frontend, and full-stack work and propose branch names.
```

## When not to use a custom agent

You do not need a custom agent when:

- the task is tiny and direct
- the default Codex behavior is enough
- you only need a quick factual answer

## Related files

- `AGENTS.md`
- `.codex/agents/`
- `.codex/skills/`
- `Prompts/`
