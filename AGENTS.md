# ResumeEnhancer Agent Instructions

These instructions apply to the entire repository unless a more specific `AGENTS.md` overrides them in a nested subtree.

## Purpose

ResumeEnhancer is a modular resume platform implemented as a .NET modular monolith with a React/Vite client.

Agents working in this repository must:

* preserve established architectural boundaries
* prefer repository evidence over assumption
* keep cross-layer changes traceable to requirements and approved plans
* load only the minimum context needed for the current decision
* avoid broad repository or knowledge-base scans when a narrower authority exists
* keep implementation, review, planning, and research responsibilities distinct

---

## Primary Architecture Rules

* Treat `application/WebSolution/ModulesComposition` as the host-facing module composition boundary.
* Do not bypass module boundaries by wiring the host directly to module internals when an existing composition path already exists.
* Keep HTTP concerns in `ResumeEnhancer.ResumeModule.Web`.
* Keep request/response contracts in `ResumeEnhancer.ResumeModule.AM`.
* Keep use-case orchestration, Mediator contracts, handlers, and mapping workflow logic in `ResumeEnhancer.ResumeModule.SL`.
* Keep domain entities and domain-only concepts in `ResumeEnhancer.ResumeModule.DM`.
* Keep EF configuration, repository adapters, and schema-specific persistence behavior in `ResumeEnhancer.ResumeModule.PL`.
* Keep shared infrastructure behavior inside `application/Infrastructure`.
* Keep frontend feature logic inside `application/WebSolution/websolution.client/src/features`.

Do not introduce a new shared abstraction, cross-module contract, persistence boundary, composition pattern, or frontend primitive until existing ownership and extension points have been inspected.

---

## Working Style

* Read the relevant README, requirement, story, approved implementation plan, target implementation files, and nearest tests before changing behavior.
* Inspect the smallest relevant code surface first. Expand discovery only when evidence shows the change crosses another boundary.
* Follow existing naming, folder, dependency, registration, validation, error-handling, and test patterns before introducing new structure.
* Prefer extending current flows over inventing parallel abstractions.
* Keep changes scoped to the requested behavior.
* Make business intent traceable to existing requirement, story, OpenSpec, or implementation-plan artifacts when available.
* Distinguish repository evidence from inference. Do not silently turn an inferred convention into a repository rule.
* Default to autonomous execution. Do not ask the user for routine confirmation, permission to inspect repository evidence, permission to run non-destructive commands, or clarification that can be resolved from repository authority, current code, tests, or an existing approved plan.
* Before asking the user an interview or clarification question, first exhaust the smallest applicable repository evidence and safe reversible options. Use the host's structured question UI when available. Fall back to concise chat questions only when structured questioning is unavailable.

---

## Autonomous Execution Contract

Autonomy is the default operating mode.

The parent agent owns the end-to-end execution loop:

```text
understand request
    ↓
resolve minimum authority and current evidence
    ↓
identify the next safe step
    ↓
execute directly or delegate with explicit scope
    ↓
observe evidence and delegated status
    ↓
verify the result of the step
    ↓
reconcile state
    ↓
take the next safe action automatically
```

The parent agent should continue this loop without user interaction while the next action is:

* inside the requested and approved scope
* consistent with repository authority and ownership
* reversible or low-risk
* non-destructive
* supported by available evidence
* not blocked by a required approval, credential, or material product decision

Do not ask the user merely to choose between implementation details when repository evidence, established conventions, tests, or the approved plan provide a safe answer.

When uncertainty is non-material, prefer the safest reversible option that matches existing repository patterns, record the assumption in progress evidence, and continue.

When one workstream is blocked, continue independent safe workstreams instead of stopping the entire task.

### User-interaction threshold

Interrupt the user only when at least one of these conditions is true:

* a workflow or approval gate explicitly requires user approval
* a material product, scope, security, data, UX, compatibility, or architectural decision has multiple plausible outcomes and repository evidence cannot resolve it
* required information is missing and cannot be derived from current repository evidence or connected tools
* a required credential, secret, external authorization, or unavailable dependency must be supplied by the user
* the next action is destructive, irreversible, externally consequential, or outside the already approved scope
* requirements or authorities materially conflict and no safe precedence rule resolves the conflict

