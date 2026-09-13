# Agent Optimization Playbook

Use this playbook when the orchestration itself needs redesign rather than another prompt tweak.

## Baseline signals

- repeated user corrections
- duplicated work across delegated agents
- long prompts with weak task ownership
- synthesis steps that re-open work instead of closing it

## Improvement sequence

1. capture representative successful and failed runs
2. classify failures by prompt clarity, context loss, tool misuse, sequencing, or approval timing
3. simplify responsibilities before adding more delegation
4. test the revised workflow on realistic repository tasks

## Practical tactics

- keep one owner for final synthesis
- keep shared-contract work out of unsafe parallel lanes
- insert approval gates before branching, PR creation, or destructive actions
- prefer small reusable roles over giant “do everything” subagents