# ResumeEnhancer Agent Instructions

These instructions apply to the repository unless a more specific nested
`AGENTS.md` overrides them.

## Purpose and authority

ResumeEnhancer is a modular resume platform built as a .NET modular monolith
with a React/Vite client.

Use authorities in this order:

1. explicit user request and approved scope;
2. this file for repository-wide invariants and gates;
3. the selected skill and its routed references;
4. approved plans, requirements, stories, OpenSpec artifacts, and ADRs for the
   decision they govern;
5. current repository code, tests, Git/worktree, and hosted evidence.

Prefer current repository evidence over inference. Do not turn an inferred
convention into a repository rule without recording the decision in its proper
authority.

The goal is minimum sufficient context: load only the authority needed for the
current decision, preserve explicit ownership, and verify every claimed state.

## Repository architecture invariants

- `application/WebSolution/ModulesComposition` is the host-facing module
  composition boundary. Do not wire the host directly to module internals when
  an existing composition path exists.
- Keep HTTP concerns in `ResumeEnhancer.ResumeModule.Web`.
- Keep request/response contracts in `ResumeEnhancer.ResumeModule.AM`.
- Keep use-case orchestration, Mediator contracts, handlers, and mapping
  workflow logic in `ResumeEnhancer.ResumeModule.SL`.
- Keep domain entities and domain-only concepts in
  `ResumeEnhancer.ResumeModule.DM`.
- Keep EF configuration, repository adapters, and schema-specific persistence
  behavior in `ResumeEnhancer.ResumeModule.PL`.
- Keep shared infrastructure in `application/Infrastructure`.
- Keep frontend feature logic in
  `application/WebSolution/websolution.client/src/features`.

Inspect existing ownership and extension points before introducing a shared
abstraction, cross-module contract, persistence boundary, composition pattern,
or frontend primitive.

## Universal working rules

Before changing behavior, read the smallest relevant README, requirement,
story, approved plan, implementation files, and nearest tests. Follow existing
naming, dependency, registration, validation, error-handling, and testing
patterns. Keep changes scoped and trace business intent to a requirement,
story, OpenSpec artifact, or approved plan when available.

Do not ask for routine confirmation, permission to inspect repository evidence,
permission to run non-destructive commands, or clarification that repository
evidence or an approved plan resolves. Use the host structured-question UI for a
material interview when available; otherwise ask one concise question.

### Autonomous execution and user threshold

The parent agent owns the execution loop:

```text
request → minimum authority/evidence → classify → next safe action
       → execute or delegate → observe → verify → reconcile → continue
```

Continue automatically when the next action is inside approved scope,
reversible or low-risk, supported by evidence, and not blocked by required
approval, credentials, external authorization, or a material product decision.
Continue independent safe work when another lane is blocked.

Interrupt the user only when:

- a workflow gate explicitly requires approval;
- a material product, scope, security, data, UX, compatibility, or architecture
  decision has multiple evidence-supported outcomes;
- required information cannot be derived from repository or connected tools;
- credentials, secrets, external authorization, or an unavailable dependency
  must be supplied;
- the next action is destructive, irreversible, externally consequential, or
  outside approved scope; or
- authorities materially conflict without a safe precedence rule.

Before asking, state the exact decision, evidence checked, why it is material,
and the smallest required question. Do not interview for curiosity or routine
implementation preferences.

After every completed, failed, or blocked step choose the next safe action in
this order: continue the approved plan; verify the step; repair a local failure
within scope; retrieve narrow evidence; delegate an independent owned lane;
continue another unblocked lane; request a required decision; stop only when no
safe action remains.

## Knowledge retrieval

`KnowledgeBase/INDEX.md` is the sole default entry point.

1. Read the index.
2. Identify the affected decision area.
3. Read only applicable topics and retrieve an ADR only when its decision is
   touched.
4. Inspect volatile implementation facts directly in current code and tests.
5. If implementation reveals a new decision area, state it, route through the
   index, reconcile the authority, then continue.

Do not enumerate `KnowledgeBase/**`, read every knowledge file, recursively
follow references, reload the same authority without reason, or use knowledge
artifacts instead of current implementation evidence. A link is not a
transitive context-loading instruction.

