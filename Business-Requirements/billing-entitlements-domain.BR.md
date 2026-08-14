# Business Requirements — Billing And Entitlements Domain

## 1. Document Purpose

This document defines the business requirements for billing and entitlements across the platform.

It is written for business readers who need to understand:

- how monetization fits the product experience
- which capabilities should be free or paid
- why entitlement consistency matters operationally
- what kinds of billing problems create customer dissatisfaction
- how the one-time vs. subscription decision shapes the entire product

The domain includes:

- plans
- premium feature access
- checkout continuity
- export monetization
- template gating
- AI and analysis access rules
- activation and support implications
- refund, cancellation, and data-retention policy

## 2. Domain Objective

The purpose of this domain is not simply "charge money." The purpose is to turn product value into revenue without breaking trust.

That means the billing and entitlement layer must achieve a difficult balance:

- the free experience must be useful enough to prove value
- the premium experience must be differentiated enough to justify payment
- the upgrade experience must not feel deceptive or disruptive

If this balance is wrong, the product will either:

- fail to monetize effectively, or
- monetize in a way that creates user resentment

## 3. Why This Domain Matters

In resume products, the billing layer is unusually sensitive because users often invest emotional effort before they decide whether to pay. By the time they hit an upgrade boundary, they may already have spent:

- time
- focus
- hope
- urgency related to job searching

That means a poor billing experience damages more than checkout conversion. It damages the entire product relationship.

## 4. Research Signals Behind This Domain

### Novorésumé Signals

**[OBSERVED] [DOCUMENTED]**

Public evidence shows:

- **pay-once, time-boxed Premium** (Month / Quarter / Year), with explicit "No recurring billing / no automatic subscription" messaging
- **clear feature differentiation** between Basic and Premium (page length, template/font/theme counts, cover letter, custom layout, specialized sections, job tracker, AI tools)
- a **"preview premium, pay to download"** pattern: premium features marked with a white star can be *tested* free; only *downloading* a document that uses them requires Premium
- a **14-day money-back guarantee** and "no hidden costs" messaging
- **data retained after Premium expiry** (access reduced, content kept)
- payments via Stripe and PayPal

Business interpretation: trust and clarity are positioned as part of the premium story. The monetization gate is deliberately placed at *export*, not at *creation*.

### Resume Now Signals

**[DOCUMENTED]**

Public evidence shows:

- **trial / limited-access pricing** with **recurring renewal** language
- **feature-inclusion matrices** on the pricing page
- **free plain-text (TXT) output** with **richer paid exports** (PDF/DOCX)
- terms of use showing more operational complexity beneath simple marketing copy (multiple offer types)

Business interpretation: monetization is deeply embedded in the workflow and must be operationally robust. Recurring billing introduces trust obligations that must be designed deliberately.

### The Strategic Question

The two references embody two viable strategies:

| | Pay-once, time-boxed (Novorésumé) | Recurring subscription (Resume Now) |
| --- | --- | --- |
| Revenue predictability | Lower (churn to rebuy) | Higher (recurring) |
| Trust / simplicity | High ("no auto-renewal") | Requires strong disclosure |
| Upsell pressure | Mild | Higher (renewal reminders) |
| User mental model | "buy when I need it" | "keep paying while I search" |
| Support complexity | Lower | Higher (cancellations, chargebacks) |

**[RECOMMENDED]** For MVP, favor a **pay-once or short-duration Premium** model (closer to Novorésumé) for its trust advantage and operational simplicity, unless leadership explicitly values recurring revenue predictability more. A hybrid (e.g., pay-once core + optional subscription for continuous AI/analysis) is possible but adds entitlement complexity — defer until the base model is proven.

## 5. Domain Scope

### In Scope

- plan definition
- feature entitlement rules
- premium locks and unlocks
- checkout initiation and return continuity
- export-related monetization
- premium template and AI/analysis access
- billing-support implications
- refund / cancellation / downgrade / expiry policy

### Out Of Scope

- finance accounting systems
- tax compliance design in full detail
- enterprise contract sales flows
- detailed refund-operations UI

## 6. Business Problem This Domain Solves

