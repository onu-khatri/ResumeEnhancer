# AI Review Playbook

Use this note when combining human review judgment with AI-assisted triage.

## Goals

- use automation to widen coverage, not replace engineering judgment
- triage large diffs quickly before deep review
- focus human attention on security, contracts, architecture, and business behavior

## Practical flow

1. classify the change by size, language, and risk
2. use automated scanners or repository tests for fast signals
3. ask AI to reason about edge cases, architectural fit, and missing tests
4. verify AI findings against the real code before reporting them

## Good AI review prompts include

- the actual diff or changed files
- surrounding context needed to understand the call path
- the acceptance criteria or story intent
- explicit focus areas such as security, performance, or compatibility

## Do not trust AI review blindly

- verify every finding before surfacing it as a defect
- do not present speculative warnings as confirmed bugs
- prefer precise, reproducible findings over long generic commentary

## ResumeEnhancer focus

- contract drift between frontend and backend
- validator or mapper mismatches
- boundary leakage across Web, SL, and PL
- missing tests around permissions, error handling, and migrations