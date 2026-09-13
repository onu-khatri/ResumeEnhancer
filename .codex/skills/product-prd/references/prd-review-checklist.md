# PRD Review Checklist

Use this checklist after creating or updating a PRD and before presenting it as
ready for review. Check only items supported by the cited repository evidence.
For any applicable item without evidence, either improve the PRD from its
sources or record the gap as an `Assumption`, `Unknown`, or `Open question`.

## Grounding And Traceability

- [ ] The PRD is saved under the repository-root `prd/` folder.
- [ ] The title and filename identify the product change clearly.
- [ ] Every source document used is listed with a valid repository-relative
  path.
- [ ] The problem, users, scope, and requirements agree with those sources.
- [ ] No sample data, template text, fabricated metrics, unsupported user
  quotes, or invented approvals remain.
- [ ] Statements derived from incomplete evidence are labelled rather than
  presented as facts.

## Problem, Users, And Outcomes

- [ ] The problem describes a user or business pain rather than prematurely
  prescribing a solution.
- [ ] Intended users and stakeholders are identified when the source material
  names or implies them.
- [ ] The PRD explains why the work matters in terms supported by the sources.
- [ ] Goals or expected outcomes are concrete enough to review.
- [ ] Success signals are measurable where source metrics exist; otherwise the
  measurement gap is explicit.
- [ ] Non-goals or out-of-scope boundaries are stated.

## Scope And Requirements

- [ ] In-scope capabilities are distinct from out-of-scope work and future
  considerations.
- [ ] Functional requirements are specific, testable, and traceable to source
  evidence.
- [ ] Requirements use stable identifiers or equivalent clear labels when the
  PRD has multiple requirements.
- [ ] Priorities are shown when supplied by the source or needed to resolve a
  scope tradeoff; otherwise the prioritization gap is explicit.
- [ ] Key acceptance expectations or observable outcomes are captured where
  available.
- [ ] Relevant non-functional requirements, including accessibility,
  performance, privacy, or security, are included when the source establishes
  them.

## Feasibility, Risk, And Delivery

- [ ] Dependencies, constraints, and external decisions are visible.
- [ ] Technical implications remain product-level unless a source requires a
  specific technical constraint.
- [ ] Risks have proportionate mitigations or explicitly named follow-up
  decisions.
- [ ] Launch, rollout, support, compliance, and monitoring considerations are
  included only when relevant to the documented scope.
- [ ] Missing stakeholder reviews or approvals are recorded as pending, not
  marked complete.

## Clarity And Consistency

- [ ] Sections are organized for the chosen lean, standard, or comprehensive
  PRD depth.
- [ ] Terminology is consistent with the BR, story pack, and existing product
  language.
- [ ] The document is concise and free of implementation detail that does not
  change a product decision.
- [ ] Links, dates, identifiers, and Markdown structure are valid.
- [ ] An informed product manager, designer, and engineer can identify the
  problem, target users, scope, requirements, risks, and unresolved decisions.

## Final Decision

Before delivery, answer:

1. Can a reviewer trace every meaningful claim back to source evidence or an
   explicit assumption?
2. Could an implementation team distinguish required behavior from future work
   and unresolved decisions?
3. Does the PRD avoid presenting incomplete research, approvals, metrics, or
   rollout plans as established facts?

If any answer is no or unsure, revise the PRD before presenting it as ready for
review.
