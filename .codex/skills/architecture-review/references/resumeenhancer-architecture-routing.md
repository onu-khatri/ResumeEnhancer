# ResumeEnhancer Architecture Routing

## Retrieval Order

1. Use `KnowledgeBase/INDEX.md`, when present, as a retrieval map rather than a fixed dependency.
2. Identify the decision area and retrieve the current project knowledge topic, ADR, or other authority whose scope matches it.
3. Verify the authority's status, scope, and effective date where available before proposing or reviewing a project-specific decision.
4. If authorities conflict or are incomplete, report the conflict and recommend the smallest requirement clarification or ADR. Do not invent a local exception.

## Authority Selection Pattern

| Decision area | Read | Boundary |
| --- | --- | --- |
| Backend business-module ownership or backlog-to-module mapping | The active module-boundary authority discovered from the index/ADR registry | This routing topic does not restate the module map. |
| Cross-module relationships, data loading, lookup, snapshot, or integration contracts | The active cross-component authority discovered from the index/ADR registry | This routing topic does not restate integration rules. |
| Endpoint, application flow, validation, mapping, or host composition | The active API/application authority discovered from the index | Do not duplicate API/application flow here. |
| EF Core model, repositories, transactions, setup data, migrations, or persistence tests | The active persistence authority and applicable ADRs discovered from the index | This topic contains no persistence implementation facts. |
| Generic architecture or implementation method | The matching generic skill/reference | Retrieve selectively; do not load unrelated topics. |
| A durable new architecture decision | `architecture-adr` skill plus the affected authority | This topic does not prescribe ADR content. |

## Boundaries

- `AGENTS.md` requires index-first selective retrieval before planning, implementing, reviewing, or documenting. It does not authorize a full knowledge-base read by default.
- Existing accepted project authorities remain authoritative for their stated scope. Skills and knowledge topics may route to them but must not copy their rules.
- API and persistence knowledge remains authoritative for its stated scope. Architecture guidance identifies a decision area and then hands off to that authority for project facts.
- When an authority is proposed rather than accepted, report that status if it affects the recommendation.
