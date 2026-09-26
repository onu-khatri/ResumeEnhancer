"""Offline regression tests for configuration validation and advisory hooks."""
from concurrent.futures import ThreadPoolExecutor
import importlib.util
import json
from pathlib import Path
import re
import shutil
import subprocess
import sys
import tempfile
import unittest
from unittest.mock import patch

ROOT = Path(__file__).resolve().parents[2]


def load(name, path):
    spec = importlib.util.spec_from_file_location(name, path)
    module = importlib.util.module_from_spec(spec)
    spec.loader.exec_module(module)
    return module


validator = load("validator", ROOT / ".codex/tools/validate_harness.py")
lifecycle = load("lifecycle", ROOT / ".codex/hooks/lifecycle.py")


class HarnessTests(unittest.TestCase):
    def setUp(self):
        self.temp = tempfile.TemporaryDirectory()
        self.addCleanup(self.temp.cleanup)
        self.root = Path(self.temp.name)

    def fixture(self):
        codex = self.root / ".codex"
        shutil.copytree(ROOT / ".codex/agents", codex / "agents")
        for relative in ("config.toml", "hooks.json", "hooks/lifecycle.py",
                         "skills/orchestration-agent-improvement/references/delegation-protocol.md"):
            dest = codex / relative
            dest.parent.mkdir(parents=True, exist_ok=True)
            shutil.copyfile(ROOT / ".codex" / relative, dest)
        (codex / "README.md").write_text("# Test guide\n", encoding="utf-8")
        return codex

    def test_repository_configuration(self):
        self.assertEqual(validator.validate(), 7)

    def test_skill_generator_bodies_match_current_authorities(self):
        for script in ("seed-project-skills.ps1", "sync-linked-skill-bodies.ps1", "complete-linked-skills.ps1"):
            text = (ROOT / ".codex/tools" / script).read_text(encoding="utf-8-sig")
            for skill in ("orchestration-agent-improvement", "quality-production-code-review"):
                pattern = r'(?ms)  "' + re.escape(skill) + r'" = @\x27\n(.*?)^\x27@'
                body = re.search(pattern, text)
                self.assertIsNotNone(body, (script, skill))
                expected = (ROOT / ".codex/skills" / skill / "SKILL.md").read_text(encoding="utf-8").strip()
                self.assertEqual(body[1].strip(), expected, (script, skill))

    def test_malformed_toml_rejected(self):
        codex = self.fixture()
        (codex / "config.toml").write_text("[broken", encoding="utf-8")
        with self.assertRaises(ValueError):
            validator.validate(self.root)

    def test_missing_role_rejected(self):
        codex = self.fixture()
        (codex / "agents/backend-implementer.toml").unlink()
        with self.assertRaisesRegex(ValueError, "Missing required agents"):
            validator.validate(self.root)

    def test_invalid_role_properties_rejected(self):
        codex = self.fixture()
        path = codex / "agents/code-reviewer.toml"
        original = path.read_text(encoding="utf-8")
        for old, new, error in (
            ('name = "code-reviewer"', 'name = "other"', "name must match"),
            ('model = "gpt-6-sol"', 'model = "unknown"', "model outside"),
            ('model_reasoning_effort = "medium"', 'model_reasoning_effort = "invalid"', "effort outside"),
            ('sandbox_mode = "read-only"', 'sandbox_mode = "workspace-write"', "read-only sandbox"),
            ('developer_instructions =', 'unused_instructions =', "missing developer_instructions"),
        ):
            with self.subTest(property=old):
                path.write_text(original.replace(old, new), encoding="utf-8")
                with self.assertRaisesRegex(ValueError, error):
                    validator.validate(self.root)

    def test_duplicate_hook_source_rejected(self):
        codex = self.fixture()
        with (codex / "config.toml").open("a", encoding="utf-8") as stream:
            stream.write("\n[hooks]\n")
        with self.assertRaisesRegex(ValueError, "duplicate hook sources"):
            validator.validate(self.root)

    def test_broken_readme_link_rejected(self):
        codex = self.fixture()
        (codex / "README.md").write_text("[missing](missing.md)", encoding="utf-8")
        with self.assertRaisesRegex(ValueError, "README link missing"):
            validator.validate(self.root)

    def test_start_events_return_context_without_control(self):
        for event in ("SessionStart", "SubagentStart"):
            result = lifecycle.observe({"hook_event_name": event}, self.root)
            self.assertEqual(result["hookSpecificOutput"]["hookEventName"], event)
            self.assertNotIn("decision", result)
            self.assertNotIn("continue", result)

    def test_logs_exclude_content_and_paths(self):
        payload = {"hook_event_name": "SubagentStart", "session_id": "session-1",
                   "agent_id": "agent-1", "prompt": "PRIVATE", "last_assistant_message": "PRIVATE",
                   "transcript_path": "PRIVATE", "cwd": "PRIVATE", "token": "PRIVATE"}
        lifecycle.observe(payload, self.root)
        record = json.loads(next((self.root / ".tmp/codex-harness/events").glob("*.json")).read_text())
        self.assertEqual(set(record), {"event", "observed_at", "session_id", "agent_id"})
        self.assertNotIn("PRIVATE", json.dumps(record))

    def test_stop_never_claims_success_or_requests_continuation(self):
        self.assertEqual(lifecycle.observe({"hook_event_name": "SubagentStop"}, self.root), {})

    def test_unknown_events_and_non_objects_are_ignored(self):
        for payload in ({"hook_event_name": "Unknown"}, {"hook_event_name": []}, [], None):
            self.assertEqual(lifecycle.observe(payload, self.root), {})
        self.assertFalse((self.root / ".tmp").exists())

    def test_concurrent_observations_do_not_overwrite(self):
        with ThreadPoolExecutor(max_workers=4) as pool:
            list(pool.map(lambda _: lifecycle.observe({"hook_event_name": "SubagentStop"}, self.root), range(16)))
        files = list((self.root / ".tmp/codex-harness/events").glob("*.json"))
        self.assertEqual(len(files), 16)
        for path in files:
            self.assertEqual(json.loads(path.read_text())["event"], "SubagentStop")

    def test_unwritable_log_does_not_block(self):
        with patch.object(Path, "mkdir", side_effect=PermissionError), patch("sys.stderr"):
            result = lifecycle.observe({"hook_event_name": "SubagentStart"}, self.root)
        self.assertIn("hookSpecificOutput", result)

    def test_bad_input_is_nonblocking_json(self):
        for raw in (b"not-json PRIVATE", b"\xff", b"x" * (lifecycle.MAX_INPUT + 1)):
            result = subprocess.run([sys.executable, "-B", str(ROOT / ".codex/hooks/lifecycle.py")],
                                    input=raw, capture_output=True, timeout=10)
            self.assertEqual(result.returncode, 0)
            self.assertEqual(json.loads(result.stdout), {})
            self.assertNotIn(b"PRIVATE", result.stderr)


if __name__ == "__main__":
    unittest.main(verbosity=2)
