# Artifact Validator Checklist

Granular YES/NO companion to `knowledge-quality-gates.md`. A NO is a defect to fix. Run it during composition and again after Gate D.

## Frontmatter & structure

- [ ] Has `title`, `intent`, `scope`, `audience`, `last_reviewed`, `status`, `source_plan`, `validation`.
- [ ] `status` is `draft` | `reviewed` | `stable` and matches what was actually done.
- [ ] `intent` is an action (not a noun); `scope` states in/out; `audience` names a role; `last_reviewed` is today.
- [ ] `source_plan` points to the real `*.kb_plan.md`.

## Evidence & grounding

- [ ] Claims are labeled `Observed` / `Inferred` / `Recommended`.
- [ ] `Observed` claims are grounded by snippets or clearly named source symbols; `Inferred` claims state the evidence they combine.
- [ ] `Recommended` is a minority; no claim lacks an evidence-map entry; gaps are stated explicitly.
- [ ] If the user asked for snippet-first evidence, the artifact actually uses short code snippets as the primary grounding style instead of falling back to path-heavy prose.

## Symbols & specificity

- [ ] Symbols/paths are real and match `references/repository-topography.md`; ≥3 project-specific identifiers; nothing guessed.

## User interview (Gate D)

- [ ] The `question` tool was used; every ambiguity, preference, option, and gap was surfaced.
- [ ] If missing from the prompt, `Objective`, `Audience`, `Depth of knowledge`, `Expected structure`, and `Applicability` were explicitly asked and recorded.
- [ ] A multi-selection list of the core template sections was presented to the user; any extra section beyond the core set was proposed separately and explicitly approved before inclusion.
- [ ] Evidence style preference was asked explicitly (for example: snippet-first vs. file/line references).
- [ ] Section self-sufficiency was asked explicitly when the user cared about how much embedded code to include.
- [ ] No unconfirmed assumption remains except recorded ones; `Clarifications` records Q&A; answers are reflected in the body.

## Verification & commands

- [ ] Runnable commands with correct paths (`dotnet build application\ResumeEnhancerApp.slnx`, `npm run check`); states what was verified this session vs. guidance only.

## Boundaries & anti-patterns

- [ ] ≥1 explicit boundary/anti-pattern as a rule; "do not" references the rule it protects; no hedges.

## Cross-references

- [ ] Every path/section resolves; product-doc links use real names; no `<TO CONFIRM>` or TODOs.

## Cold-start usability

- [ ] Purpose is clear from `intent`; workflows name a concrete start; no conversation-dependent context; acronyms defined at first use.
- [ ] Every kept major section contains enough concrete detail, usually snippets, that a cold-start agent can understand the main behavior without reopening the repository for first-pass comprehension.

## Validation record

- [ ] `validation` lists gates A–H PASS/FAIL, `assumptions`, and gaps.
- [ ] After saving the final `*.knowledge.md`, the user was asked whether to delete the intermediate `*.kb_plan.md` and `*.pre-knowledge.md` files.