Default focused-task budget: up to three knowledge topics and two ADRs. Exceed
it only for genuinely cross-cutting work and record the decision requiring each
additional authority.

## Skill and agent routing

Discover repository skills under `.codex/skills/<skill>/SKILL.md` and custom
agents under `.codex/agents/*.toml`. Read the selected skill before acting and
only its applicable references. Use this file for durable constraints, skills
for reusable procedures, agents for focused delegated work, the KnowledgeBase
index for durable project authority, and approved plans for change execution.

Select the smallest applicable set:

| Decision or work | Route |
|---|---|
| Backend feature/pattern/security/persistence | `$backend-feature-development`, `$backend-dotnet-patterns`, `$backend-security`, `$backend-ef-core` as triggered |
| Architecture/domain/ADR | `$architecture-review`, `$backend-dotnet-architecture`, `$architecture-domain-modeling`, `$architecture-adr` as triggered |
| Frontend feature or UI | `$frontend-development` plus `$frontend-guidelines`; use React, design, security, performance, or review skills only when triggered |
| Full-stack/story delivery | `$delivery-full-stack-feature`, story/issue kickoff, and applicable OpenSpec workflow |
| Development code, tests, configuration, migrations, or architecture implementation | `$workflow-development-entry` first; it routes specialists but does not invoke implementers |
| OpenSpec transition | `$openspec-workflow` plus `$openspec-repository-policy`; use `$openspec-orchestration` for approved worktree coordination |
| Plan created by `$workflow-planning` | `$plan-review-approval` |
| Production review/security/quality | `$quality-production-code-review`, `$quality-ai-code-review`, `$quality-code-review-checklist`, `$security-management`, or focused quality skill |
| Research or durable knowledge | `$research-deep`, `$knowledge-project-builder`, or documentation/product skill as triggered |
| Branch/worktree/commit/push/PR | `$git-worktrees`, `$git-workflows`, `$git-commit`, `$delivery-pull-request` as triggered |

Specialist skills return constraints, findings, evidence, or handoff; they do
not recursively invoke unrelated specialists or duplicate implementation
ownership. `$frontend-guidelines` is a non-delegating standards authority.

## Development and approval gates

Use `$workflow-development-entry` before development-related work. For
OpenSpec implementation, use the following lifecycle and do not collapse its
responsibilities:

```text
  → OpenSpec proposal in main checkout
  → explicit OpenSpec approval
  → canonical branch/worktree
  → proposal artifacts synchronized into worktree
  → development-entry readiness
  → implementation plan created in the branch worktree
  → explicit plan approval in that worktree
  → plan identity/approval/worktree validation
  → exactly one implementation owner
  → implementation and targeted verification
  → review, evidence reconciliation, and delivery
```

OpenSpec proposal artifacts and implementation plans are different:

- proposal artifacts define requirements, design, specifications, and task
  identities; they are authored in the main checkout and synchronized into the
  branch worktree;
- implementation plans define execution ownership, files, dependencies,
  validation, risks, and handoff; they are created and approved in the
  canonical branch worktree under
  `.tmp/ImplementationPlans/<change-name>/` and are not copied elsewhere.

The implementation planner may write only `Proposed` plans. Any agent editing
production code, tests, migrations, or configuration must read the matching
plan in the current worktree and verify `status: Approved`, approval metadata,
change/task identity, worktree identity when applicable, freshness, and scope.
Stop for missing, Proposed, stale, mismatched, or inconsistent plans. Do not
repair approval metadata implicitly.

For OpenSpec implementation, the parent/coordinator may inspect, plan,
reconcile, and review, but must not directly edit production code, tests,
migrations, or runtime configuration. Route one approved plan to exactly one primary implementation owner; keep review lanes read-only.

OpenSpec workflows under `.agents/skills/` are generator-managed. Do not edit them for repository-specific behavior; run `openspec update` when a generated refresh is required. Keep ResumeEnhancer policy in this file, repository-owned `.codex/skills/`, and applicable KnowledgeBase authorities.

## Delegation and parallel work

The parent resolves common context once and gives each delegated agent a focused manifest containing task/change identity, objective, owned and excluded paths, decision areas, required and conditional authorities, approved plan/task
references, shared-file ownership, and branch/worktree identity.

