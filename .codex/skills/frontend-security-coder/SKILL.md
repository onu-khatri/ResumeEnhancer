---
name: frontend-security-coder
description: Implement and review secure frontend behavior in ResumeEnhancer with attention to auth flow, data exposure, user input handling, DOM safety, redirects, and safe API interaction. Use when Codex is changing client-side behavior with security or privacy implications.
---

# Frontend Security Coder

Use this skill for client-side security work that should prevent real browser-side vulnerabilities instead of relying on backend protection alone.

## Use this skill when

- the UI handles authentication, tokens, redirects, user-generated content, or sensitive data
- a component or page renders untrusted input
- a frontend change needs a dedicated security-focused implementation or review pass

## Do not use this skill when

- the task has no meaningful client-side trust boundary
- ordinary frontend guidance is sufficient

## Security workflow

1. Identify trusted versus untrusted content and where it enters the UI.
2. Prefer safe DOM patterns and avoid unsafe rendering shortcuts.
3. Validate redirect targets, route parameters, and user-controlled links.
4. Keep authentication and session behavior aligned with server expectations.
5. Review error states, analytics, and logs for accidental information leakage.

## Security lenses

- XSS and unsafe HTML rendering
- open redirect or navigation abuse
- token and session handling
- sensitive state exposure in UI or storage
- third-party script or widget risk
- privacy and consent-aware telemetry

## ResumeEnhancer focus

- resume content rendered back to the user
- auth, account, or entitlement screens
- export, upload, or rich-text flows
- frontend handling of backend security assumptions

## Output requirements

- identified client-side threat areas
- blocking versus hardening issues
- concrete implementation guidance
- verification notes