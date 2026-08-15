---
name: deep-research
description: Perform deep, evidence-based research for ResumeEnhancer across product requirements, user stories, code, tests, and external sources when needed. Use when Codex needs a thorough answer before implementation, architecture, or planning decisions.
---

# Deep Research

Use this skill when a shallow code read would create avoidable risk and the answer needs evidence from multiple sources.

## Use this skill when

- the question spans requirements, code, tests, and current external information
- implementation or planning depends on a reliable understanding first
- there is meaningful ambiguity or cross-cutting impact

## Do not use this skill when

- a short local inspection can answer the question confidently
- current external data is unnecessary

## Research workflow

1. Start with local sources: requirements, stories, code, tests, and architecture notes.
2. Separate observed facts from inferred conclusions.
3. Use external sources only when the topic is current, user-requested, or not fully represented locally.
4. Synthesize around the actual decision the user must make.

## Evidence order

1. repository facts
2. tests and executable behavior
3. current external documentation or standards
4. recommendations grounded in the first three

## Output requirements

- findings grouped by theme
- evidence summary
- open questions
- implications for design, implementation, or rollout