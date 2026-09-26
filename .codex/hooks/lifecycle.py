"""Advisory lifecycle observer. Never blocks, reads transcripts, or executes input."""
from datetime import datetime, timezone
import json
from pathlib import Path
import re
import sys
from uuid import uuid4

ROOT = Path(__file__).resolve().parents[2]
EVENTS = {"SessionStart", "SubagentStart", "SubagentStop"}
MAX_INPUT = 4 * 1024 * 1024
PROTOCOL = ".codex/skills/orchestration-agent-improvement/references/delegation-protocol.md"


def observe(payload, root=ROOT):
    if not isinstance(payload, dict):
        return {}
    event = payload.get("hook_event_name")
    if not isinstance(event, str) or event not in EVENTS:
        return {}
    record = {"event": event, "observed_at": datetime.now(timezone.utc).isoformat()}
    # Store only runtime correlation identifiers, never prompts, outputs, paths or credentials.
    for key in ("session_id", "turn_id", "agent_id"):
        value = payload.get(key)
        if isinstance(value, str) and re.fullmatch(r"[A-Za-z0-9_-]{1,128}", value):
            record[key] = value
    try:
        folder = root / ".tmp/codex-harness/events"
        folder.mkdir(parents=True, exist_ok=True)
        # Separate exclusive files avoid interleaving from concurrent parent/child hooks.
        with (folder / f"{uuid4()}.json").open("x", encoding="utf-8") as stream:
            json.dump(record, stream)
    except OSError:
        print("Codex lifecycle observation could not be saved; continuing without telemetry.", file=sys.stderr)
    if event == "SubagentStop":
        # Runtime termination is not success and must not cause an automatic continuation loop.
        return {}
    if event == "SubagentStart":
        context = (
            f"For this assigned lane, follow {PROTOCOL}. Validate the manifest, owned paths, "
            "and applicable approval before writing. Return evidence to the parent; "
            "do not recursively delegate unless the assignment explicitly permits it."
        )
    else:
        context = (
            "Use AGENTS.md as repository policy and KnowledgeBase/INDEX.md for selective retrieval. "
            "On resume or compaction, recover the current plan and ownership checkpoint before "
            "continuing; do not infer progress from lifecycle logs. .codex/README.md is the "
            "operating guide when configuring agents, skills, hooks, or MCP."
        )
    return {"hookSpecificOutput": {"hookEventName": event, "additionalContext": context}}


def main():
    try:
        raw = sys.stdin.buffer.read(MAX_INPUT + 1)
        if len(raw) > MAX_INPUT:
            raise ValueError("oversized event")
        result = observe(json.loads(raw.decode("utf-8-sig")))
    except (ValueError, UnicodeError, RecursionError):
        print("Codex lifecycle observer received an invalid event; continuing.", file=sys.stderr)
        result = {}
    print(json.dumps(result))


if __name__ == "__main__":
    main()
