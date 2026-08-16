---
title: Backlog-Driven Business Module Boundaries
status: proposed
date: 2026-08-16
---

# Problem And Constraints

ResumeEnhancer currently has three backend business modules under `application/Modules`:

- `IdentityModule`
- `ProfileModule`
- `ResumeModule`

The current user-story backlog defines a much broader product surface. The repository needs a durable architectural decision that explains which backlog areas should become standalone backend business modules, which areas should remain capabilities inside existing modules, and which areas are better treated as shared application or infrastructure concerns.

This ADR also needs to clarify the split between authentication runtime concerns and user or authorization-profile concerns so those responsibilities do not collapse into one module boundary.

The decision must preserve the repository rules already documented in `AGENTS.md` and `README.md`:

- the host enters modules through `application/WebSolution/ModulesComposition`
- HTTP concerns stay in `*.Web`
- contracts stay in `*.AM`
- use-case orchestration stays in `*.SL`
- domain entities stay in `*.DM`
- persistence adapters stay in `*.PL`

The decision also needs to stay grounded in the existing story packs rather than inventing a different product decomposition.

# Decision Drivers

- Maintainability: avoid turning one module into an unbounded product grab-bag.
- Delivery clarity: map story ownership to explicit backend module seams.
- Boundary safety: keep admin, billing, trust, and document workflows isolated where policies differ.
- Reuse: avoid creating modules for concerns that are primarily frontend-shell, shared-workspace, or cross-cutting infrastructure behavior.
- Incremental adoption: allow the current three-module codebase to grow without a full rewrite.

# Considered Options

## Option 1: Keep expanding only the existing modules

Place most future behavior inside `IdentityModule`, `ProfileModule`, and `ResumeModule`, with folders or slices handling the new backlog areas.

### Benefits

- Fastest short-term implementation path.
- Lowest initial composition overhead.
- Minimal project and DI registration churn.

### Costs

- `ResumeModule` would become a catch-all for documents, analysis, exports, templates, job tracking, and review flows.
- `IdentityModule` and `ProfileModule` would absorb unrelated admin, privacy, support, notification, and authorization-profile responsibilities.
- Policy-sensitive areas such as billing, admin access, moderation, and audit would be harder to reason about and test independently.

## Option 2: Create a standalone backend module for every story-tagged module

Mirror every `**Module:**` label from the backlog as a backend business module.

### Benefits

- Strong traceability from story packs to code structure.
- Small, explicit module ownership.
- Easy to reason about what code belongs where.

### Costs

- Over-fragments the backend for concerns that are not true business domains.
- Creates module overhead for frontend-foundation, dashboard shell, builder customization, and similar capability groupings.
- Increases composition and dependency-management cost before there is enough backend complexity to justify it.

## Option 3: Introduce standalone business modules only for true domain or policy boundaries, and keep shell or cross-cutting capabilities inside existing modules or shared layers

Use the backlog as the source of candidate boundaries, but collapse module labels into backend business modules only where the stories indicate distinct data ownership, policy enforcement, or lifecycle behavior.

### Benefits

- Preserves architectural clarity without creating needless module churn.
- Aligns module seams to persistence, authorization, compliance, and operational boundaries.
- Keeps shell/UI-centric story groups out of the backend module taxonomy.

### Costs

- Requires architectural judgment rather than a mechanical one-story-one-module rule.
- Some story labels will map to capabilities instead of modules, which future contributors must understand.

# Decision

Adopt Option 3.

ResumeEnhancer should continue treating `IdentityModule`, `ResumeModule`, and a profiling-focused user module as first-class business modules, and should add the following backlog-driven backend business modules as the product surface expands:

- `ProfilingModule`
- `TemplatesModule`
- `DiscoveryModule`
- `CoverLetterModule`
- `AiAssistanceModule`
- `AtsAnalysisModule`
- `ExportModule`
- `BillingModule`
- `ReviewSharingModule`
- `JobTrackingModule`
- `NotificationsModule`
- `PrivacyModule`
- `SupportModule`
- `ContentAdminModule`
- `TemplateAdminModule`
- `AdminModule`
- `FinanceOpsModule`
- `ModerationModule`
- `ObservabilityModule`
- `ConfigurationModule`

