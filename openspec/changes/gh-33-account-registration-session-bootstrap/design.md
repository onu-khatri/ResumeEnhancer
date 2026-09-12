## Context

`ProfilingModule` is foundational and must not depend on another business module. Billing may reference Profiling DM entities, including `AccessProfile` from `BillingPlan`, as defined by ADR-002.

## Decisions

1. `BillingPlan` owns a required `AccessProfileId` and navigation; `BillingSubscription` owns only user, plan, account, status, and dates.
2. Remove `UserEntitlement`. `UserAccessProfile` stores user assignment, `ValidTillUtc`, `Enabled`, `AccessProfileSourceId`, and optional `BillingSubscriptionId` traceability without a Billing navigation. `AccessProfileSource` is a seeded Profiling setup entity.
3. `Currency`, `BillingInterval`, `BillingSubscriptionStatus`, and `BillingAccountStatus` are seeded Billing setup entities. `BillingPlan`, `BillingSubscription`, and `BillingAccount` store their required IDs and navigations rather than free-form strings.
4. Profiling exposes snapshot-based access-profile/source validation, assignment, and revision contracts. Billing supplies the stable billing plan code; Profiling resolves the source ID.
5. Plan changes affect new subscriptions by default. An explicit admin cascade option revises all active subscriptions for the plan.
6. Billing persists the plan change and durable revision request atomically. Profiling processes revisions idempotently and reports retryable failures.

## Migration

Drop subscription access-profile and entitlement schema, add plan access-profile and user-assignment validity/source fields, rebuild indexes, and generate one final migration.

## Risks

Large cascades require batching, audit records, idempotency keys, and outbox retry. The endpoint should return an accepted revision status rather than wait for every user when the operation is asynchronous.
