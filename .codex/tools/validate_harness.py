"""Validate repository Codex configuration; Python 3.11+, standard library only."""
import json
from pathlib import Path
import re
import sys
import tomllib

ROOT = Path(__file__).resolve().parents[2]
ROLES = {
    "backend-implementer", "frontend-implementer", "implementation-planner",
    "knowledge-researcher", "code-reviewer", "security-auditor", "story-orchestrator",
}
REVIEWERS = {"code-reviewer", "security-auditor"}
MODELS = {"gpt-6-sol", "gpt-6-luna", "gpt-6-astra"}
EFFORTS = {"low", "medium", "high", "xhigh", "max"}
HOOK_EVENTS = {"SessionStart", "SubagentStart", "SubagentStop"}


def require(condition, message):
    if not condition:
        raise ValueError(message)


def model_settings(model, effort, location):
    require(model in MODELS, f"{location}: model outside the reviewed GPT-6 policy")
    require(effort in EFFORTS, f"{location}: reasoning effort outside the reviewed policy")


def validate(root=ROOT):
    codex = root / ".codex"
    config = tomllib.loads((codex / "config.toml").read_text(encoding="utf-8"))
    model_settings(config.get("model"), config.get("model_reasoning_effort"), "project")
    defaults = config.get("agents", {})
    model_settings(defaults.get("default_subagent_model"),
                   defaults.get("default_subagent_reasoning_effort"), "subagent defaults")
    seen = set()
    files = sorted((codex / "agents").glob("*.toml"))
    for path in files:
        data = tomllib.loads(path.read_text(encoding="utf-8"))
        name = data.get("name")
        require(isinstance(name, str) and name == path.stem, f"{path.name}: name must match filename")
        require(name not in seen, f"{path.name}: duplicate agent name")
        seen.add(name)
        for key in ("description", "developer_instructions"):
            require(isinstance(data.get(key), str) and data[key].strip(), f"{path.name}: missing {key}")
        model_settings(data.get("model"), data.get("model_reasoning_effort"), path.name)
        expected = "read-only" if name in REVIEWERS else "workspace-write"
        require(data.get("sandbox_mode") == expected, f"{path.name}: expected {expected} sandbox")
        require("delegation-protocol.md" in data["developer_instructions"],
                f"{path.name}: missing canonical handoff reference")
    require(ROLES <= seen, f"Missing required agents: {sorted(ROLES - seen)}")
    require((codex / "skills/orchestration-agent-improvement/references/delegation-protocol.md").is_file(),
            "Canonical delegation protocol is missing")
    hooks = json.loads((codex / "hooks.json").read_text(encoding="utf-8"))["hooks"]
    require(set(hooks) == HOOK_EVENTS, "Unexpected or missing lifecycle hooks")
    require("hooks" not in config, "Use hooks.json only; duplicate hook sources would run twice")
    for event, groups in hooks.items():
        require(len(groups) == 1 and len(groups[0].get("hooks", [])) == 1,
                f"{event}: expected one observer")
        handler = groups[0]["hooks"][0]
        require(handler.get("type") == "command", f"{event}: expected command handler")
        for key in ("command", "commandWindows"):
            require(".codex/hooks/lifecycle.py" in handler.get(key, ""), f"{event}: missing {key} entrypoint")
        require(handler.get("timeout") == 5, f"{event}: expected bounded observer timeout")
    require((codex / "hooks/lifecycle.py").is_file(), "Lifecycle observer is missing")
    guide = codex / "README.md"
    for target in re.findall(r"\]\(([^)]+)\)", guide.read_text(encoding="utf-8")):
        if "://" in target or target.startswith("#"):
            continue
        require((guide.parent / target.split("#", 1)[0]).exists(), f"README link missing: {target}")
    return len(files)


if __name__ == "__main__":
    try:
        count = validate()
    except (ValueError, KeyError, TypeError, OSError) as exc:
        print(f"FAIL: {exc}", file=sys.stderr)
        sys.exit(1)
    print(f"PASS: project TOML, {count} agents, lifecycle hook configuration, and README links")
    print("Structural validation only; model access, discovery, hook trust, and MCP connectivity require runtime checks.")
