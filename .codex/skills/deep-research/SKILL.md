---
name: deep-research
description: Perform decision-focused, evidence-based research for ResumeEnhancer across requirements, user needs, code, tests, and external sources. Use when significant ambiguity or cross-cutting impact requires reliable findings before implementation, architecture, or planning.
---

# Deep Research

Use this skill when a shallow code read would create avoidable risk and the answer needs reliable findings from multiple sources. Its output is a research brief that supports a decision; it does not create a user-story pack, PRD, OpenSpec change, or implementation plan.

## Use this skill when

- the question spans requirements, code, tests, and current external information
- implementation or planning depends on a reliable understanding first
- there is meaningful ambiguity or cross-cutting impact

## Do not use this skill when

- a short local inspection can answer the question confidently
- current external data is unnecessary
- the request is only to author or revise a story pack; use `user-story-creator`
- the request is to coordinate approved story delivery; use `us-kickoff`

## Research workflow

1. Frame the decision: state the question, affected audience or system, decision owner when known, scope boundary, and what evidence could change the outcome. Use `$user-interview` if this cannot be established from the request and repository.
2. Plan the evidence: start with local sources, choose the smallest suitable method for each unanswered question, and record assumptions that must be challenged rather than confirmed.
3. Collect evidence with provenance: retain source, date or version, direct observation, limitations, and relevance to the decision. Separate observed facts, quoted material, and data from interpretations.
4. Synthesize: extract atomic observations, group corroborating or conflicting evidence into themes, and write insights as `observation + pattern + implication`. Seek disconfirming evidence for high-impact conclusions.
5. Deliver a decision-ready brief: present prioritized findings, confidence and limitations, open questions, options or implications, and the recommended next action. Route downstream work to the skill that owns it.

Read [research-design.md](references/research-design.md) when selecting a research method, defining a research question, planning user research, or evaluating bias and evidence quality. Read [evidence-synthesis.md](references/evidence-synthesis.md) when analyzing multiple sources, qualitative evidence, contradictions, or prioritization.

## Evidence order

1. repository facts: requirements, stories, code, tests, architecture notes, and executable behavior
2. first-party product evidence: support cases, analytics, research recordings, or user-provided data
3. authoritative current external documentation, standards, or primary sources
4. secondary analysis only when it adds value and its limitations are explicit

Use the lowest sufficient evidence tier; external research is not a substitute for inspecting the repository. Do not invent user quotes, research findings, metrics, baselines, targets, or sources.

## Output requirements

- decision question and scope boundary
- findings grouped by theme, each linked to evidence and confidence
- evidence summary, including material contradictions and limitations
- open questions, assumptions, and gaps that could change the conclusion
- implications, options, and a recommended next action

## Handoff

- Research that becomes a user-story pack: `$user-story-creator`.
- Approved story sequencing and execution: `$us-kickoff`.
- Architecture decision record: `$architecture-decision-records`.
- Durable repository knowledge: `$project-knowledge-builder`.