Do not use a user interview for curiosity, preference gathering, confirmation of obvious repository conventions, or decisions that can safely be deferred.

Before interviewing the user:

1. identify the exact blocking decision
2. state the evidence already checked
3. state why the gap is material
4. narrow the question to the smallest decision the user must make
5. continue every independent safe action before waiting on the answer

If several blockers are already known, batch only the related blocking decisions into one structured interview rather than repeatedly interrupting the user.

### Next-safe-action policy

After every completed, failed, or blocked step, the parent agent must choose the next safe action from actual evidence.

Prefer, in order:

1. continue the approved plan
2. verify the just-completed step
3. repair a local failure within the same approved scope
4. retrieve narrowly scoped evidence needed for the next decision
5. delegate an independent owned workstream
6. continue another unblocked workstream
7. request a required approval or user decision only when the user-interaction threshold is met
8. stop only when no safe action remains

Never ask the user what to do next when the governing plan, repository evidence, or the next-safe-action policy already determines the answer.

---

## Knowledge Retrieval Contract

`KnowledgeBase/INDEX.md` is the sole default discovery entry point for repository knowledge.

### Required behavior

1. Read `KnowledgeBase/INDEX.md`.
2. Identify the decision area affected by the task.
3. Read only the knowledge topics explicitly applicable to that decision.
4. Retrieve ADRs only when the task actually touches the decision governed by that ADR.
5. Inspect volatile implementation facts directly from current code and tests rather than expecting durable knowledge to contain them.

### Prohibited behavior

Do not:

* enumerate or glob `KnowledgeBase/**` as a default discovery strategy
* read every knowledge file "to be safe"
* recursively follow every link found inside a selected knowledge artifact
* treat a reference link as an automatic reading requirement
* reload the same authority repeatedly in one delegated workstream without a concrete reason
* use the knowledge base as a substitute for inspecting current implementation

A link inside a knowledge artifact is a reference, not a transitive context-loading instruction.

### Default knowledge budget

For a focused task, the default authority budget is:

* up to 3 knowledge topics
* up to 2 ADRs

This is a routing guideline, not a correctness limit.

Exceed it only when the task is genuinely cross-cutting. Before loading each additional authority, identify the decision that requires it.

Examples:

* endpoint validation change:

  * API/application knowledge
  * project adaptation knowledge if needed

* EF Core migration/change:

  * persistence knowledge
  * applicable persistence ADR if one governs the decision

* local React form change:

  * frontend routing/guideline authority
  * no backend or persistence knowledge unless the API contract changes

* new cross-module abstraction:

  * modular architecture knowledge
  * applicable module-boundary ADR

### Context expansion

If implementation reveals a decision outside the current authority set:

1. state the newly discovered decision area
2. retrieve the applicable authority through `KnowledgeBase/INDEX.md`
3. continue only after reconciling the new authority with the current approach

Do not restart broad discovery.

---

## Delegated Agent Context

The parent agent or orchestrator should resolve common repository context once and provide each delegated agent with a focused context manifest.

A delegated agent should not independently rediscover the entire repository knowledge graph when the parent has already supplied validated context.

A context manifest should include, where applicable:

```yaml
task:
  id: "<story/change/task>"
  objective: "<assigned outcome>"

scope:
  owned_paths:
    - "<path>"
  excluded_paths:
    - "<path>"

decision_areas:
  - "<api|persistence|architecture|frontend|security|...>"

required_context:
  - "AGENTS.md"
  - "KnowledgeBase/INDEX.md"
  - "<selected knowledge authority>"
  - "<approved implementation plan>"
  - "<source story/change>"

conditional_context:
  - path: "<knowledge or ADR>"
    when: "<specific decision that requires it>"

explicitly_not_required:
  - "<irrelevant decision area>"

coordination:
  shared_contract_owner: "<agent/workstream if applicable>"
  migration_owner: "<agent/workstream if applicable>"
  composition_owner: "<agent/workstream if applicable>"

progress_reporting:
  parent_step_id: "<parent workflow step>"
  reporting_mode: "event-driven"
  required_events:
    - "started"
    - "checkpoint"
    - "blocked"
    - "completed"
    - "failed"
```

