---
name: backend-security-coder
description: Implement and review secure backend code for ResumeEnhancer with OWASP-oriented practices around validation, authorization, data handling, logging, and abuse resistance. Use when Codex is changing APIs, persistence flows, auth-sensitive logic, or other backend attack surfaces.
---

# Backend Security Coder

Use this skill to apply practical backend security during implementation and review, not as an afterthought.

## Use this skill when

- backend work touches authentication, authorization, exports, admin actions, privacy, file handling, or AI usage limits
- an endpoint or repository flow crosses a trust boundary
- security-sensitive code needs a dedicated implementation pass

## Do not use this skill when

- the task has no meaningful backend trust or data-handling risk
- a general code-quality review is enough

## Security workflow

1. Identify the trust boundary and who is allowed to perform the action.
2. Validate request shape and business preconditions explicitly.
3. Enforce ownership and authorization close to the action.
4. Minimize data exposure in responses, logs, and exceptions.
5. Review abuse paths such as enumeration, replay, and unbounded resource use.
6. Verify the security behavior with targeted tests when practical.

## Security lenses

- broken access control and IDOR risk
- input validation and injection risk
- sensitive data exposure
- secret and token handling
- rate limiting and abuse resistance
- auditability and safe failure behavior

## ResumeEnhancer focus

- user-owned resume data
- admin or support-only operations
- file import or export handling
- AI-adjacent usage quotas, billing, or entitlement checks

## Output requirements

- identified trust boundaries
- blocking versus hardening issues
- concrete code-level mitigation steps
- verification notes