These proposed modules are justified by the backlog because they introduce one or more of the following:

- distinct persistence models
- distinct authorization or entitlement rules
- distinct external integrations
- distinct operational or compliance workflows
- distinct internal-operator versus end-user boundaries

`IdentityModule` is intentionally narrow. It should own only authentication and authenticated runtime concerns such as:

- authentication flows
- session issuance and validation
- cookies tied to authenticated session behavior
- authentication middleware
- request identity resolution

`IdentityModule` should not own user master data, roles, claims, access profiles, or user address data.

`ProfilingModule` is the owner of user and authorization-profile data. It should own:

- user records
- access profiles
- claims and roles
- user-address data such as billing and communication addresses
- broader user profile data and preferences needed by product flows

The current codebase still contains `ProfileModule`, but the target boundary described by this ADR is a profiling module. The implementation may keep the current project name temporarily or rename it later, but the ownership boundary should follow this ADR.

The following backlog module labels should not automatically become standalone backend business modules:

- `Frontend Foundation`
- `Dashboard & Workspace`
- `Builder Customization`
- `Builder And Workspace`

These areas should be treated as one of the following unless later implementation evidence proves otherwise:

- frontend-shell or feature-composition concerns
- capabilities inside `ResumeModule`, `ProfilingModule`, `TemplatesModule`, or `DiscoveryModule`
- shared application or infrastructure behavior

# Proposed Backlog-To-Module Mapping

## Existing first-class modules

- `Authentication & Onboarding` -> split across `IdentityModule` and `ProfilingModule`
- `Profile & Preferences` -> `ProfilingModule`
- `Resume Builder` -> `ResumeModule`

## New first-class business modules

- `User, access profiles, claims, roles, and user-address concerns` -> `ProfilingModule`
- `Public Discovery And Acquisition` -> `DiscoveryModule`
- `Templates & Import` -> `TemplatesModule`
- `Cover Letter` -> `CoverLetterModule`
- `AI Assistance` -> `AiAssistanceModule`
- `ATS Analysis & Job Matching` -> `AtsAnalysisModule`
- `Export & Billing` -> `ExportModule`
- `Billing And Entitlements` -> `BillingModule`
- `Subscription Management` -> `BillingModule`
- `Document Review And Handoff` -> `ReviewSharingModule`
- `Job Tracking` -> `JobTrackingModule`
- `Notifications` -> `NotificationsModule`
- `Privacy & Account Deletion` -> `PrivacyModule`
- `Support & Recovery` -> `SupportModule`
- `Content Administration` -> `ContentAdminModule`
- `Template Administration` -> `TemplateAdminModule`
- `Admin & RBAC` -> `AdminModule`
- `Finance Operations` -> `FinanceOpsModule`
- `Moderation & Abuse Prevention` -> `ModerationModule`
- `Analytics & Audit` -> `ObservabilityModule`
- `Feature Flags & System Settings` -> `ConfigurationModule`

## Capability groups that should not become backend modules by default

- `Frontend Foundation` -> shared frontend and application shell only
- `Dashboard & Workspace` -> client/workspace composition plus query surfaces owned by other modules
- `Builder Customization` -> capability inside `ResumeModule` and `TemplatesModule`
- `Builder And Workspace` -> localization and workspace behavior spanning client plus owning modules

# Identity And Profiling Boundary Rules

## `IdentityModule`

`IdentityModule` owns authentication-only concerns:

- authentication
- session handling
- cookies used for authenticated session behavior
- authentication middleware
- request identity resolution

It should not own:

- user master records
- roles
- claims
- access profiles
- billing addresses
- communication addresses
- broader user profile data

## `ProfilingModule`

`ProfilingModule` owns user and access-shape data:

- user records
- access profiles
- claims
- roles
- billing and communication addresses
- profile and preference data

It may collaborate with `IdentityModule`, but it should remain the source of truth for these records and relationships.