### Delegated-agent rules

A delegated agent must:

* read the supplied manifest
* validate that the assigned task and worktree match the manifest
* use the supplied authority set as the starting context
* inspect current code and tests in its assigned slice
* expand knowledge only when implementation encounters a decision outside the manifest
* emit progress events to the parent against the assigned `parent_step_id`
* report a status event when work starts, after each meaningful evidence/implementation/verification checkpoint, when blocked, when a step fails, and when the assignment completes
* include the next safe action in every non-terminal progress event
* report any required context expansion immediately and again in the final handoff

A delegated agent must not:

* load all KnowledgeBase files
* repeat parent-agent discovery without evidence that the supplied context is incomplete
* silently modify shared contracts, migrations, composition, or shared UI primitives owned by another workstream
* expand its assignment merely because neighboring code is related

---

## Skills And Agents

Discover repository skills under:

```text
.codex/skills/<skill-name>/SKILL.md
```

and custom agents under:

```text
.codex/agents/*.toml
```

before choosing a workflow.

Read a selected skill's `SKILL.md` before acting.

Read only routed references that apply to the current decision.

Use:

* `AGENTS.md` for durable repository-wide constraints
* skills for reusable task guidance
* custom agents for focused delegated workstreams
* `KnowledgeBase/INDEX.md` for durable project-authority routing
* approved implementation plans for change-specific implementation decisions

A delegated agent does not replace the parent agent's ownership of scope, coordination, or final integration.

---

## Implementation Planning

Use the `implementation-planner` custom agent for OpenSpec implementation planning.

Plans are stored under this path in the canonical branch worktree for the task:

```text
.tmp/ImplementationPlans/<change-name>/
```

The implementation planner creates and revises task plans in the canonical
branch worktree for the task. Task planning must not occur in the main
checkout. The approved plan remains in that branch worktree and is referenced
directly by the implementation owner; it is not copied or synchronized into a
second worktree.

OpenSpec proposal artifacts are authored in the main checkout and must be
synchronized into the branch worktree before task planning or implementation
continues there. Proposal-artifact synchronization is separate from
implementation-plan creation.

Keep these responsibilities separate:

* OpenSpec proposal artifacts define the requested change, requirements,
  design, specifications, and task identities. They are not implementation
  plans.
* The implementation planner consumes the approved proposal artifacts and
  creates a separate task or batch implementation plan in the branch
  worktree. It must not create, revise, or replace proposal
  artifacts, specifications, or task identities.
* The implementation plan defines execution ownership, dependencies, files,
  validation, risks, and handoff decisions. It must not redefine approved
  product scope or silently add OpenSpec tasks.

The planner may write only `Proposed` plans until the user explicitly approves every covered task.

The implementation planner must:

* inspect applicable KnowledgeBase topics and ADRs through `KnowledgeBase/INDEX.md`
* follow the Knowledge Retrieval Contract
* use `$research-deep` for material evidence gaps
* use `$workflow-user-interview` for unresolved material user decisions
* inspect relevant current code and tests
* record evidence, assumptions, decisions, dependencies, risks, validation, and focused code snippets
* perform a recorded self-review before requesting approval
* avoid loading unrelated knowledge purely for completeness

Nothing material to implementation may remain implicit.

---

## Implementation Approval Gate

Any agent implementing production code, tests, migrations, or configuration must read the matching plan under:

```text
.tmp/ImplementationPlans/<change-name>/
```

Before the first implementation edit, verify:

* `status: Approved`
* change identity
* assigned task identity
* worktree/branch identity when applicable
* plan freshness against material repository changes
* consistency between the assigned task and current implementation scope

Stop before editing when the plan is:

* missing
* unapproved
* stale
* assigned to a different change/task
* materially inconsistent with the repository or requested behavior

Do not repair approval metadata implicitly.

For OpenSpec implementation, the parent or coordinator may inspect, plan,
reconcile, and review, but must not directly edit production code, tests,
migrations, or configuration. Route the approved plan to exactly one primary
implementation owner and keep read-only review lanes separate from that owner.