The platform needs to answer these business questions clearly:

- What does the user get for free?
- What does the user get if they pay?
- When should we ask them to pay?
- How do we ensure they feel the paid value immediately?
- How do we avoid support chaos when payment and access do not align?

This domain exists to make those questions operationally manageable.

## 7. Outcomes We Need From This Domain

### Outcome 1 — Users Understand The Value Ladder

Users should be able to tell the difference between free and premium without reading fine print or discovering it only at the last second.

### Outcome 2 — Upgrade Happens At The Right Moment

Upgrade should happen when the user already sees value and wants more, not before the platform has earned trust.

### Outcome 3 — Paid Access Works Reliably

Once a user pays, the expected features should become available quickly and consistently.

### Outcome 4 — Support Can Diagnose Problems

When something goes wrong with payment or access, support should be able to understand the state and recover the user experience.

### Outcome 5 — Fair Exit Is Communicated

Users must understand what happens after expiry, cancellation, or downgrade (what is kept, what is restricted), matching Novorésumé's explicit "data is saved, access is reduced" policy.

## 8. Domain Requirements

### 8.1 Plan Definition

**Need explanation:** Plans are not just labels like "basic" and "premium." They are bundles of capability decisions.

**Requirement:** The system must model plans in terms of capabilities such as:

- export formats
- template availability
- page or document limits
- AI quota or feature depth
- ATS or analysis depth
- cover-letter access
- customization depth (fonts, themes, layouts)

**Business value:** This makes pricing flexible and prevents entitlement chaos later.

### 8.2 Feature Entitlements

**Need explanation:** A user does not experience "the plan" abstractly. They experience individual product decisions such as "Can I export this?" or "Can I use this template?"

**Requirement:** Entitlement checks must be capability-based, centrally defined, and consistent across the application.

**Business value:** This reduces frustration, support load, and contradictory product behavior.

### 8.3 Pricing Transparency

**Need explanation:** Hidden or unclear billing terms create immediate distrust, especially in a job-search context where users may already feel vulnerable.

**Requirement:** The product must communicate clearly:

- whether billing recurs (and, if so, when)
- whether there is a trial or limited-access period
- what is included
- what happens after expiry, cancellation, or downgrade
- refund eligibility

**Business value:** This improves trust and reduces refund-related dissatisfaction.

### 8.4 Upgrade Continuity

**Need explanation:** Users often encounter upgrade at the point of need, such as export or premium-template selection. If they return from checkout and must reconstruct their task manually, the experience feels broken.

**Requirement:** The system must preserve the original task context when upgrade occurs.

**Business value:** This supports conversion and lowers abandonment after checkout.

### 8.5 Export Monetization

**Need explanation:** Export is one of the strongest monetization points because it is the moment when user effort becomes real-world value. Both references monetize export, but differently (free-TXT vs paid-PDF for Resume Now; preview-free/pay-to-download for Novorésumé).

**Requirement:** The system must distinguish between:

- export blocked because content is not ready
- export blocked because the plan does not allow it
- export running successfully
- export failed operationally

**Business value:** This protects trust and prevents premium upsell from looking like a technical problem.

### 8.6 Premium Templates

**Need explanation:** Templates are both an acquisition surface and a premium value surface. Users must understand why some templates are locked without feeling tricked.

**Requirement:** Premium template availability and value must be clearly signaled before or during selection.

**Business value:** This improves upgrade clarity and reduces disappointment after time is already invested.

### 8.7 AI And Analysis Entitlements

**Need explanation:** AI and analysis can be restricted in multiple ways:

