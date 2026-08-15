# Client Security Playbook

Use this note when the browser-side code handles untrusted content or sensitive state.

## Check for

- unsafe HTML rendering
- token or session leakage
- insecure redirects or user-controlled navigation
- excessive data exposure in client storage, UI, or telemetry

## Safe defaults

- prefer safe DOM APIs
- validate redirect targets
- keep sensitive state short-lived and intentional
- treat third-party scripts and widgets as risk surfaces