param([string]$PythonCommand = "python")
$ErrorActionPreference = "Stop"
& $PythonCommand (Join-Path $PSScriptRoot "validate_harness.py")
if ($LASTEXITCODE -ne 0) { throw "Codex harness validation failed (exit $LASTEXITCODE)." }