---

## Generated And Repository-Owned Workflow Boundaries

Keep these mechanisms separate:

* skill UI metadata:

  * `.codex/skills/<skill-name>/agents/openai.yaml`

* OpenSpec-generated workflows:

  * `.agents/skills/`

* custom-agent definitions:

  * `.codex/agents/`

OpenSpec workflows under `.agents/skills/` are generator-managed.

Do not edit generated OpenSpec workflow files directly for repository-specific behavior.

Use:

```text
openspec update
```

to refresh generated workflows.

Keep ResumeEnhancer-specific policy in:

* `$openspec-repository-policy`
* `AGENTS.md`
* repository-owned `.codex/skills/`
* applicable KnowledgeBase authorities

---

## Skill Selection

Select the smallest set of skills necessary for the current task.

Do not load every skill in a category.

### Backend delivery

Use as applicable:

* `$backend-feature-development`
* `$backend-dotnet-patterns`
* `$backend-ef-core`
* `$backend-security`
* `$quality-performance`

### Architecture and domain

Use as applicable:

* `$architecture-review`
* `$backend-dotnet-architecture`
* `$architecture-domain-modeling`
* `$architecture-adr`

### Frontend delivery

Use as applicable:

* `$frontend-development`
* `$frontend-guidelines`
* `$frontend-react-patterns`
* `$frontend-design`
* `$frontend-production-ui`
* `$frontend-security`

### Full-stack and story delivery

Use as applicable:

* `$delivery-full-stack-feature`
* `$delivery-user-story-kickoff`
* `$delivery-issues-kickoff`
* `$openspec-workflow`
* `$workflow-planning`
* applicable generated OpenSpec skills under `.agents/skills/`

### Development entry gate

Use `$workflow-development-entry` before development-related:

* backend code
* frontend code
* tests
* configuration
* migrations
* architecture implementation

It routes specialist skills and returns readiness.

It does not invoke implementation agents.

### OpenSpec repository policy

Use `$openspec-repository-policy` as an overlay with generated OpenSpec workflows.

It preserves repository-specific:

* approval gates
* worktree sequencing
* authority routing
* delivery evidence requirements

### Development handoff

`$openspec-workflow`:

* keeps proposal work in the main checkout
* obtains explicit user approval
* creates the canonical branch/worktree only when implementation and task based planning can be ready to begin

`$openspec-orchestration`:

* coordinates approved development work inside that worktree
* resolves shared ownership
* prepares focused agent assignments
* should provide context manifests to delegated implementation agents

Implementation agents are reached only through the approved coordinator flow.

### Review, quality, and security

Use as applicable:

* `$quality-production-code-review`
* `$quality-ai-code-review`
* `$quality-code-review-checklist`
* `$quality-clean-code`
* `$quality-refactoring`
* `$security-management`
* `$frontend-design-review`

### Research and durable guidance

Use as applicable:

* `$research-deep`
* `$knowledge-project-builder`
* `$documentation-generation`
* `$documentation-readme`
* `$product-prd`
* `$product-user-story`

Use `$knowledge-project-builder` for reusable `KnowledgeBase/` artifacts, not ordinary documentation.

### Delivery operations

Use as applicable:

* `$git-workflows`
* `$git-worktrees`
* `$git-commit`
* `$delivery-pull-request`

Use these when managing:

* branches
* worktrees
* commits
* synchronization
* history operations
* pull requests

### User decisions

Use `$workflow-user-interview` only when a selected workflow requires user-confirmed material decisions or the Autonomous Execution Contract's user-interaction threshold is met.

Before invoking it, exhaust the smallest applicable repository evidence and safe reversible options.

Do not replace a required interview with an assumption.

Do not interview the user for non-material implementation preferences, routine confirmations, or information that can be derived from repository evidence.

When a required user decision blocks only part of the task, continue all independent safe work before waiting for the decision.

---

## Frontend Operating Model

Use:

```text
.codex/skills/frontend-guidelines/references/frontend-workflow-routing.md
```

to select frontend skills and the `frontend-implementer` agent.

Resolve material uncertainty before frontend delivery:

