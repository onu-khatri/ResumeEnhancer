# Business Requirement Alignment

Use this reference when a requested user-story capability is absent, ambiguous, or misaligned in `Business-Requirements/`. The business requirement remains the source of product intent; a story derives an implementation slice from it.

## Gap Assessment

Before proposing a BR edit, search all relevant BRs and classify the request:

| Result | Action |
| --- | --- |
| Already covered | Link the existing requirement; do not duplicate it. |
| Covered but ambiguous | Propose a clarifying revision in the owning BR. |
| Missing product requirement | Propose an addition to the most specific authoritative BR. |
| Implementation-only detail | Keep it in `.SI.md` or implementation planning, not a BR. |
| Unvalidated proposal | Record it as an open decision or research gap; do not state it as an approved requirement. |

## Ownership

Prefer the narrowest BR that owns the capability. Use `resume-platform-foundation.BR.md` only for platform-wide outcomes or cross-domain principles. Use a domain BR for capability-specific behavior, rules, and constraints. Do not add the platform's normative requirements to competitor/reference-analysis BRs; those documents preserve evidence and may support a proposed requirement, but do not own it.

If no existing domain BR is a clear owner, identify that ambiguity in the plan and request a decision before editing. Do not spread one requirement across several BRs without a primary owner.

## Approved Change Shape

After the user approves the authoring plan, add or revise the requirement in the selected BR using its established headings and writing style. Include the business need or intent, the requirement, relevant boundaries, and an open decision when detail remains unresolved. Update existing traceability or "next documents" sections when the document contains them.

Then link each affected `.US.md` back to the BR and keep the story scoped to the approved delivery slice. A BR change that materially alters story scope, dependencies, or acceptance criteria requires a revised plan and approval before modifying the story pack.
