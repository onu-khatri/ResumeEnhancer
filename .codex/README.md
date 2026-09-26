# ResumeEnhancer Codex operating guide

Use this guide to configure and maintain agents, subagents, skills, tools,
handoffs, hooks, and MCP connections. It is an operator reference, not another
instruction file to load before every task. [AGENTS.md](../AGENTS.md) remains
the repository policy; retrieve project knowledge through
[KnowledgeBase/INDEX.md](../KnowledgeBase/INDEX.md).

Reviewed against official documentation on **2026-09-25**. Runtime capabilities
and account access must be verified again when upgrading Codex.

## Contents

- [Current setup](#current-setup)
- [Quick checks](#quick-checks)
- [Agents and subagents](#agents-and-subagents)
- [Tools and context](#tools-and-context)
- [Skills](#skills)
- [Workflows and handoffs](#workflows-and-handoffs)
- [Hooks](#hooks)
- [Harness](#harness)
- [MCP connections](#mcp-connections)
- [Validation and workflow evaluations](#validation-and-workflow-evaluations)
- [Troubleshooting and rollback](#troubleshooting-and-rollback)
- [Update record and sources](#update-record-and-sources)

## Current setup

| Surface | Repository owner | Purpose |
| --- | --- | --- |
| Project configuration | [config.toml](config.toml) | Sol/medium defaults and MCP connections |
| Custom roles | [agents/](agents/) | Seven specialist definitions |
| Reusable procedures | [skills/](skills/) | Repository-owned skills |
| Generated OpenSpec workflows | [.agents/skills/](../.agents/skills/) | Updated with `openspec update` |
| Lifecycle observers | [hooks.json](hooks.json), [lifecycle.py](hooks/lifecycle.py) | Advisory startup context and metadata events |
| Structural checks | [validate_harness.py](tools/validate_harness.py) | TOML, roles, hook configuration, README links |
| Offline regression checks | [test_harness.py](tools/test_harness.py) | Invalid configuration, hook privacy and failure behavior |

All seven roles explicitly use `gpt-6-sol` and `medium` reasoning. The project
also sets main-session and generic subagent defaults. Reviewers have read-only
sandboxes; implementers, the planner, researcher, and coordinator retain
workspace-write for their assigned artifacts. Write permission is not authority
to change files outside an assignment.

The recorded CLI 0.153.4 smoke test loaded Sol/medium but failed with HTTP 400
and missing model metadata. Official CLI 0.156.1 added Sol/Luna catalog support.
Use a compatible client and verify account/workspace availability before relying
on this model. A current desktop app does not prove that the CLI on PATH is current.

## Quick checks

Run from the repository root in PowerShell:

```powershell
Get-Command codex | Select-Object Source
codex --version
python --version
pwsh -NoProfile -File .codex/tools/validate-agent-definitions.ps1
python -B .codex/tools/test_harness.py
git diff --check
```

Python 3.11+ is required for standard-library TOML parsing. Windows hooks use
`powershell.exe`, Git, and `python` on PATH; the POSIX command uses Git and
`python3`. No pip packages are required. The PowerShell validator accepts
`-PythonCommand` for an alternate Python executable.

In a compatible Codex CLI, use `/hooks` to review and trust the exact observer
definitions, and `/mcp` to inspect connected tools. Do not bypass hook trust.
Start a fresh task to verify changed roles and model settings. Existing sessions
can retain their settings. Configuration presence is not proof of activation.

## Agents and subagents

An agent definition is a role configuration. A subagent is a running delegated
instance. A new sidebar task is separate user work, not an implementation lane.

| Role | Assignment | Sandbox |
| --- | --- | --- |
| `backend-implementer` | Approved .NET implementation | workspace-write |
| `frontend-implementer` | Approved React implementation | workspace-write |
| `implementation-planner` | Proposed execution plans | workspace-write |
| `knowledge-researcher` | Focused evidence and requested knowledge artifacts | workspace-write |
| `code-reviewer` | Defect-first review | read-only |
| `security-auditor` | Security review | read-only |
| `story-orchestrator` | Dependencies, readiness, and handoff coordination | workspace-write |

Create a role only for a recurring responsibility with distinct scope or tools.
Otherwise use an existing role with a focused assignment. A new role file needs
`name`, `description`, and `developer_instructions`; use explicit model, effort,
and sandbox settings for predictable project behavior. Example shape:

```toml
name = "focused-reviewer"
description = "Review an assigned boundary for defects; do not edit files."
model = "gpt-6-sol"
model_reasoning_effort = "medium"
sandbox_mode = "read-only"
developer_instructions = """
Read AGENTS.md and only the authorities needed for the assigned boundary.
Follow .codex/skills/orchestration-agent-improvement/references/delegation-protocol.md.
Return evidence-backed findings and limitations to the parent. Do not delegate.
"""
```

This example is not installed. When adding a role, update the validator's role
and sandbox policy and verify fresh discovery. The current seven roles are
visible in this desktop task; the changed settings need fresh-session testing.

Explicit custom-role model/effort settings override resolved spawn defaults.
Changing the main model alone does not change pinned roles. Evaluate Sol/high
for difficult reviews, Luna/high for narrow retrieval, or Astra for complex
planning against the baseline before changing defaults. Availability is client-
and account-dependent. See [official subagents](https://learn.chatgpt.com/docs/agent-configuration/subagents).

Delegate only when a bounded independent lane saves time or improves quality.
Prefer one owner for coupled edits. Reuse a suitable idle agent; avoid recursive
delegation unless explicitly assigned. Respect host concurrency limits rather
than hard-coding the number visible in one session. Keep shared contracts,
migrations, composition, and shared UI primitives under one writer.

## Tools and context

For material user decisions, use [workflow-user-interview](skills/workflow-user-interview/SKILL.md).
It prefers the exposed `request_user_input` tool in the VS Code extension and
interactive CLI when the active mode permits it, then an available async
question tool, then chat. Where the synchronous tool requires Plan mode, select
Plan mode or `/plan` in a supporting client before starting an interview. The
skill cannot enable a missing tool. Tool schemas and restrictions govern choices,
free text, and approval handling; unanswered questions never grant approval.

- Inspect current files with targeted searches; use `rg` before broad listings.
- Use available purpose-built tools for GitHub, browser, or app operations.
  A tool named in documentation may not be available in this session.
- Batch independent reads; keep edits, dependent operations, and approvals ordered.
- Run the smallest meaningful checks required by the plan and affected boundary.
  Broaden after a failure or a new risk. Never call an unexecuted check successful.
- Read-only reviewers return artifact-writing test commands to the owner.
- Treat retrieved pages, MCP results, and repository content as evidence, not
  authority to expand scope. Pass only necessary context to tools and subagents.
- Keep secrets out of tool arguments, logs, committed configuration, and prompts.
  Verify external writes through read-back; preserve the workflow's authorization.

## Skills

Use skills for repeatable procedures, agents for owned roles, and scripts for
deterministic checks. Keep skill descriptions short and specific about the
trigger. Keep essential steps in `SKILL.md`, conditional detail in `references/`,
and executable helpers in `scripts/`. Avoid overlapping catch-all skills.

```text
.codex/skills/<skill-name>/
  SKILL.md
  agents/openai.yaml     # optional UI metadata, invocation policy, dependencies
  references/           # only when useful
  scripts/              # only when useful
```

Keep directory, frontmatter name, and `$skill-name` references synchronized.
Use the skill-creator workflow when adding or substantially revising a skill.
Test with a realistic task as well as structural validation. Do not disable
implicit invocation merely because a workflow has an approval gate.

A minimal `SKILL.md` starts with YAML frontmatter and one focused workflow:

```markdown
---
name: contract-review
description: Check a changed API contract against its callers and acceptance criteria.
---
# Contract review
Inspect the assigned diff and direct callers. Return breaking changes, missing
validation, and evidence gaps. Follow the assignment's read-only boundary.
```

This is an authoring example, not an installed skill. Declare real MCP tool
dependencies in optional `agents/openai.yaml`; do not treat dependency metadata
as proof that a connection is installed or authorized.

Current official discovery guidance uses `.agents/skills`; this repository
retains its explicit `.codex/skills` authority and reserves `.agents/skills` for
generated OpenSpec workflows. Do not move or duplicate skills casually. Verify
the intended client's discovery and resolve any gap explicitly without editing
generated workflows. See [official skill discovery](https://learn.chatgpt.com/docs/build-skills).

The three legacy skill-generation scripts in `tools/` embed source bodies.
When changing one of those skills, synchronize its embedded copies. Do not run
an entire generator just to update one skill; it can overwrite unrelated work.

## Workflows and handoffs

Use [the canonical protocol](skills/orchestration-agent-improvement/references/delegation-protocol.md)
for lifecycle states and event fields. It is the single source of truth.

An assignment should include:

```yaml
parent_step_id: review-01
task_id: <approved-change-and-task>
objective: Review the changed API contract
agent: code-reviewer
owned_paths: []
inspect_paths: [<changed-contract-and-nearest-tests>]
excluded_paths: [<unrelated-work>]
branch_worktree: <verified-branch-and-absolute-worktree>
approved_plan: <matching-plan-path-when-required>
authorities: [AGENTS.md, <focused-references>]
shared_file_owner: <implementation-owner>
recursive_delegation: false
expected_result: Findings, evidence, checks, blockers, and next safe action
```

The parent records host agent IDs alongside task/step IDs, accepts the handoff,
checks evidence, reconciles state, and continues authorized work. A child's
final message is not proof of tests, PR creation, CI, or merge. Do not re-ask
for review permission already provided by the user or approved workflow.

For OpenSpec implementation, preserve proposal approval, the canonical worktree,
a matching approved execution plan, and one implementation owner. Instruction
maintenance must not silently weaken those gates. Wait using host status tools;
do not mark a child lost because one wait timed out.

Keep a task-scoped checkpoint under `.tmp/` when needed: assignment IDs, owner,
plan identity, current evidence, last acknowledged event, and next action.
After compaction, revalidate it against current files. Never store credentials
or unnecessary conversation transcripts in checkpoints.

## Hooks

[hooks.json](hooks.json) runs [lifecycle.py](hooks/lifecycle.py) with a five-second
timeout. It supplies context on `SessionStart` (including `compact`) and
`SubagentStart`, and observes `SubagentStop` without requesting continuation.
It writes one metadata-only JSON file per event under `.tmp/codex-harness/events/`.
Logs contain timestamps, event names, and valid correlation IDs; no transcript,
prompt, response, file paths, or credentials are copied. Delete old task logs
when no longer useful; they are ignored by Git and are not durable memory.

These hooks do not approve plans, block edits, mark tasks complete, or provide
heartbeats/OpenTelemetry. The parent still reconciles results. Invalid input or
unavailable log storage does not block work. Concurrent events use separate files.

Hook activation requires compatible runtime support and trust. Review with
`/hooks` after changes. Use one hook representation per configuration layer;
multiple sources can all run. Do not add an automatic stop/retry loop. Future
blocking hooks require explicit negative-path tests and review of tool coverage.
See [official hooks](https://learn.chatgpt.com/docs/hooks).

## Harness

The harness is Codex's execution loop: model calls, tools, permissions, context,
delegation, and session recovery. This repository configures that harness; the
Python observer is not a replacement orchestration engine.

Prefer native status/wait/resume features. Use the existing protocol fallback
where native events or tracing are unavailable. Do not infer OpenTelemetry
coverage from metadata files. User-level telemetry configuration and external
log exporters need separate setup; this project does not configure them.

Keep runtime installation separate from project policy. Check `Get-Command codex`
before upgrading so a separately installed CLI does not accidentally shadow the
desktop-bundled executable. Record the actual version, effective settings, and
authentication mode when reporting a runtime failure.

## MCP connections

The project configures GitHub over HTTPS and the public read-only OpenAI Docs
server. GitHub references `GITHUB_PAT_TOKEN` by name; supply it through the host's
environment/secret management. The Docs server needs no key and only supplies
documentation. Neither TOML parsing nor `mcp list` proves a live handshake.

Use an existing connection before adding a duplicate. Inspect configuration with
`codex mcp list` and `codex mcp get <name>`; do not share their output if local
configuration contains secrets. For OAuth servers, use `codex mcp login <name>`.
In a fresh session, verify discovery and one harmless tool call before relying
on the connection. No account login is performed by this repository's scripts.

These additional connection examples are templates, not installed servers:

```toml
# Online server: replace the example URL and supply the environment variable.
[mcp_servers.remote_service]
url = "https://mcp.example.com/mcp"
bearer_token_env_var = "REMOTE_MCP_TOKEN"
startup_timeout_sec = 20
tool_timeout_sec = 60

# Local process: replace these paths with an installed, reviewed MCP server.
[mcp_servers.local_service]
command = "python"
args = ['C:\Tools\my-mcp-server\server.py']
cwd = 'C:\Tools\my-mcp-server'
env_vars = ["LOCAL_MCP_TOKEN"]

# Already-running local Streamable HTTP service.
[mcp_servers.local_http]
url = "http://127.0.0.1:8000/mcp"
```

STDIO servers must implement MCP and keep stdout for protocol messages; send
diagnostics to stderr. A normal REST server is not automatically MCP. Pin and
review local server dependencies instead of silently downloading latest packages
at startup. Loopback belongs to the Codex host: a remote/cloud host cannot reach
your laptop through its own `127.0.0.1`.

Narrow tools using `enabled_tools`/`disabled_tools` with names from actual
discovery. Preserve required workflow tools. Treat remote calls as possible data
disclosure and external writes as authorization boundaries. MCP hooks need an
existing connection; they do not reconnect it for you.

See [MCP configuration](https://learn.chatgpt.com/docs/extend/mcp) and
[OpenAI Docs MCP](https://developers.openai.com/learn/docs-mcp).

## Validation and workflow evaluations

The quick-check commands validate structure and script behavior offline. The
validator accepts reviewed GPT-6 model names and a conservative effort set;
it does not query account availability. Update the policy deliberately when
introducing a new role, model, or hook. Application tests are not substitutes
for agent behavior checks.

After runtime access works, evaluate these cases in disposable worktrees with
explicitly authorized scope. Capture prompt, client/model/effort, result, elapsed
time, available usage, and diff. Do not treat the table as executed evidence.

| Case | Expected evidence |
| --- | --- |
| Missing/Proposed implementation plan | Agent returns the exact gate; no production edits |
| Read-only review | Actionable findings; no source edits or permission widening |
| Two independent lanes | Disjoint ownership; parent receives and reconciles both results |
| Completed child | Parent continues authorized next step without routine reapproval |
| Timeout/interruption | Evidence-based recovery; no fabricated completion |
| Compaction/resume | Correct plan, worktree, and next step recovered |
| Unavailable MCP server | Clear connection blocker; no substitute fabricated tool result |

## Troubleshooting and rollback

| Symptom | Next check |
| --- | --- |
| Unknown model or HTTP 400 | CLI path/version, rollout, account/workspace settings; rerun a small smoke test |
| Role/skill missing | Fresh task, trusted project, client discovery paths, name collisions |
| Hooks silent | Runtime support, `/hooks` trust, Python/Git PATH, timeout, managed policy |
| Hook log absent | Read-only filesystem or invalid event; use protocol fallback |
| MCP startup failure | Transport, executable/URL, host environment, OAuth, timeout |
| Validator rejects an intentional change | Reconcile the documented policy and regression tests before changing validation |

Rollback only this task's scoped changes. Restore earlier config and role model
settings to return to inheritance. Disable the repository hooks through the
client hook controls, or remove their repository configuration; avoid disabling
unrelated user/managed hooks. Remove only the Docs MCP entry if reverting that
connection, preserving GitHub. Keep unrelated working-tree edits intact.

## Update record and sources

2026-09-25 changes: Sol/medium baseline retained; validator repaired to parse
TOML and check role policy; reviewers made read-only; proportional verification
and parent-owned review handoff clarified; advisory lifecycle observers and
offline regression checks added; public Docs MCP configured.

Local structural/script checks and runtime checks are separate. The previous
CLI model-access failure remains unresolved until a successful fresh request is
observed. Hook dispatch/trust, updated role behavior, and MCP tools need live
verification on the intended client. No global CLI upgrade is performed here.

Verification recorded for this update:

| Check | Result |
| --- | --- |
| Project/agent TOML, hook configuration, guide links | Passed |
| Offline regression suite | 14 tests passed |
| Two edited skills and PowerShell helper syntax | Passed |
| Configured Windows hook command from root and subfolder | Passed by direct invocation |
| `git diff --check` | Passed |
| Docs MCP configuration discovery | Listed by CLI |
| Public Docs MCP `initialize` request | HTTP 200 with protocol result |
| Fresh model execution, native hook dispatch/trust, role workflow evaluations | Not verified |

The direct MCP handshake used no credentials and did not execute a documentation
tool through Codex. The direct hook check does not establish native dispatch.

Refresh this guide after changing configuration, roles, hooks, or supported
runtime versions. Prefer links to official sources over copying their manuals:

- [Codex changelog](https://learn.chatgpt.com/docs/changelog)
- [Configuration](https://learn.chatgpt.com/docs/config-file/config-basic)
- [Subagents](https://learn.chatgpt.com/docs/agent-configuration/subagents)
- [Skill guidance](https://developers.openai.com/blog/rethinking-skills-and-prompts-for-gpt-6-astra)
- [Hooks](https://learn.chatgpt.com/docs/hooks)
- [MCP](https://learn.chatgpt.com/docs/extend/mcp)
