---
kind: workflow-plan
status: Approved
scope: batch
change: parent-sub-agent-communication-hardening
tasks:
  - lifecycle-state-machine
  - explicit-task-events
  - heartbeat-and-lost-agent-detection
  - parent-child-tracing
  - completion-acknowledgement
created_at: 2026-09-21
plan_version: 1
current_phase: Complete; repository guidance implemented and reviewed
intended_owner: workflow-orchestrator
approved_by: automated-plan-review-approval
approved_at: 2026-09-21
approval_notes: >-
  Approved after review of requirements coverage, ownership, repository
  conventions, failure handling, validation, rollback, and capability-aware
  tracing. Host event and OpenTelemetry availability is explicitly a Phase 1
  evidence gate; unsupported capabilities use the documented fallback.
---

# Parent–Sub-Agent Communication and Task Status Hardening

## Goal and success criteria

Make delegated-work progress resumable and externally observable across the
ResumeEnhancer Codex workflow. A parent must be able to determine the current
state, evidence, blocker, ownership, and next safe action without inspecting
sub-agent logs. The plan covers the partial and missing items identified in the
communication audit; it does not change product runtime behavior.

Success means:

1. Delegated work has a documented state machine covering `Assigned`,
   `Accepted`, `In Progress`, `Blocked`, `Waiting for User`, `Completed`,
   `Failed`, and `Cancelled`, including valid transitions and terminal rules.
2. The workflow defines the named events `TaskAssigned`, `TaskStarted`,
   `TaskProgressUpdated`, `TaskBlocked`, `UserInputRequired`, `TaskCompleted`,
   `TaskFailed`, and `TaskCancelled`, with a stable correlation identity.
3. A parent can detect stale delegated work using a documented heartbeat or
   checkpoint timeout and can distinguish a lost agent from a slow agent.
4. Completion is not accepted until the sub-agent reports the required result,
   the parent acknowledges and reconciles it, and the user receives the
   relevant outcome or next action.
5. OpenTelemetry integration is defined as a capability-aware contract. If the
   host exposes tracing hooks, parent and child work are correlated; if it does
   not, the text/status envelope remains the required fallback and the gap is
   reported rather than simulated.
6. The relevant skills, custom-agent contracts, and `AGENTS.md` agree without
   duplicating contradictory authority.

## Current state and evidence

Confirmed in the current repository on 2026-09-21:

- `AGENTS.md` already requires a `parent_step_id`, evidence-based status
  updates, user-visible progress, automatic next-safe-action selection, and
  parent reconciliation.
- `AGENTS.md` currently permits only the status values `started`, `checkpoint`,
  `blocked`, `completed`, and `failed`; it does not define the requested full
  lifecycle or named task events.
- `AGENTS.md` requires near-real-time observability but has no heartbeat,
  timeout, stale-agent, or recovery rule.
- `AGENTS.md` requires a structured text envelope but has no OpenTelemetry
  trace/span contract or host capability detection.
- `.codex/skills/openspec-orchestration/SKILL.md` requires delegated lanes to
  return identity, progress, changed paths, verification, findings, and a safe
  next transition, but does not define the full event/state protocol.
- `.codex/skills/orchestration-agent-improvement/SKILL.md` requires explicit
  ownership, approval checkpoints, validation, and synthesis, but does not
  define communication transport or stale-agent recovery.
- `implementation-planner.toml`, `backend-implementer.toml`, and
  `code-reviewer.toml` define role boundaries and handoff content, but do not
  share a common lifecycle/event schema.

These observations are repository evidence, not assumptions about capabilities
of the current Codex host. The host's available background-agent event and
tracing APIs must be inspected before promising runtime automation.

### Phase 1 capability matrix — 2026-09-21

| Capability | Observed host support | Repository fallback |
|---|---|---|
| Spawn delegated agent | Available through `multi_agent_v1__spawn_agent` | Record assignment and `parent_step_id` in the parent update |
| Send progress/input | Available through `multi_agent_v1__send_input` | Use the canonical status envelope in the message |
| Wait for terminal result | Available through `multi_agent_v1__wait_agent` | Parent polls at bounded checkpoints and records the observed result |
| Resume agent | Available through `multi_agent_v1__resume_agent` | Record resume as a new observable transition with the same task identity |
| Close/cancel agent | Available through `multi_agent_v1__close_agent` | Record cancellation and preserve the last confirmed evidence |
| Structured progress-event subscription | Not exposed in the current tool registry | Parent polling plus evidence-based user updates |
| Native heartbeat/lost-agent detector | Not exposed in the current tool registry | Bounded parent checkpoint and stale-status recovery rule |
| OpenTelemetry trace/span API | Not exposed in the current tool registry | Correlation metadata in the status envelope; do not claim telemetry coverage |

