## 1. Architecture and contracts

- [x] Amend `ADR-001` for the approved `AuthModule` and `ProfilingModule` ownership boundary.
  Evidence: `KnowledgeBase/ADRs/ADR-001-backlog-driven-module-boundaries.md` names AuthModule ownership and ProfilingModule profile ownership.
- [x] Inventory current composition, persistence, configuration, and test seams; record the implementation decisions for token lifetimes, rotation, key management, queue/outbox, and idempotency.
  Evidence: `design.md` Implementation Decisions records 15-minute access tokens, 30-day rotation, shared HMAC key, durable outbox, and normalized-email retry handling.
- [x] Define versioned registration, refresh, resend, and bootstrap contracts with stable error/state codes and source allowlists.
  Evidence: Auth AM request/response records expose register, refresh, resend, bootstrap, stable `AUTH_*` codes, and `BootstrapResponse.ContractVersion = v1`; Auth Web build passed.

## 2. Auth and domain model

- [x] Add AuthModule domain models and ports for credentials, refresh sessions, consent records, audit events, hashing, token issuance, and side effects; keep user preferences and entitlements in ProfilingModule.
  Evidence: Auth owns authentication/session/consent/audit/outbox entities; `UserPreference` and `UserEntitlement` are in `ProfilingModuleDM`, while `AuthenticationIdentity.User` remains the ADR-002-permitted DM relationship; composition build passed.
- [x] Add explicit ProfilingModule integration contracts for user/profile/access-shape creation and default entitlement resolution.
  Evidence: `ProfilingModelSL/Integrations/IProfilingRegistrationService.cs` and `ProfilingRegistrationService.cs` provide the Auth-to-Profiling registration seam.
- [x] Add validation and throttling behavior for identity, password, consent, source, and retry inputs.
  Evidence: `RegisterRequestValidator` enforces trimmed source allowlist, identity/password/consent/idempotency rules; `InMemoryRegistrationThrottle` enforces bounded email/IP windows; Auth Web build passed.

## 3. Persistence and composition

- [x] Add EF mappings, unique normalized-email constraints, refresh-session indexes, consent-version persistence, audit persistence, and Profiling-owned user preference/entitlement relationships, including the subscription-to-access-profile grant relationship.
  Evidence: `ProfilingEntityConfigurations` maps one `UserEntitlement` per `UserId`/`BillingSubscriptionId`/`AccessProfileId`, with FKs to `User`, `BillingSubscription`, and `AccessProfile`; `BillingSubscription` has a direct `UserId` FK and no `Resume` relationship; the reconciled migration backfills legacy ownership and fails loudly for unmappable rows.
- [x] Add the migration and idempotent setup/configuration for consent versions, default plan/entitlements, signing settings, and queue dependencies.
  Evidence: migration history contains only `20260825214719_Initial` plus `20260911074301_AuthRegistrationSessionBootstrapFinal`; `AuthModuleSeeder` and `ProfilingModuleSeeder` provide repeatable consent, stable-role/capability, and starter-profile setup; Auth persistence build passed.
- [x] Wire AuthModule through `ModulesComposition` without host-to-internal bypasses.
  Evidence: `ModulesComposition/DependencyInjection.cs` and project references expose only Auth module composition entry points; migration and Auth project builds passed.

## 4. Application and HTTP behavior

- [x] Implement atomic registration orchestration and safe duplicate/concurrency handling.
  Evidence: `AuthRepository.AddAsync` commits user-linked auth, consent, preference, entitlement, outbox, and starter billing records in one unit-of-work transaction; normalized-email unique index and conflict mapping are present.
- [x] Implement bearer access/refresh issuance, database-backed refresh rotation/revocation, shared-instance validation, logout/revocation, and resend behavior.
  Evidence: `RegistrationService` issues 15-minute access and 30-day refresh credentials, conditionally claims refresh-session rotation in the database, revokes replayed families/logout families, and persists resend outbox requests; focused Auth unit and HTTP integration suites pass.
- [x] Implement subscription-to-access-profile-to-role/capability entitlement resolution with one Profiling-owned entitlement grant per subscription/access profile, stable seeded identifiers, and safe unknown-subscription fallback, using narrow BillingModule and ProfilingModule integration contracts rather than cross-module persistence access.
  Evidence: Billing PL returns `BillingSubscriptionSnapshot`; Profiling PL returns `AccessShapeSnapshot` from seeded `Role.Capability` values; Auth SL unions attached profile capabilities and falls back to the seeded starter profile without repository or `DbContext` access.
