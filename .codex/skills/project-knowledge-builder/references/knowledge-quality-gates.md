# Knowledge Quality Gates

Eight sequential steps every artifact must pass, in order, to reach `reviewed`/`stable`. Each gate must PASS before the next begins. A FAIL blocks progress: fix and re-run, or keep `status: draft` and record the failing gate.

Gates A–C run during investigation/composition. Gate D interrupts for a mandatory user interview. Gates E–H are final checks.

## Gate A — Grounding

Every operational claim is `Observed` or `Inferred` from named files; `Recommended` is flagged and secondary. Prefer short code snippets for high-value claims so the artifact carries its own proof. FAIL if any "how the repo works" claim lacks evidence or is generic .NET/React advice posing as repo fact.

Check: pick five random claims and confirm each has an evidence-map entry.

## Gate B — Specificity

The artifact names this repo's actual modules, symbols, paths, and conventions. FAIL if it reads like a transferable tutorial with the project name swapped in.

Check: at least three project-specific identifiers appear (a module name, a symbol, a real path).

## Gate C — Reproducibility

A cold-start agent can follow every step with zero extra context. FAIL if steps say "the usual flow" without concrete symbols, commands, or enough embedded code for first-pass comprehension.

Check: the verification section has runnable commands (`dotnet build`, `dotnet test`, `npm run check`), not prose.

---

## Gate D — User Interview (do not assume)

PAUSE and interview the **user** with the `question` tool before proceeding. Resolve every:

- **Objective** — if the prompt does not explicitly state the artifact's objective, ask for it.
- **Audience** — if the prompt does not explicitly state the target audience, ask for it.
- **Depth of knowledge** — if the prompt does not explicitly state the desired depth, ask for it.
- **Expected structure** — if the prompt does not explicitly state the preferred structure, ask for it.
- **Applicability** — if the prompt does not explicitly state when or where this knowledge should be used, ask for it.
- **Ambiguity** — hedging language ("typically", "usually", "might") → ask for the definitive behavior.
- **Preference** — depth, format, scope, or primary path.
- **Evidence style** — ask whether the user wants code-snippet-first grounding, file/line references, or another explicit style.
- **Option** — alternate codebase paths; ask which to document as primary.
- **Required sections** — present the core section set from `references/knowledge-artifact-template.md` and let the user choose which of those to include. If investigation reveals a clearly useful extra section, propose it explicitly and ask for approval before adding it.
- **Section density** — confirm whether each included section should be self-sufficient with enough code snippets that the agent does not need to inspect the repo for basic understanding.
- **Gap** — confirm whether to record a known gap or investigate further.

Ask concrete questions with 2–5 evidence-based options. If `Objective`, `Audience`, `Depth of knowledge`, `Expected structure`, or `Applicability` are missing from the prompt, they must be asked before proceeding and must not be assumed. For section selection, use `multiple: true` with the core sections first. If you discovered extra sections that would materially improve the artifact, present them separately with a short rationale and ask for explicit approval before adding them. Record every question, answer, and confirmed `assumption` in the draft's `Clarifications` section. Unanswered items become recorded `assumptions`, never silent guesses.

Proceed to Gate E only after the user responds.

---

## Gate E — Internal Consistency

Frontmatter, cross-references, body, and Gate D clarifications agree; every referenced file/section exists; no contradictory statements. FAIL on broken refs, frontmatter that mismatches the body, or an evidence style in the artifact that contradicts the user's chosen preference.

## Gate F — Boundary Discipline

At least one explicit boundary, anti-pattern, or "do not" rule is stated. Each kept section must also be substantively useful on its own; FAIL if the artifact is only happy-path how-to or if major sections are too thin to stand alone.

Check: a `Pitfalls`, `Boundaries`, `Anti-patterns`, or `Do not` section with concrete rules.

## Gate G — Currency

`last_reviewed` is a real date; high-risk claims were re-checked against current code this session; staleness is recorded. FAIL if undated or copied from memory unverified.

## Gate H — Validation Record

The `validation` block lists A–H with PASS/FAIL, user-interview outcome, and known gaps. FAIL if missing or not actually run.

After the final `*.knowledge.md` file is saved, the agent must ask the user whether to delete the intermediate `*.kb_plan.md` and `*.pre-knowledge.md` files. Keeping or deleting those files is a user decision, not an agent assumption.
