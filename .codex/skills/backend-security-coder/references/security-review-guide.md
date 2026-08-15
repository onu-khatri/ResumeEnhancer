# Backend Security Review Guide (ResumeEnhancer)

Use this note when the code touches a trust boundary or a protected data path. It maps OWASP concerns to the actual patterns in this repository.

## Trust model

- `ResumeEndpointHeaders.ReadUserId` reads the ownership user id (`X-User-Id`); `ReadAuditUserId` reads the acting user id (`X-Audit-UserId`).
- Ownership is enforced in the repository by filtering on `resume.UserId == userId` (`GetAsync`, `DeleteAsync`, `ExistsAsync`), not by trusting the id in the route alone.

## Check for

### Broken access control / IDOR
- Does every read/mutate path that returns a specific `resumeId` filter by the caller's `userId`?
- Does bulk delete return separate `NotFoundIds` and `ForbiddenIds` instead of silently deleting or erroring ambiguously?
- Are audit-user and ownership-user headers kept distinct and never conflated?

### Input validation and injection
- Are user-controlled request fields covered by FluentValidation in `<ModuleName>ModuleWeb/Validation`?
- Are search/filter values applied through EF query composition (parameterized), never string-built SQL?
- Are paging, date ranges, and id lists validated before they reach the query?

### Sensitive data exposure
- Does the response DTO expose only what the contract allows (no navigation-graph over-sharing)?
- Are `Email`, `PhoneNumber`, and profile-derived personal data gated by the owner?
- Do logs and exception messages avoid echoing full resume content or personal data?

### Secret and token handling
- Are secrets read from configuration (`ConnectionStrings`, `CacheOptions`), never hardcoded?
- Is the audit pipeline (`SaveChangesAsync(IAudit, ...)`) used so user attribution is accurate without leaking credentials?

### Abuse resistance
- Are search and list endpoints paged with a page-size cap (`MaxPageSize`)?
- Is setup/seeding idempotent so it cannot be abused to duplicate or overwrite data?
- Are failure paths mapped through `ApiEndpointExecutor` (403 for `UnauthorizedAccessException`) rather than leaking stack traces?

## Mitigation mindset

- deny by default
- validate early at the API boundary
- expose the minimum necessary data
- make failures safe and observable