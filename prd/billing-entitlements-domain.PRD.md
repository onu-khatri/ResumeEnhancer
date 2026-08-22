# PRD: Billing And Entitlements Domain

**Status:** Draft  
**Last Updated:** 2026-08-22  
**Source Documents:** `Business-Requirements/billing-entitlements-domain.BR.md`

## Executive Summary

Billing must convert product value into revenue without making a job seeker feel
tricked or abandoned. This domain defines a clear free-to-premium value ladder,
consistent capability entitlements, continuity through checkout, and a fair,
supportable lifecycle after payment, expiry, downgrade, cancellation, or refund.

## Users And Stakeholders

- Free users evaluating value and premium users needing immediate access.
- Product, finance, support, legal/compliance, and engineering stakeholders.

## Scope

### In Scope

- Plans, feature entitlements, premium locks, checkout continuity, export and
  template gating, AI/analysis access, and lifecycle/support policies.

### Out Of Scope

- Finance accounting systems, detailed tax design, enterprise sales contracts,
  and detailed refund-operations UI.

## Requirements

- **REQ-001: Capability-based plans.** Model plans through capabilities such as
  export formats, templates, limits, AI/analysis depth, cover letters, and
  customization.
- **REQ-002: Central entitlements.** Apply a single, capability-based
  entitlement definition consistently across every product surface.
- **REQ-003: Transparent pricing.** Clearly communicate recurrence or trial
  terms, inclusions, expiry/downgrade outcomes, cancellation, and refund
  eligibility.
- **REQ-004: Upgrade continuity.** Preserve the task that triggered checkout
  when the user returns to the product.
- **REQ-005: Honest export states.** Distinguish content-blocked, plan-blocked,
  running/succeeded, and operationally failed exports.
- **REQ-006: Clear premium-template value.** Signal template locks and value
  before or during selection.
- **REQ-007: AI and analysis boundaries.** Explain access, depth, and quota
  states, including visible credit state where AI is metered.
- **REQ-008: Timely activation.** Successful payment must lead to prompt,
  supportable entitlement activation.
- **REQ-009: Fair post-premium access.** Define what remains visible, editable,
  and exportable after expiry or downgrade.
- **REQ-010: Fair exit policy.** Define refund eligibility, refund effects, and
  simple recurring-plan cancellation if recurring billing is selected.

## Success Signals

- Users can understand free versus premium value before encountering a lock.
- Paid access activates reliably and support can diagnose billing/access state.
- Export gates do not appear as product failures.
- **Needs validation:** conversion, activation, refund, and support-health
  targets are not quantified in the BR.

## Dependencies And Risks

- Requires a central entitlement engine, idempotent payment handling, fast
  event-driven activation, and support-visible billing/access state.
- Risks include deceptive-feeling limits, inconsistent entitlements, activation
  lag, early billing complexity, and recurring-charge backlash.

## Open Questions

- Will MVP use a time-boxed pay-once model, subscription, or hybrid?
- Which export is free and what AI/analysis depth is premium?
- What page and document limits apply, and how are they single-sourced?

## Traceability

- REQ-001 to REQ-010 <- `billing-entitlements-domain.BR.md`, Sections 8-9.
- Delivery and risk boundaries <- Sections 10-13.
- Related delivery packs <- `User-Stories/4.1`, `4.2`, `13.1`, `13.2`, `15.1`,
  and `15.2`.
