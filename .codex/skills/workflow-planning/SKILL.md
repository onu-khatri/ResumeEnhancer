---
name: workflow-planning
description: Write clear, actionable plans for complex work across product, business, research, design, content, operations, and technical domains. Use when sequencing decisions, activities, dependencies, and validation will reduce execution risk.
---

# Plan Writing

Use this skill to turn an idea, request, decision, or approved outcome into a practical plan that another person or agent can understand, review, execute, and resume.

## Use this skill when

- the work has multiple activities, decisions, deliverables, or dependencies;
- sequencing or ownership will reduce risk or coordination cost;
- the plan needs explicit assumptions, evidence, milestones, or decision points;
- the work may pause and resume later; or
- a stakeholder needs a reviewable path from intent to outcome.

## Do not use this skill when

- the task is a single obvious action with no meaningful choices or dependencies; or
- a plan would add ceremony without improving clarity, safety, or execution.

## Planning workflow

1. **Clarify the outcome** — state what success means, who it serves, and the boundary of the work.
2. **Establish the current state** — summarize relevant facts, existing work, constraints, evidence, and known gaps. Do not invent missing information.
3. **Choose the work structure** — identify workstreams, deliverables, milestones, decision points, owners, and the order in which they should proceed.
4. **Define the phases** — write ordered, reviewable steps with concrete outputs and completion conditions. Keep independent work parallel only when ownership and dependencies permit it.
5. **Handle uncertainty** — distinguish confirmed facts, assumptions, recommendations, open questions, and blockers. Identify which decisions require approval or further research.
6. **Plan validation** — define how each important output, decision, or acceptance condition will be checked. Use domain-appropriate evidence rather than forcing software tests onto non-technical work.
7. **Define completion and follow-up** — state the final handoff, rollout or adoption actions, residual risks, and the next safe action.

## Plan structure

Use the sections that materially apply; do not add empty ceremony.

1. **Goal and success criteria** — desired outcome, audience, scope, and measurable or observable completion conditions.
2. **Current state and context** — relevant evidence, existing assets, constraints, assumptions, and unresolved gaps.
3. **Scope and boundaries** — included work, excluded work, and conditions that would require replanning.
4. **Workstreams and ownership** — deliverables, responsible people or roles, collaborators, and handoff points.
5. **Phases and actions** — ordered steps, dependencies, outputs, decision gates, and completion criteria.
6. **Risks, dependencies, and decisions** — impact, mitigation, external dependencies, approval needs, and fallback paths.
7. **Validation and evidence** — reviews, demonstrations, experiments, inspections, tests, metrics, or other appropriate checks.
8. **Rollout and completion** — launch, adoption, communication, maintenance, handoff, and remaining follow-up.

## Planning rules

- Match the plan to the domain and scale; use technical layers only for technical work.
- Prefer the smallest sequence that safely reaches the outcome.
- Make dependencies directional and explicit. Do not claim parallelism when work shares an unresolved dependency or ownership boundary.
- Keep assumptions visible and assign an action to validate material ones.
- Separate required work from optional recommendations and future improvements.
- Preserve user decisions and approval gates; do not silently resolve material ambiguity.
- Use concrete outputs and observable completion conditions instead of vague verbs such as “handle” or “improve.”
- Make the plan resumable: identify the current phase, completed outputs, blockers, and the next safe action.
- If the plan is based on external or changing information, record the source and date or identify what must be refreshed.
- Keep the plan proportional; omit sections that do not reduce execution risk.

## Definition of done

- The outcome and completion conditions are clear.
- Current-state evidence, assumptions, and unknowns are distinguished.
- Scope, ownership, sequencing, dependencies, and decision gates are explicit.
- Each phase has a concrete output and completion condition.
- Validation is appropriate to the domain and can be reported honestly.
- Risks, rollout or handoff actions, residual work, and one next safe action are stated.

## Review and approval handoff

When a plan requires formal approval, pass the completed plan to
`$plan-review-approval` for the combined review and approval gate.

Use this cycle:

```text
Plan draft
    ↓
Plan review and approval
    ├─ Approved → plan handoff
    ├─ RevisionRequired → planner corrects the plan and repeats the review
    ├─ Escalate → human resolves the decision, then review resumes
    └─ Rejected → stop or return to request clarification
```

- A high-confidence review with all criteria passing may be automatically
  approved.
- `RevisionRequired` returns actionable findings to this planning workflow.
- `Escalate` requires a human decision for architectural ambiguity, conflicting
  requirements, high-risk changes, or insufficient confidence.

These gates review plans created by this skill. They do not replace domain
implementation, OpenSpec proposal or implementation-plan, code-review,
verification, or delivery gates. Do not apply this skill as a substitute for
the separate OpenSpec implementation-planner approval contract.