- access/no access
- summary/deep detail
- low/high quota
- standard/premium recommendations
- one-free-use vs unlimited (Novorésumé's toolkit model)

**Requirement:** The platform must distinguish these cases cleanly and present them honestly, including visible quota/credit state (Novorésumé shows a monthly AI credit counter).

**Business value:** This allows the product to monetize advanced intelligence without making the free experience feel fake.

### 8.8 Activation Integrity

**Need explanation:** From the user's point of view, payment success should quickly mean feature access. Delays or inconsistencies feel like the product took money without delivering value.

**Requirement:** Successful payment must result in timely and supportable entitlement activation.

**Business value:** This reduces support issues and protects premium trust.

### 8.9 Data Retention After Expiry Or Downgrade

**Need explanation:** Users often expect their work to remain in the system even if premium access changes. This expectation is visible in competitor messaging (Novorésumé explicitly retains data after Premium expiry).

**Requirement:** The product must define and communicate:

- what remains visible after downgrade
- what remains editable
- what remains exportable
- which premium-only outputs or controls become unavailable

**Business value:** This improves fairness perception and return likelihood.

### 8.10 Refund And Fair-Exit Policy

**Need explanation:** A clear refund posture (Novorésumé: 14-day money-back) and a clear non-renewal story reduce the fear that blocks purchase.

**Requirement:** The product must define refund eligibility, how refunds are requested, and what happens to access on refund. If billing is recurring, cancellation must be simple and clearly communicated.

**Business value:** This lowers purchase anxiety and reduces chargeback risk.

## 9. Business Rules

- entitlement decisions must be centrally defined (single source of truth)
- checkout success does not guarantee export success if document generation later fails
- premium boundaries must be consistent across all channels
- free users must still receive enough value to understand the product
- support must be able to see the user's billing/access state meaningfully
- a "preview premium, pay to download" gate must never hide the fact that a download will be gated
- plan limits (page count, document count, quota) must be single-sourced and identical across pricing page, editor, and help content

## 10. Operational And Technical Implications

### For Product And UX

- pricing pages, in-product locks, and checkout-return flows must tell the same story
- premium messaging should frame value, not punishment
- quota/credit state should be visible to users before they hit a limit

### For Backend

- central entitlement engine required
- idempotent payment handling required (no double charges)
- fast access propagation required (near-immediate activation)
- webhook/notification handling for payment provider events (Stripe/PayPal)

### For Support

- payment status and entitlement status must be diagnosable
- cancellation, downgrade, refund, and recovery processes must be explainable
- support tooling must surface both "what did they pay" and "what can they currently access"

## 11. Risks

### Risk 1 — Product Feels Deceptive

If users discover limits only after major effort, conversion may happen once but trust will erode.

### Risk 2 — Entitlements Are Inconsistent

If the same feature looks free in one place and locked in another, support load and frustration increase quickly.

### Risk 3 — Activation Lags After Payment

Users who pay and still cannot access the expected features are likely to contact support or abandon.

### Risk 4 — Billing Complexity Outgrows Product Clarity

If multiple offer types are introduced too early, the product becomes harder to explain and harder to operate.

### Risk 5 — Recurring-Charge Backlash

If we choose recurring billing and under-communicate renewal terms, we inherit a high refund/chargeback and trust-risk profile.

## 12. Recommended MVP Boundaries

### Must Have

- one free plan
- one premium plan
- clear export gating
- premium template gating
- AI/analysis entitlement integration
- checkout return continuity
- visible quota/credit state (if AI is metered)
- data-retention-after-expiry policy

### Should Have

- promo/coupon support
- clearer plan comparison telemetry
- stronger downgrade messaging
- refund request flow (14-day-style window)

### Could Have

- multiple premium tiers
- advanced promotional logic
- human services upsell packaging
- hybrid pay-once + subscription options

## 13. Open Decisions

- one-time payment, subscription, or hybrid? (recommendation: pay-once/time-boxed for MVP — see Section 4)
- what exactly is free in export? (recommendation: free plain-text or watermarked PDF; paid full PDF/DOCX)
- how much AI/analysis depth is premium in MVP?
- how simple can the initial billing model remain without hurting conversion?
- what are the MVP page-count and document-count limits, and how are they single-sourced?

## 14. Related Story Packs

- [export-entitlements-frontend.US.md](export-entitlements-frontend.US.md)
- [export-entitlements-backend.US.md](export-entitlements-backend.US.md)
- [template-import-frontend.US.md](template-import-frontend.US.md)
- [ai-assistance-frontend.US.md](ai-assistance-frontend.US.md)
- [ats-job-match-frontend.US.md](ats-job-match-frontend.US.md)