Phase 1 conclusion: the parent session exposed agent spawn, messaging, wait,
resume, and close operations, so the repository can define and enforce a
reliable communication contract around explicit messages and bounded parent
polling. Sub-agents and future hosts may not receive the same tool registry;
therefore this matrix is parent-session evidence, not a universal host claim.
Native event streaming, heartbeat automation, and OpenTelemetry instrumentation
remain capability-gated extensions rather than hidden requirements.

## Scope and boundaries

### Included

- A repository-owned delegation protocol and state-transition table.
- A canonical event/status envelope and completion handoff contract.
- Heartbeat/checkpoint expectations, stale detection, and recovery behavior
  that can be followed with available host events or text fallback.
- Capability-aware OpenTelemetry correlation requirements.
- Alignment of `AGENTS.md`, orchestration skills, and custom-agent contracts.
- Structural validation and a realistic workflow exercise.

### Excluded

- Product application code, database schema, migrations, or API changes.
- Changes to generated `.agents/skills/` files.
- Fabricating a background-agent transport, heartbeat service, or telemetry
  exporter when the host does not expose one.
- Rewriting the OpenSpec approval, implementation ownership, or delivery gates.

Replanning is required if implementation reveals that a host/plugin API must be
added outside repository-owned instructions, or if the user wants a runtime
orchestration service rather than policy and capability integration.

## Workstreams and ownership

1. **Protocol owner — workflow-orchestrator**
   - Define states, transitions, event names, correlation IDs, acknowledgement,
     and terminal-state rules.
2. **Repository-policy owner — instruction maintainer**
   - Update `AGENTS.md` with the authoritative protocol, fallback behavior, and
     user-visible reporting requirements.
3. **Skill-contract owner — orchestration maintainer**
   - Align `openspec-orchestration` and `orchestration-agent-improvement`; keep
     generated OpenSpec workflows untouched.
4. **Agent-contract owner — custom-agent maintainer**
   - Align planner, implementer, reviewer, and orchestrator handoff/output
     contracts with the common envelope.
5. **Verification owner — code-reviewer or workflow verifier**
   - Review the diff for contradictions, missing transitions, unsupported
     claims, and evidence/reporting gaps.

The protocol owner retains final synthesis and may not silently transfer that
responsibility to a delegated implementation lane.

## Phases and actions

### Phase 1 — Capability and baseline confirmation

Inspect the current task/agent event capabilities available to the host and
record whether structured events, polling, cancellation, timestamps, and
OpenTelemetry hooks are actually available.

Output: a capability matrix with `available`, `unavailable`, or `unknown`, plus
the repository-only fallback for each unavailable capability.

Completion condition: no proposed mechanism depends on an unverified host API.

### Phase 2 — Define the canonical delegation protocol

Add a concise protocol reference under the repository-owned `.codex/skills/`
guidance (or another existing workflow-authority location) containing:

- lifecycle states and allowed transitions;
- event names and required payload fields;
- `parent_step_id`, `task_id`, agent/workstream, branch/worktree, and monotonic
  `update_seq` correlation fields;
- evidence requirements for files, commands, tests, findings, warnings,
  remaining work, blocker, user-input requirement, and next safe action;
- completion acknowledgement and parent reconciliation rules;
- cancellation, failure, retry, and stale-agent recovery semantics.

Completion condition: every requested state and event has one owner, entry
condition, exit condition, and observable evidence requirement.

### Phase 3 — Align repository instructions and agent contracts

Update `AGENTS.md`, `openspec-orchestration`,
`orchestration-agent-improvement`, and only the custom-agent definitions that
emit or consume delegated status. Keep generated `.agents/skills/` files
unchanged. Remove or reconcile any duplicate envelope that disagrees with the
canonical protocol.

Completion condition: a search finds one authoritative state/event contract,
all referenced paths exist, and no selected agent is instructed to report only
“working” or to claim completion without evidence and parent acknowledgement.

### Phase 4 — Add capability-aware observability rules

Document the following order:

1. Use host structured events and native parent-child tracing when available.
2. Otherwise use the canonical text/YAML envelope and parent polling/checkpoint
   evidence.
3. Mark tracing as unavailable rather than inventing spans or claiming
   OpenTelemetry coverage.
4. Treat a missing checkpoint beyond the configured threshold as `Blocked` or
   `Failed` only after the recovery check; preserve the last confirmed evidence.

Define default checkpoint expectations as workflow policy, not a hidden runtime
constant: the parent records the last observed event time, performs a bounded
status check, and escalates only after the documented stale threshold and
recovery attempts are exhausted.

Completion condition: the protocol explains slow, blocked, failed, cancelled,
and lost-agent cases without conflating them.

### Phase 5 — Validate and hand off

Run structural checks, inspect the scoped diff, and execute a tabletop workflow
exercise covering:

- normal assignment through acknowledged completion;
- progress followed by a blocker and automatic continuation of an independent
  lane;
- explicit user-input requirement;
- failed task and retry/reassignment;
- cancellation;
- missing heartbeat/lost-agent recovery;
- host tracing unavailable with text fallback.