* use `$research-deep` for significant evidence gaps
* use `$workflow-user-interview` for material user decisions evidence cannot answer

Assign one primary implementation owner per frontend workstream.

`$frontend-development` is the default frontend implementation authority.

`$frontend-production-ui` is an alternative only for an explicitly UI-dominant surface. It is not a parallel implementation owner.

Treat `$frontend-guidelines` as a non-delegating standards authority.

Use design, React-pattern, security, performance, review, and accessibility guidance only when their trigger applies.

Specialist skills return:

* constraints
* findings
* evidence
* recommendations
* handoff information

They do not:

* recursively invoke unrelated frontend skills
* spawn `frontend-implementer`
* duplicate implementation ownership

---

## Custom Agent Selection

### `implementation-planner`

Use for:

* approval-gated OpenSpec task planning
* dependency analysis
* implementation evidence
* validation strategy
* focused code snippets

It does not implement production code.

### `backend-implementer`

Use for assigned backend slices involving:

* Minimal APIs
* validation
* Mediator/application flow
* mapping
* persistence
* backend tests

It must use the supplied context manifest and must not independently load unrelated KnowledgeBase topics.

### `frontend-implementer`

Use for assigned frontend slices involving:

* React/TypeScript features
* routes
* forms
* client state
* typed API integration

It must use the supplied context manifest and frontend routing authority.

### `knowledge-researcher`

Use for:

* evidence gathering
* reusable project knowledge
* approval-driven KnowledgeBase work

It does not implement product code.

Broad KnowledgeBase inspection is appropriate only when the research question itself requires catalog-level analysis.

### `security-auditor`

Use for focused review of:

* trust boundaries
* authentication
* authorization
* validation
* sensitive data exposure
* abuse resistance
* OWASP-oriented concerns

Do not make it a default implementer.

### `code-reviewer`

Use for defect-first review of real diffs across:

* correctness
* security
* architecture
* maintainability
* tests

It should review the change and its governing authorities, not reload unrelated project knowledge.

### `story-orchestrator`

Use for:

* approved multi-story dependency ordering
* workstream decomposition
* parallel-work planning
* shared ownership assignment
* context-manifest preparation
* review-ready handoff

The orchestrator should resolve shared knowledge once where possible rather than requiring every sub-agent to repeat discovery.

---

## Parallel Work

Implementation agents receive their branch or worktree from `$openspec-workflow` after proposal approval.

Use:

* `$git-worktrees` for isolated workspace creation or cleanup
* `$git-commit` for staging, commit, and push decisions
* `$git-workflows` only for synchronization, history rewriting, cherry-picking, bisect, or recovery

Before parallelizing work:

1. identify shared contracts
2. identify migrations
3. identify module composition changes
4. identify shared UI primitives
5. identify generated artifacts
6. assign explicit ownership
7. resolve dependencies
8. prepare focused context manifests

Do not let multiple agents independently mutate:

* shared contracts
* migrations
* composition
* shared UI primitives
* generated workflow artifacts

Keep cross-layer contract changes in one coordinating lane unless the boundary is explicitly decomposed.

Use agents only when delegation materially improves the task.

---

## Verification Expectations

For code changes:

1. run the smallest meaningful tests for the touched area
2. run broader verification only when the affected boundary requires it
3. state exactly what was verified
4. state what was not verified
5. do not claim success from planned commands

For documentation, prompt, skill, agent, or instruction changes:

* verify document structure
* verify referenced paths
* verify cross-references
* check for contradictory guidance
* inspect the resulting diff

Do not claim a skill or custom agent is auto-discovered in the current session unless that behavior was actually observed in a fresh Codex task.

---

## Commands

Full solution build:

```powershell
dotnet build application\ResumeEnhancerApp.slnx
```

Unit tests:

```powershell
dotnet test test\ResumeEnhancer.Tests\ResumeEnhancer.Tests.Unit.csproj --no-restore
```

Integration tests:

```powershell
dotnet test test\IntegrationTest\ResumeEnhancer.Tests.Integration.csproj --no-restore
```

API host:

```powershell
dotnet run --project application\WebSolution\WebSolution.Server\ResumeEnhancer.WebSolution.Server.csproj
```