# Consequences

## Positive

- Future modules can be added with a consistent composition model through `ModulesComposition`.
- Billing, admin, moderation, privacy, and audit concerns gain clearer isolation.
- Backlog planning can reference explicit target modules instead of ambiguous shared ownership.
- Testing strategy can follow module boundaries more cleanly for unit, integration, and contract coverage.

## Negative

- The project count and composition surface will grow materially.
- Some flows will require intentional cross-module contracts, especially around identity, ownership, entitlements, and document references.
- Teams must resist creating modules for every UI grouping or minor capability.

## Neutral But Important

- This ADR does not require all listed modules to be created immediately.
- The ADR defines the target module map for planning and future implementation, not a mandate to scaffold every module up front.
- A later ADR may consolidate or split specific modules if code, scale, or delivery evidence proves the boundary wrong.

# Follow-Up Actions

1. Use this ADR as the default module map when turning approved story packs into implementation plans.
2. Introduce new modules incrementally, starting with the stories that require their own persistence and policy boundaries.
3. Create a follow-up ADR if `ExportModule` and `BillingModule` prove too coupled or too fragmented in implementation.
4. Create a follow-up ADR if `ContentAdminModule` and `TemplateAdminModule` should be merged into a single internal operations module.
5. Update `application/WebSolution/ModulesComposition` and module project conventions as each accepted module is introduced.

# Related Stories And Evidence

- [User-Stories/2.2 public-discovery-examples-backend.US.md](../../User-Stories/2.2%20public-discovery-examples-backend.US.md)
- [User-Stories/5.2 template-import-backend.US.md](../../User-Stories/5.2%20template-import-backend.US.md)
- [User-Stories/7.2 resume-builder-backend.US.md](../../User-Stories/7.2%20resume-builder-backend.US.md)
- [User-Stories/10.2 cover-letter-backend.US.md](../../User-Stories/10.2%20cover-letter-backend.US.md)
- [User-Stories/11.2 ai-assistance-backend.US.md](../../User-Stories/11.2%20ai-assistance-backend.US.md)
- [User-Stories/12.2 ats-job-match-backend.US.md](../../User-Stories/12.2%20ats-job-match-backend.US.md)
- [User-Stories/13.2 export-entitlements-backend.US.md](../../User-Stories/13.2%20export-entitlements-backend.US.md)
- [User-Stories/14.2 document-review-sharing-backend.US.md](../../User-Stories/14.2%20document-review-sharing-backend.US.md)
- [User-Stories/15.2 subscription-management-backend.US.md](../../User-Stories/15.2%20subscription-management-backend.US.md)
- [User-Stories/16.2 job-tracker-backend.US.md](../../User-Stories/16.2%20job-tracker-backend.US.md)
- [User-Stories/18.2 notifications-backend.US.md](../../User-Stories/18.2%20notifications-backend.US.md)
- [User-Stories/19.2 privacy-account-deletion-backend.US.md](../../User-Stories/19.2%20privacy-account-deletion-backend.US.md)
- [User-Stories/21.2 content-administration-backend.US.md](../../User-Stories/21.2%20content-administration-backend.US.md)
- [User-Stories/22.2 template-administration-backend.US.md](../../User-Stories/22.2%20template-administration-backend.US.md)
- [User-Stories/23.2 admin-rbac-backend.US.md](../../User-Stories/23.2%20admin-rbac-backend.US.md)
- [User-Stories/24.2 finance-operations-backend.US.md](../../User-Stories/24.2%20finance-operations-backend.US.md)
- [User-Stories/25.2 moderation-abuse-backend.US.md](../../User-Stories/25.2%20moderation-abuse-backend.US.md)
- [User-Stories/26.2 audit-analytics-backend.US.md](../../User-Stories/26.2%20audit-analytics-backend.US.md)
- [User-Stories/27.2 feature-flags-backend.US.md](../../User-Stories/27.2%20feature-flags-backend.US.md)
- [AGENTS.md](../../AGENTS.md)
- [README.md](../../README.md)
