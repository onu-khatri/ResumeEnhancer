# Main Branch Merge Protection

The `PR merge gate` check runs the separate frontend and backend CI workflows for every pull request targeting `main`. It fails whenever either reusable workflow fails.

An organization or repository administrator must enforce it in GitHub. This repository file cannot configure hosted branch protection by itself.

1. Open `Settings` > `Rules` > `Rulesets` and create or update the ruleset targeting `main`.
2. Require a pull request before merging.
3. Enable `Require status checks to pass`. In its additional settings, add the `PR merge gate` status check and require the branch to be up to date.
4. Enable the rule for administrators as well if administrators must not bypass failed CI.

If `PR merge gate` is not offered yet, open or update a pull request targeting `main` so the workflow runs once, then return to the ruleset and add the check by name. After saving, open a pull request that intentionally fails frontend or backend CI and confirm the merge button remains disabled until the check succeeds.