Migration help:

```powershell
dotnet run --project application\Infrastructure\Migration\ResumeEnhancer.Infrastructure.Migration.csproj -- --help
```

---

## Checks Required For Instruction-Oriented Changes

When changing:

* `AGENTS.md`
* `.codex/`
* `Prompts/`

review the edited files for:

* broken paths
* broken cross-references
* contradictory guidance
* duplicated authority
* accidental recursive delegation
* instructions that encourage broad KnowledgeBase loading
* stale generated-vs-repository-owned workflow assumptions

Run:

```powershell
Get-ChildItem -Recurse .codex, Prompts
```

Then run:

```powershell
git diff -- AGENTS.md .codex Prompts
```

When changing knowledge-routing behavior, also inspect:

```text
KnowledgeBase/INDEX.md
```

and confirm that agent instructions route through the index rather than hard-coding or bulk-loading KnowledgeBase content.

---

## Knowledge Artifact Location

Save durable project knowledge under:

```text
KnowledgeBase/
```

unless the user explicitly requests another location.

New durable knowledge must have a clear decision scope and must be added to `KnowledgeBase/INDEX.md` when agents are expected to retrieve it.

Do not create a new knowledge artifact when:

* an existing authority already owns the decision
* the information is volatile implementation detail
* the content belongs in a README, requirement, ADR, skill, or implementation plan instead
* the only purpose is to avoid reading the current code

---

## Core Operating Principle

Use the smallest sufficient authority set. Repository safety comes from selecting the correct authority and validating current evidence, not from maximizing context volume.

The default flow is:

```text
Request / Story / OpenSpec change
        ↓
AGENTS.md + KnowledgeBase/INDEX.md
        ↓
Minimum applicable skills, requirements, ADRs, and plans
        ↓
Current implementation + nearest tests + Git/worktree state
        ↓
OpenSpec approval + canonical worktree (when applicable)
        ↓
Development-entry readiness gate before implementation
        ↓
Approved implementation plan + exactly one implementation owner
        ↓
Implementation
        ↓
Review + verification
        ↓
Evidence reconciliation + delivery status
```

### Delegated Work

For delegated work, the parent or orchestrator resolves common context once and provides a focused context manifest containing:

* task/change identity
* objective
* owned paths
* excluded paths
* decision areas
* required authorities
* conditional authorities
* approved plan/task references
* shared-file ownership
* branch/worktree identity when applicable

The delegated agent must:

* validate the manifest against its actual assignment and worktree
* inspect the assigned implementation surface and nearest tests
* remain within its declared write scope
* expand context only when new evidence introduces a decision outside the supplied authority set
* report any context or scope expansion before relying on it
* stop when required ownership, approval, or dependency information is inconsistent

The delegated agent must not:

* rediscover the entire KnowledgeBase or authority graph
* reload unrelated skills or ADRs for completeness
* silently expand its task
* edit shared contracts, migrations, composition, generated artifacts, or shared UI primitives owned by another workstream

### OpenSpec Implementation

OpenSpec implementation is additionally gated. The detailed approval
requirements are defined in the Implementation Approval Gate above; this
section summarizes the lifecycle without creating a second approval authority.

The lifecycle sequence is:

```text
OpenSpec proposal artifacts in main checkout
        ↓
OpenSpec approval
        ↓
Canonical branch worktree
        ↓
Synchronize OpenSpec proposal artifacts into the branch worktree
        ↓
$workflow-development-entry
        ↓
Implementation plan created from the approved proposal/task identities
        ↓
Explicit approval of the implementation plan in the branch worktree
        ↓
Validate the approved plan in the branch worktree
        ↓
Validate approval metadata, task identity, and branch-worktree identity
        ↓
Exactly one implementation owner
        ↓
Implementation + targeted verification
        ↓
Review + evidence reconciliation
```

The parent or coordinator must not directly edit production code, tests,
migrations, or runtime configuration for an OpenSpec implementation, whether
or not a delegated implementation owner is currently active.

Planning, coordination, implementation, review, and verification are separate responsibilities. Do not collapse them merely to reduce handoffs.