The delegated agent validates the manifest, stays within write scope, inspects the assigned surface and nearest tests, expands context only for a newly discovered decision, reports that expansion, and stops on ownership/approval inconsistency. It must not rediscover all knowledge, silently expand scope, or edit shared contracts, migrations, composition, generated artifacts, or shared UI primitives owned by another lane.

Before parallelizing, identify shared contracts, migrations, composition,
shared UI primitives, generated artifacts, dependencies, and ownership. Keep each write set disjoint and one coordinating lane for shared boundaries. Use agents only when delegation materially improves the task.

The parent/sub-agent lifecycle, event schema, parent acknowledgement, checkpoint and lost-agent fallback, and tracing capability rules are canonical here:

`.codex/skills/orchestration-agent-improvement/references/delegation-protocol.md`

Every delegated lane must use that protocol and report observable evidence, not merely “working.” The parent assigns a stable `parent_step_id` to every delegated lane, reconciles each event, updates the user on meaningful state changes, and continues automatically when `needs_user: false`. Native event, heartbeat, and OpenTelemetry support are capability-gated; use the protocol's text/status and bounded-polling fallback when unavailable.

## Verification and evidence

For code changes, run the smallest meaningful tests, broaden only when the affected boundary requires it, and state exactly what passed, failed, skipped, or was not run. Never claim success from planned commands.

Keep these states distinct:

```text
local change ≠ local verification ≠ commit ≠ push ≠ hosted PR
             ≠ CI success ≠ review approval ≠ merge ≠ final delivery
```

Update task, OpenSpec, issue, and delivery status only from corresponding
implementation or verification evidence. Do not infer CI from local tests,
push from a commit, review from a PR, or merge from CI.

For documentation, prompt, skill, agent, or instruction changes, verify:

- document structure and referenced paths;
- cross-references and authority precedence;
- contradictory or duplicated guidance;
- accidental recursive delegation or broad KnowledgeBase loading;
- generated-vs-repository-owned assumptions; and
- the resulting scoped diff.

Do not claim a skill or custom agent is auto-discovered in the current session
unless that behavior was actually observed in a fresh Codex task.

When changing `AGENTS.md`, `.codex/`, or `Prompts`, run:

```powershell
Get-ChildItem -Recurse .codex, Prompts
git diff -- AGENTS.md .codex Prompts
```

When changing knowledge routing, inspect `KnowledgeBase/INDEX.md` and confirm
that routing still goes through the index.

## Commands

Use these repository commands when their scope applies:

| Purpose | Command |
|---|---|
| Full solution build | `dotnet build application\ResumeEnhancerApp.slnx` |
| Unit tests | `dotnet test test\ResumeEnhancer.Tests\ResumeEnhancer.Tests.Unit.csproj --no-restore` |
| Integration tests | `dotnet test test\IntegrationTest\ResumeEnhancer.Tests.Integration.csproj --no-restore` |
| API host | `dotnet run --project application\WebSolution\WebSolution.Server\ResumeEnhancer.WebSolution.Server.csproj` |
| Migration help | `dotnet run --project application\Infrastructure\Migration\ResumeEnhancer.Infrastructure.Migration.csproj -- --help` |

## Durable knowledge

Save durable project knowledge under `KnowledgeBase/` unless explicitly asked
otherwise. Add a clear decision scope and update `KnowledgeBase/INDEX.md` when
agents are expected to retrieve it. Do not create knowledge for volatile
implementation detail, ordinary documentation, an existing authority's
decision, or merely to avoid reading current code.

## Core operating principle

Use the smallest sufficient authority set:

```text
request/story/OpenSpec
  → this file + KnowledgeBase/INDEX.md
  → minimum applicable skill/plan/requirement/ADR
  → current implementation, nearest tests, Git/worktree evidence
  → approved gate and one owner when implementation applies
  → implementation → review → verification → evidence reconciliation
```

The objective is not maximum context. It is the correct authority, current
evidence, explicit ownership, approved scope, focused context, and observable
verification. Preserve safety and determinism while routing specialized detail
to the authority that owns it.
