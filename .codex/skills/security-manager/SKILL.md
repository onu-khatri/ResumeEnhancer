---
name: security-manager
description: Apply OWASP-oriented secure design and secure coding guidance to ResumeEnhancer across frontend, backend, authentication, persistence, and AI-adjacent flows. Use when Codex needs to assess, implement, or review security-sensitive changes in this project.
---

# Security Manager

Use this skill when a task touches authorization, data protection, input validation, file handling, privacy, secrets, payment-adjacent flows, or any feature that could introduce abuse or trust issues.

## Use this skill when

- a change crosses a trust boundary or touches user-owned resume data
- authentication, authorization, or entitlement logic is involved
- data exposure, secrets, logging, or abuse resistance must be assessed

## Do not use this skill when

- the change has no meaningful security surface
- a general code review is sufficient without security depth

## Security workflow

1. Identify the trust boundary and who is allowed to perform each action.
2. Enforce ownership and authorization close to the action, not only at the perimeter.
3. Validate input at the API boundary and encode output on render.
4. Minimize data exposure in responses, logs, exceptions, and client storage.
5. Review abuse paths: enumeration, IDOR, replay, quota bypass, and unbounded resource use.
6. Verify security behavior with targeted tests when practical.

## Security review lens

- authentication and authorization
- input validation and output encoding
- data exposure and privacy
- persistence safety and injection risk
- logging and secret handling
- rate limiting and abuse controls

## ResumeEnhancer focus

- user-owned resume data and sharing boundaries
- admin or support-only operations
- file import and export handling
- AI-adjacent usage quotas, billing, or entitlement checks

## Output requirements

- identified trust boundaries
- blocking versus hardening issues
- concrete mitigation steps
- verification notes