### Delegated Progress And Observability

Delegated progress must be observable to the parent agent in near real time, subject to the host's actual sub-agent/event capabilities.

The parent agent must assign a stable `parent_step_id` to each delegated step or workstream. A delegated agent must report against that step ID rather than sending unstructured "working" messages.

A delegated agent must emit a status event:

* immediately after accepting the assignment
* after each meaningful discovery/evidence checkpoint
* after each meaningful implementation checkpoint
* after each verification checkpoint
* immediately when blocked
* immediately after a material command or verification failure that changes the next action
* when its assignment completes

Do not emit redundant heartbeat noise when no observable state changed.

Use this status envelope where the host supports structured messages; otherwise preserve the same fields in text:

```yaml
status:
  update_seq: <monotonic integer>
  parent_step_id: "<parent workflow step>"
  task_id: "<story/change/task>"
  agent: "<agent/workstream>"
  state: "<started|checkpoint|blocked|completed|failed>"
  phase: "<discovery|planning|implementation|verification|review|handoff>"
  summary: "<what changed since the previous event>"

evidence:
  inspected:
    - "<file/symbol/authority>"
  changed:
    - "<file/symbol>"
  commands:
    - command: "<command>"
      result: "<actual result>"
  checks:
    - "<test/check and actual outcome>"

control:
  blocker: "<none or exact blocker>"
  needs_user: <true|false>
  user_decision: "<smallest required decision or null>"
  context_or_scope_expansion: "<none or exact expansion>"
  next_safe_action: "<specific next action>"
```

Every event must be based on actual observed evidence. Do not report planned commands as executed, planned edits as changed files, or expected tests as passing.

The parent agent must consume each status event and promptly:

1. reconcile it with the parent task state
2. surface a concise user-visible progress update when the host supports progress messages and the update represents a meaningful state change
3. state the current parent step, owner, observable evidence, and next safe action
4. take the next safe action automatically when `needs_user: false`
5. continue other independent work when one delegated step is blocked
6. invoke the user-interaction threshold only when `needs_user: true` is justified by evidence

User-visible progress should describe evidence, not internal speculation. Prefer concise updates such as:

```text
Step 3/7 — backend-implementer — verification
Changed: <files/symbols>
Verified: <actual checks/results>
Next: <next safe action>
```

Do not expose hidden chain-of-thought or private scratch reasoning. Report actions, evidence, decisions, blockers, and next steps.

Progress updates and final handoffs should report, as applicable:

* assigned change/task IDs
* parent step ID
* implementation owner
* branch/worktree
* declared write scope
* current phase and state
* files and symbols inspected
* files and symbols changed
* authorities consulted
* commands executed
* actual command results
* tests/checks completed
* remaining work
* blockers
* whether user input is actually required
* context or scope expansions
* next safe action

Never report only that an agent is "working", "still working", or "making progress".

Report observable evidence instead.

### Evidence Reconciliation

Reconcile implementation state only from actual implementation and verification evidence.

Keep these states distinct:

```text
local change
≠ local verification
≠ commit
≠ push
≠ hosted pull request
≠ CI success
≠ review approval
≠ merge
≠ final delivery completion
```

Do not infer a later state from an earlier one.

For example:

* passing local tests does not mean CI passed
* creating a commit does not mean it was pushed
* pushing a branch does not mean a pull request exists
* opening a pull request does not mean it was reviewed
* CI success does not mean the change was merged
* merge does not automatically prove every requested delivery step was completed

Task, OpenSpec, issue, and delivery status must be updated only when the corresponding implementation or verification evidence exists.

### Governing Principle

Prefer:

```text
correct authority
+ current repository evidence
+ explicit ownership
+ approved scope
+ focused context
+ observable verification
```

over:

```text
maximum context
+ broad discovery
+ duplicated ownership
+ inferred progress
```

The goal is not to make every agent know everything.

The goal is to ensure that the agent responsible for a decision has the smallest sufficient context, the correct authority, an explicit scope, and verifiable evidence for the result.

The parent agent should remain in motion: reconcile evidence, choose the next safe action, and continue autonomously until a genuine user-interaction threshold or terminal state is reached.
