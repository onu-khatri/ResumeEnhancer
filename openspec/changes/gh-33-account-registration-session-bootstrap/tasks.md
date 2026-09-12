## 1. Domain and ownership

- [x] Add required `BillingPlan.AccessProfileId` and `AccessProfile` navigation.
- [x] Add seeded Profiling `AccessProfileSource` and store `AccessProfileSourceId` on `UserAccessProfile`.
- [x] Add seeded Billing `Currency`, `BillingInterval`, `BillingSubscriptionStatus`, and `BillingAccountStatus` setup entities and replace corresponding string fields with required FK IDs and navigations.
- [x] Remove `AccessProfileId` and navigation from `BillingSubscription`.
- [x] Remove `UserEntitlement` and move validity/source/subscription traceability to `UserAccessProfile`.
- [x] Confirm Profiling has no project dependency on Billing.

## 2. Integration and application

- [x] Add Profiling access-profile/source validation, assignment, and revision snapshots/contracts; accept Billing plan code and resolve source ID in Profiling.
- [x] Update registration to assign the plan access profile through Profiling.
- [x] Add administrator plan access-profile update endpoint with explicit cascade option.
- [x] Add durable idempotent revision/outbox processing for all active subscriptions of the plan.

## 3. Persistence

- [x] Update EF mappings, indexes, seed data, and final migration.
- [x] Remove obsolete entitlement and subscription access-profile schema.

## 4. Verification

- [x] Add unit tests for assignment, expiry, disabling, validation, cascade, and idempotency.
- [x] Add integration tests for endpoint authorization, cascade enabled/disabled, retries, and unaffected subscriptions.
- [x] Run architecture reference scans, build, migrations, unit tests, integration tests, and coverage.

  Verification: Auth-scoped coverage using `test/auth.coverage.runsettings` measured Auth SL, PL, and Web only after excluding AM/DM and generated files: 92.87% line coverage (874/941) and 80.64% branch coverage (100/124). Unit tests: 313 passed. Integration tests: 27 passed.
