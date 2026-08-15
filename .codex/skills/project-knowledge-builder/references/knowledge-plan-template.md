# Knowledge Plan Template

Use this template for every `*.kb_plan.md` file. The plan is a reviewable scaffold, not the final knowledge artifact.

## File name

```text
KnowledgeBase/<topic-name>.kb_plan.md
```

## Frontmatter

```yaml
---
title: <short title>
topic_slug: <topic-name>
audience: <explicit named audience such as AI agent implementer, reviewer, and planner>
status: draft
plan_date: <YYYY-MM-DD>
depends_on_interview: true
approved_to_compose: false
---
```

## Body structure

```md
# <Title>

## Purpose
- <what knowledge this will produce>

## Interview Outcome
- Objective: <answer>
- Audience: <explicit named audience>
- Depth of knowledge: <answer>
- Expected structure: <answer>
- Applicability: <answer>
- Primary use: <implementation | review | planning | onboarding>
- Required sections: <user-approved core sections>
- Additional approved sections: <extra sections discovered during investigation and approved by the user, or none>
- Evidence style: <snippet-first | file-level | line-level | other>
- Section self-sufficiency: <whether each kept section must contain enough code to stand on its own>
- Scope constraints: <answer>
- Open assumptions: <answer or none>

## Scope
- In scope:
  - <placeholder>
- Out of scope:
  - <placeholder>

## Planned Output Files
- `KnowledgeBase/<topic-name>.kb_plan.md`
- `KnowledgeBase/<topic-name>.pre-knowledge.md`
- `KnowledgeBase/<topic-name>.knowledge.md` after approval

## Finalization Note
- After saving `KnowledgeBase/<topic-name>.knowledge.md`, ask the user whether `KnowledgeBase/<topic-name>.kb_plan.md` and `KnowledgeBase/<topic-name>.pre-knowledge.md` should be deleted.
- Do not delete either file without explicit user approval.

## Planned Sections
### 1. <Section name>
- Goal:
  - <placeholder>
- Evidence to gather:
  - <placeholder>
- Expected symbols / files:
  - <placeholder>

### 2. <Section name>
- Goal:
  - <placeholder>
- Evidence to gather:
  - <placeholder>
- Expected symbols / files:
  - <placeholder>

## Evidence Collection Plan
- README / requirements:
  - <placeholder>
- backend:
  - <placeholder>
- frontend:
  - <placeholder>
- tests:
  - <placeholder>
 - evidence packaging:
   - prefer short embedded code snippets for high-value claims unless the user chose another style
   - prefer interface/type-name links over raw file inventories when references are needed

## Validation Plan
- Pass the sequential gates A–H from `references/knowledge-quality-gates.md` (Grounding → Specificity → Reproducibility → User Interview → Consistency → Boundary → Currency → Record).
- User approvals required:
  - user approved `*.kb_plan.md`
  - user answered Gate D interview questions
  - user approved `*.pre-knowledge.md`
- Companion checks: `references/artifact-validator-checklist.md` and `references/artifact-cross-examination.md`.

## Approval Gate
- User may edit this plan file directly before composition starts.
- Do not create `*.pre-knowledge.md` until the user explicitly approves this plan.
```
