# Evidence And Discovery

Use this reference when evidence must be converted into a requirement, a research gap prevents planning, or a story needs a success measure. It supports story authoring; `$deep-research` remains responsible for substantial repository or external research.

## Evidence To Requirement

Keep these levels distinct:

| Level | Meaning | Where it belongs |
| --- | --- | --- |
| Observation | A directly supported fact, behavior, quote, metric, or repository condition. | `.Research.md` |
| Pattern | A repeated or corroborated observation. | `.Research.md` |
| Insight | An explanation of why the pattern matters. | `.Research.md` and story description |
| Requirement | An approved outcome or constraint the product must satisfy. | `.US.md` |
| Implementation choice | A proposed technical or UX mechanism. | `.SI.md`, unless architecturally mandated |

Express the reasoning chain as `evidence -> user problem or opportunity -> desired outcome -> requirement -> observable validation`. Write an insight as `observation + pattern + implication`; cite the observation and qualify the pattern when sample size, recency, or source quality is limited.

## Research Gaps

Choose research only when an unanswered question would change the slice, priority, or acceptance criteria.

| Question | Suitable evidence |
| --- | --- |
| What users do and why | Past-behavior interviews, usability observation, support cases |
| How frequent or widespread a behavior is | Analytics, surveys, support volumes |
| Whether an interface is understandable | Usability test with a representative task |
| Whether a market or standard changed | Dated authoritative external source |

For interviews, ask for a specific recent example: what triggered the task, what the person did, alternatives or workarounds, constraints, and outcome. Avoid leading or hypothetical questions such as "Would you use this feature?" Record observations and interpretations separately.

For a research/spike story, define the decision it will unblock, research questions and sources, a team-agreed boundary, the reviewable deliverable, and completion criteria based on evidence rather than a predetermined answer.

## Scope And Measures

Every feature story needs explicit in-scope and out-of-scope boundaries. Record whether exclusions are deferred or permanently excluded only when known.

When a success metric is relevant, capture its definition, source, owner, baseline, target, and measurement window. Do not invent a target. A missing metric is an open decision, not a placeholder number.
