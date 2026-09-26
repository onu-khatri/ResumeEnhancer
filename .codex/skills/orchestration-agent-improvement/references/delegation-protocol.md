# Delegated Work Communication Protocol

This is the canonical repository protocol for parent/sub-agent task status.
`AGENTS.md`, orchestration skills, and custom agents should route here rather
than define competing envelopes.

## Correlation

The assignment must also include objective, owned/excluded paths, required and conditional authorities, approved plan/task references when applicable, shared-file owner, and branch/worktree identity. Use a focused manifest rather than copying unrelated conversation history. A child may not delegate again unless its assignment explicitly permits it.

Every delegated workstream has:

- `parent_step_id`: stable identifier assigned by the parent;
- `task_id`: story, change, or task identity;
- `agent`: assigned agent or workstream;
- `branch_worktree`: branch/worktree when applicable;
- `update_seq`: monotonically increasing event sequence.

The parent owns the state record and final synthesis. A sub-agent reports
evidence; it does not unilaterally mark the parent workflow complete.

## Lifecycle

| State | Entered by | Meaning | Valid next states |
|---|---|---|---|
| `Assigned` | Parent | Scope and owner were assigned but acceptance is not confirmed | `Accepted`, `Cancelled` |
| `Accepted` | Agent | Agent validated assignment, worktree, authority, and write scope | `In Progress`, `Blocked`, `Cancelled` |
| `In Progress` | Agent/parent | Work is actively executing or being verified | `In Progress`, `Blocked`, `Waiting for User`, `Completed`, `Failed`, `Cancelled` |
| `Blocked` | Agent/parent | Progress cannot continue because of a concrete dependency or failure | `In Progress`, `Waiting for User`, `Failed`, `Cancelled` |
| `Waiting for User` | Parent | A material user decision or authorization is required | `In Progress`, `Cancelled` |
| `Completed` | Parent after acknowledgement | Agent result was received, reconciled, and accepted for the assigned scope | terminal |
| `Failed` | Parent after evidence | The assignment cannot complete within scope after safe recovery | terminal or `Assigned` for a new assignment |
| `Cancelled` | Parent | Work was intentionally stopped or the agent was closed | terminal |

`TaskStarted` is the event that moves `Accepted` to `In Progress`. A missing
message is not itself proof of failure; the parent performs the stale-status
procedure before changing state.

## Events

The event names are stable even when the host transports them as structured
events or ordinary messages:

- `TaskAssigned`
- `TaskStarted`
- `TaskProgressUpdated`
- `TaskBlocked`
- `UserInputRequired`
- `TaskCompleted`
- `TaskFailed`
- `TaskCancelled`

Each event includes the correlation fields above, the current lifecycle state,
a concise summary, and evidence. A progress event must report only observed
changes. A completion event must include:

- status and work summary;
- files/symbols changed;
- commands and actual test/check results;
- findings, warnings, and remaining issues;
- whether user interaction is required;
- one recommended next safe action;
- context or scope expansion, or `none`.

Example message fallback:

```yaml
event: TaskProgressUpdated
state: In Progress
update_seq: 3
parent_step_id: "step-04"
task_id: "<story/change/task>"
agent: "<agent>"
branch_worktree: "<branch/worktree>"
summary: "<observed change since the previous event>"
evidence:
  inspected: ["<file/symbol/authority>"]
  changed: ["<file/symbol>"]
  commands:
    - command: "<command>"
      result: "<actual result>"
  checks: ["<actual check result>"]
control:
  blocker: none
  needs_user: false
  context_or_scope_expansion: none
  next_safe_action: "<one action>"
  telemetry: "<native|fallback|unavailable>"
```

## Parent acknowledgement

The parent must acknowledge every terminal or blocking event by:

1. validating the identity and evidence against the current worktree;
2. reconciling the parent task state and ownership record;
3. publishing a concise user-visible update for meaningful state changes;
4. selecting and executing the next safe action when `needs_user` is false.

Only after these actions may `TaskCompleted` be treated as complete. A
sub-agent's final message without parent reconciliation is a handoff, not a
completed workflow state.

## Heartbeat and lost-agent fallback

The preferred mechanism is a host-native progress event or heartbeat. When it
is unavailable, the parent uses bounded polling/checkpoints:

- configurable `checkpoint_interval`: recommended default 60 seconds;
- configurable `stale_threshold`: recommended default 180 seconds without an
  observable event;
- configurable `recovery_attempts`: recommended default 1 bounded check or
  resume operation before classifying the work as lost or failed.

These are workflow defaults, not application settings. A host may override
them when its wait/event semantics justify another value, but the selected
values must be recorded in the parent status evidence.

1. record the timestamp and sequence of the last observed event;
2. use the host wait/status operation at the configured checkpoint boundary;
3. if no result is available, perform one bounded recovery check or resume
   operation when supported;
4. classify the result as slow, blocked, failed, or lost only from evidence;
5. preserve the last confirmed evidence and report the exact limitation.

Do not emit redundant heartbeat noise when no observable state changed. Do not
claim native heartbeat automation or mark an agent lost solely because a wait
timed out.

## Tracing

Repository lifecycle hooks are an optional observation adapter, configured in `.codex/hooks.json`. They write metadata-only records under `.tmp/codex-harness/events/` and provide startup reminders. `SubagentStop` means the runtime stopped that child; it does not establish `TaskCompleted`, successful verification, or parent acknowledgement. Hooks do not receive all application correlation fields automatically: the parent maintains the mapping from host agent/session IDs to `parent_step_id` and `task_id` in its checkpoint.

After compaction, recover that checkpoint and validate current evidence before resuming. Hook logs are not a checkpoint, heartbeat service, approval gate, or OpenTelemetry exporter. Keep the message/status fallback when hooks are unavailable or untrusted; never infer availability from the presence of a configuration file.

When the host exposes OpenTelemetry hooks, create a parent workflow span and a
child span per delegated step, propagating `parent_step_id`, `task_id`, and
`update_seq` as attributes or baggage according to the host integration.

When tracing hooks are unavailable, retain those identifiers in the status
envelope and report `telemetry: unavailable`. Text correlation is a fallback;
it is not OpenTelemetry coverage.
