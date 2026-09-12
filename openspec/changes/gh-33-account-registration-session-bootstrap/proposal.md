## Why

Correct the Auth/Billing/Profiling model so Profiling remains the foundational access-profile module and billing-plan changes can optionally revise existing subscriber access.

## What Changes

- Make `BillingPlan` reference one Profiling `AccessProfile`.
- Remove access-profile ownership from `BillingSubscription`.
- Remove `UserEntitlement`; represent assignments through `UserAccessProfile` validity and source data.
- Add a seeded Profiling `AccessProfileSource` setup entity and store its ID on `UserAccessProfile` instead of a free-form source string.
- Replace free-form billing-plan currency, billing interval, and subscription status values with seeded setup-entity foreign keys.
- Replace free-form billing-account status values with a seeded Billing setup-entity foreign key.
- Resolve assignment source from the billing plan code supplied through the Profiling integration contract.
- Add an administrator-only opt-in cascade operation for active subscribers when a plan's access profile changes.
- Use narrow, durable, idempotent Billing-to-Profiling integration contracts.

## Capabilities

### New Capabilities
- `auth/account-registration-session-bootstrap`: Account registration and access-profile bootstrap behavior.

### Modified Capabilities
- `auth/account-registration-session-bootstrap`: Replace entitlement baseline with user access-profile assignment and add plan revision behavior.

## Impact

Billing and Profiling domain models, EF mappings, migration, registration orchestration, integration contracts, admin API, outbox processing, unit tests, and integration tests.
