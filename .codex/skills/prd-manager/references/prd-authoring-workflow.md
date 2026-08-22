# PRD Authoring Workflow

Use this workflow to create or update a ResumeEnhancer PRD from repository
artifacts.

## 1. Determine the source mode

Choose one primary mode before writing:

- `BR mode`: the request names a file in `Business-Requirements/`
- `Story mode`: the request names a file in `User-Stories/`
- `Update mode`: the request names an existing file in `prd/`

If the user is vague, inspect the named feature or topic and infer the narrowest
source set that can support the PRD. Do not scan the entire repository when one
BR or one story pack is enough.

### Decompose BR requirement clusters

For `BR mode`, treat the document as a container that can define more than one
business requirement. Group requirements by shared user outcome, scope boundary,
and delivery decision:

- Keep clusters in one PRD when they form one coherent product capability or
  domain outcome. Give each cluster distinct requirement identifiers and
  traceability entries.
- Split a cluster into its own PRD when it can be independently prioritized,
  delivered, owned, measured, or deferred without invalidating the other
  clusters.
- Do not split a tightly coupled workflow merely because it has many numbered
  requirements.

For split output, use `prd/<source-stem>--<capability>.PRD.md`. Each PRD must
state the exact BR sections it covers and identify related clusters that remain
out of scope. If the source does not clearly support a split, keep one coherent
PRD and record the possible boundary as an open question.

## 2. Read the minimum required sources

### BR mode

Read:

1. the target `*.BR.md`
2. any linked downstream BRs or story packs only if they materially affect the
   PRD scope or launch shape

Extract:

- product problem and why it matters
- target users or stakeholders
- requirement clusters and their shared or independent scope boundaries
- in-scope capabilities
- business and non-functional requirements
- risks, constraints, assumptions, and success measures

### Story mode

Read:

1. the target `*.US.md`
2. sibling `*.SI.md` if present
3. sibling `*.Research.md` if present
4. the upstream BR only when the story explicitly relies on it or the story
   cannot be framed correctly without it

Extract:

- story objective and value
- exact scope boundary
- acceptance criteria and business rules
- implementation implications that matter to product planning
- verification expectations and rollout risks

### Update mode

Read:

1. the existing PRD
2. the PRD's cited source files
3. any newly named BR or story files the user wants folded in

Compare the existing PRD against the latest source evidence and update only the
sections that drifted.

## 3. Choose the template depth

Default to the repository PRD template in
`assets/resumeenhancer-prd-template.md`.

Adapt depth based on the work:

- Use a lean shape for narrow changes, localized fixes, or a single story with a
  tight delivery boundary.
- Use a standard/comprehensive shape for most features and domain expansions.
- Use a metrics-heavy or working-backwards variant only when the user asks for
  it or the source material clearly warrants it.

## 4. Write for this repository

Use repository-relative links and evidence-shaped language.

Prefer these section patterns:

- `Executive Summary`
- `Problem And Opportunity`
- `Source Documents`
- `Users And Stakeholders`
- `Scope`
- `Requirements`
- `Success Signals`
- `UX And Workflow Notes`
- `Technical And Operational Implications`
- `Dependencies`
- `Risks And Mitigations`
- `Open Questions`
- `Traceability`

Keep sections only when they add signal. Omit empty sections instead of filling
them with boilerplate.

## 5. Rules for assumptions and evidence

- Never fabricate metrics, user quotes, research volume, support-ticket counts,
  or competitor claims.
- If a source document already classifies evidence as observed, documented,
  inferred, or recommended, preserve that distinction in the PRD where useful.
- When data is missing, use one of:
  - `Assumption`
  - `Unknown`
  - `Open question`
  - `Needs validation`

## 6. Rules for updates

When editing an existing PRD:

- retain the file path and filename unless the user asks to rename it
- keep sections that still reflect the source truth
- collapse duplicate material introduced by prior revisions
- update dates and source references together
- keep the PRD readable; do not turn it into a running log of every edit

## 7. Final self-review

Read [prd-review-checklist.md](prd-review-checklist.md) after drafting or
updating the PRD. Resolve every applicable issue before delivery:

- correct a requirement, scope boundary, source link, or contradiction
- label unsupported information as an `Assumption`, `Unknown`, or `Open
  question`
- retain a check as `Not applicable` only when the source and PRD scope make
  the item irrelevant

The review is a self-validation pass. It does not substitute for documented
stakeholder, legal, design, engineering, or launch approvals.

## 8. Quality bar

A finished PRD should let a product manager, designer, or engineer answer:

- what problem is being solved
- who it is for
- what is in and out of scope
- what must be true for launch
- what risks or dependencies still need decisions

If the PRD cannot answer those clearly from the cited sources, keep refining.