Output: validation evidence and a final handoff stating the implemented
repository guidance, unsupported host capabilities, residual risks, and one
next safe action.

Completion condition: all scenarios have observable expected states/events and
the review finds no contradictory instruction.

## Decisions, dependencies, and risks

### Confirmed decisions

- The parent remains the owner of overall workflow state and final synthesis.
- The protocol must be evidence-first and must not expose hidden reasoning.
- Generated OpenSpec workflows remain generator-managed.
- Missing host capabilities must be reported honestly with a fallback.

### Decisions to validate in Phase 1

- Whether the host exposes structured task events and cancellation callbacks.
- Whether OpenTelemetry spans can be created or only represented as correlation
  metadata.
- The practical stale-check interval and maximum recovery attempts supported by
  the host. These are operational defaults, not product requirements.

### Risks and mitigations

- **False lost-agent detection:** require a bounded recovery check and preserve
  the last confirmed event before changing state.
- **Conflicting authorities:** keep the protocol in one repository-owned
  reference and make `AGENTS.md` and skills route to it.
- **Over-reporting noise:** require meaningful checkpoints and suppress
  redundant heartbeats when no observable state changes.
- **Unsupported telemetry claims:** gate OpenTelemetry language on a verified
  host capability matrix.
- **Recursive delegation:** preserve one parent owner and prohibit delegated
  agents from creating additional workflow owners unless explicitly assigned.

## Validation and evidence

- `Get-Content -Raw AGENTS.md` and selected skill/agent files: confirm the
  protocol references and boundaries are coherent.
- `rg` over `AGENTS.md`, `.codex/skills`, and `.codex/agents`: confirm all event
  names, states, and required fields have one consistent definition.
- `Get-ChildItem -Recurse .codex, Prompts`: satisfy instruction-change checks.
- `git diff --check -- AGENTS.md .codex Prompts .tmp/WorkflowPlans`: no
  whitespace errors.
- A read-only tabletop trace: record expected event/state sequences for all
  Phase 5 scenarios.
- A focused review by `code-reviewer`: verify no generated files, production
  code, OpenSpec state, or delivery state was changed.

Not required for this plan: application unit tests or integration tests, unless
Phase 1 discovers that a repository runtime component must be implemented.

## Rollback and recovery

- Keep the protocol reference and instruction edits in one deliberate change
  boundary so they can be reverted together.
- If a capability-specific rule is unsupported, retain the text fallback and
  mark the unsupported integration as a documented limitation.
- If the revised protocol conflicts with an existing skill, stop and reconcile
  the authority before implementation; do not patch generated workflows.

## Completion and handoff

The work is complete when the protocol, repository instructions, selected agent
contracts, capability matrix, and tabletop validation agree. The handoff must
include changed paths, exact validation results, unsupported capabilities,
remaining risks, and one next safe action. No production implementation or
delivery status is implied by this workflow plan.

Next safe action after approval: perform Phase 1 capability confirmation, then
revise the protocol only where current host evidence supports it.

## Self-review record

- Requirements coverage: all five missing/partial areas from the audit are
  represented as explicit workstreams and acceptance conditions.
- Architecture and ownership: repository policy, skills, and custom-agent
  contracts are separated; generated workflows and product code are excluded.
- Security and failure handling: stale detection, cancellation, failure,
  unsupported telemetry, and evidence preservation are covered.
- Validation: structural checks and a realistic tabletop sequence are defined.
- Remaining limitation: actual host event and OpenTelemetry capabilities are
  not yet verified and are intentionally a Phase 1 dependency.

## Final implementation and review evidence

Implemented within the approved scope:

- Added the canonical delegated-work protocol at
  `.codex/skills/orchestration-agent-improvement/references/delegation-protocol.md`.
- Routed `AGENTS.md` and `openspec-orchestration` to the canonical protocol
  without editing generated `.agents/skills/` workflows.
- Aligned all seven custom-agent contracts with the full status envelope,
  parent-step correlation, evidence, blocker, and next-action requirements.
- Recorded parent-session capability evidence and explicit configurable
  checkpoint defaults: 60-second checkpoint interval, 180-second stale
  threshold, and one recovery attempt.

Validation evidence:

- Both affected skills passed `quick_validate.py`.
- All seven custom-agent TOML files parsed successfully with Python `tomllib`.
- Protocol token and reference checks passed.
- `git diff --check -- AGENTS.md .codex Prompts .tmp/WorkflowPlans` passed;
  only normal LF/CRLF conversion warnings were emitted.
- Generated `.agents/skills` diff: none.
- Read-only `code-reviewer` final verdict: no blocking or important findings;
  no files edited; application tests not run because this is instruction-only
  scope.

Remaining warning: native OpenTelemetry and heartbeat/event-stream support are
not exposed in the current parent session. The text/status envelope and
bounded polling procedure remain the required fallback.

Parent acknowledgement: the reviewer result was consumed, reconciled with the
workflow state, and the next safe action is `Complete`.
