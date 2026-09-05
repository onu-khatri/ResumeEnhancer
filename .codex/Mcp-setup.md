commands to set the personal access token (PAT) for GitHub

```powershell
[System.Environment]::SetEnvironmentVariable(
  "GITHUB_PAT_TOKEN",
  "github_pat",
  "User"
)

[System.Environment]::SetEnvironmentVariable(
  "GH_TOKEN",
  "github_pat",
  "User"
)
```