- [x] Implement source-aware bootstrap calculation, safe fallback routing, and partial-success response states.
  Evidence: `AuthModuleSL/Services/RegistrationService.cs` allowlists sources, falls back to homepage/templates, and returns verification state codes in `BootstrapResponse`.
- [x] Implement asynchronous verification/welcome side effects, retry/degraded handling, and secret-safe audit logging.
  Evidence: registration writes verification/welcome rows to the durable outbox; `AuthOutboxDispatcher` retries with bounded backoff, records secret-safe `side_effect_failed` audit metadata, and marks the row `side_effect_delivery_exhausted` at the five-attempt ceiling with `retryable: false`; resend creates a new verification row. Unit tests cover retry and terminal degraded behavior.

## 5. Verification and review

- [x] Add unit tests for normalization, validation, consent, error mapping, route fallback, token/session rules, and idempotency decisions.
  Evidence: Auth unit suite covers normalization, validation, duplicate error mapping, terms/privacy/marketing consent staging, route fallback, token/session rotation and replay, outbox degradation, hashing, throttling, repository transitions, entitlement fallback, handler delegation, idempotency-key length validation, persisted idempotency replay, and different-payload idempotency conflict. Auth registration tests passed 17/17.
- [x] Add focused entitlement tests for subscription/profile/role resolution and safe fallback behavior.
  Evidence: `test/ResumeEnhancer.Tests/Modules/AuthModule/EntitlementIdentifierTests.cs` verifies the five seeded identifiers without Auth-owned constants, and `ProfilingEntitlementOwnershipTests.cs` verifies multi-plan entitlement representation.
- [ ] Add integration tests for transaction atomicity, unique-email races, multi-instance refresh state, queue failure recovery, audit events, and HTTP serialization.
  Evidence: Auth HTTP integration tests use the ResumeModule setup-object pattern with a shared assembly host, per-case reset/reseed, `Theory`, and `MemberData`. The complete integration project passed 27/27, covering bootstrap route variants, registration success/validation/duplicate conflict, invalid refresh, logout, resend, refresh rotation/replay rejection, malformed JSON, and persisted atomic-baseline records for identity, session, consents, audits, outbox, preference, and entitlement. Repository tests cover database-backed outbox lease exclusion and lease expiry/reclaim; dedicated transaction-race, multi-instance refresh, and queue recovery scenarios remain pending. Harness requirements and the parallel-execution plan are recorded in `User-Stories/3.3 integration-test-multi-writer-harness.TODO.md`.
- [ ] Run backend security review and architecture review; resolve findings before PR readiness.
  Review evidence (2026-09-11): conditional database refresh-session rotation, database-backed outbox leases, and persisted idempotency replay/conflict handling address the previously identified race and replay findings, with focused unit, repository, and integration coverage. A formal security and architecture review is still pending, so this task is not marked complete.
- [ ] Run focused backend tests, full required backend tests, and solution build; record actual results.
  Coverage evidence (2026-09-11): exact command `dotnet test test\\ResumeEnhancer.Tests\\ResumeEnhancer.Tests.Unit.csproj --no-restore --settings test\\auth.coverage.runsettings --collect:"XPlat Code Coverage" --filter FullyQualifiedName~AuthModule`; tool `coverlet.collector` via XPlat Code Coverage; 32 tests passed. Measured scope is AuthModule SL, PL, and Web only; AM and DM are excluded by assembly-level `ExcludeFromCodeCoverage`. Result: line 92.92% (749/806), branch 83.69% (77/92), meeting the required >92% line and >80% branch thresholds. Added direct coverage for Auth endpoint mapping, consent seeding, EF relationship/index metadata, malformed password payloads, refresh/session failure paths, resend behavior, and outbox success/invalid-payload paths.
  Additional evidence (2026-09-11): full unit command `dotnet test test\\ResumeEnhancer.Tests\\ResumeEnhancer.Tests.Unit.csproj --no-restore` passed 298/298; complete integration command `dotnet test test\\IntegrationTest\\ResumeEnhancer.Tests.Integration.csproj --no-restore` passed 27/27; `openspec validate gh-33-account-registration-session-bootstrap --strict` passed. Solution command `dotnet build application\\ResumeEnhancerApp.slnx --no-restore` remains blocked by the missing JavaScript SDK/NuGet authorization issue.
- [ ] Verify each `AUTH-BE-001` acceptance criterion and update OpenSpec checkboxes/evidence.
