$ErrorActionPreference = "Stop"

$agentsDir = Join-Path $PSScriptRoot "..\agents"
$requiredAgents = @(
  "implementation-planner.toml",
  "backend-implementer.toml",
  "frontend-implementer.toml",
  "knowledge-researcher.toml",
  "security-auditor.toml",
  "code-reviewer.toml",
  "story-orchestrator.toml"
)

foreach ($name in $requiredAgents) {
  $path = Join-Path $agentsDir $name
  if (-not (Test-Path -LiteralPath $path)) {
    throw "Missing required custom agent: $name"
  }

  $content = Get-Content -LiteralPath $path -Raw
  if ($content -notmatch '(?m)^name\s*=\s*"[^"]+"') { throw "$name has no name field" }
  if ($content -notmatch '(?m)^description\s*=\s*"[^"]+"') { throw "$name has no description field" }
  if ($content -notmatch '(?m)^developer_instructions\s*=\s*"""') { throw "$name has no developer_instructions field" }
  if ($content -notmatch '(?m)^##\s+') { throw "$name has no structured instruction sections" }
}

$pins = Get-ChildItem -LiteralPath $agentsDir -Filter "*.toml" -File | Select-String -Pattern '(?m)^model\s*='
if ($pins) { throw "Explicit model pins remain in custom agents: $($pins -join '; ')" }

Write-Output "Validated $($requiredAgents.Count) structured, model-neutral custom agents."
