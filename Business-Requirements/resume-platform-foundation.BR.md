# Business Requirements — Resume Platform Foundation

## 1. Document Purpose

This document defines the business requirements for the proposed resume platform at the platform level. It is meant to answer a business stakeholder's first set of questions before engineering starts deep implementation planning:

- What kind of product are we actually building?
- Why would users adopt it instead of simpler tools?
- Which capabilities are essential versus optional?
- Where does business value come from?
- Which product decisions are obvious from research, and which still need explicit leadership choices?

This BR is intentionally broader than a user story and more explanatory than a technical specification. It is written to help:

- product managers make scope decisions
- business analysts create traceable requirements
- UX teams understand what the experience must achieve
- engineering teams understand why a capability matters, not just that it exists

This document is based on public research from:

- [Novorésumé](https://novoresume.com/)
- [Resume Now](https://www.resume-now.com/)

Research baseline date: **August 14, 2026**

## 2. How To Read This Document

This BR uses four evidence markers so business stakeholders can distinguish fact from interpretation:

- **[OBSERVED]** means the behavior was directly visible on a public page or public flow.
- **[DOCUMENTED]** means the behavior or rule was explicitly stated in pricing, FAQ, support, legal, or help content.
- **[INFERRED]** means the requirement is not directly stated but is clearly necessary if the public behavior is real.
- **[RECOMMENDED]** means this is our product recommendation based on research and competitive logic.
- **[CONTEXT]** means general market or industry background used for framing; it is not attributed to any single competitor page and should be independently validated before being used in customer-facing claims.

This distinction matters because business decisions should not be made as if every competitor marketing claim is proven system behavior.

## 3. Research Scope And Business Framing

The research covered public acquisition pages, builder descriptions, pricing pages, FAQs, template libraries, ATS-related tools, AI-related tools, help/support content, terms, privacy material, and public content libraries such as examples and guides.

The goal of the research was not to copy competitors. The goal was to understand:

- what users are being promised
- what workflows are being normalized in the market
- what business capabilities are required to support those promises
- what business tradeoffs appear to drive monetization and retention

### Business Framing

The strongest insight from the research is that modern resume products are no longer "just resume builders." They are becoming lightweight career-application platforms. The resume editor is still the center, but value is increasingly created by the surrounding system:

- templates that reduce design effort
- AI that reduces writing effort
- ATS/job-match tools that reduce uncertainty
- examples and guides that reduce user hesitation
- export and premium unlocks that convert value into revenue

That means our business requirements cannot be limited to "user can create a resume." That would be far too shallow and would not explain the real business model or user expectation.

## 4. Research Sources

### Key Novorésumé Sources

- [Homepage](https://novoresume.com/)
- [Builder landing](https://d.novoresume.com/)
- [Pricing](https://novoresume.com/page/pricing)
- [Resume templates](https://novoresume.com/resume-templates)
- [CV templates](https://novoresume.com/cv-templates)
- [Cover letter templates](https://novoresume.com/cover-letter-templates)
- [Cover letter maker](https://novoresume.com/cover-letter-maker)
- [ATS checker](https://novoresume.com/tools/ats-resume-checker)
- [Resume job matcher](https://novoresume.com/tools/resume-job-matcher)
- [AI tools hub](https://novoresume.com/career-ai-tools)
- Help-center articles (via `intercom.help/novoresume`), including:
  - changing a template without losing content
  - adjusting layout and reordering entries
  - the theme feature
  - writing documents in multiple languages
  - managing all resumes and cover letters in "My Documents"
- public help-center and legal pages

### Key Resume Now Sources

- [Homepage](https://www.resume-now.com/)
- [Pricing](https://www.resume-now.com/pricing)
- [FAQ](https://www.resume-now.com/faq)
- [Contact / support](https://www.resume-now.com/contact-us)
- [Resume templates](https://www.resume-now.com/resume/templates)
- [ATS-friendly templates](https://www.resume-now.com/resume/templates/ats-friendly)
- [Resume examples](https://www.resume-now.com/resume/examples)
- [ATS resume checker](https://www.resume-now.com/resume/ats-resume-checker)
- [Resume summary generator](https://www.resume-now.com/resume-summary-generator)
- [Resume skills generator](https://www.resume-now.com/resume-skills-generator)
- [Cover letter builder](https://www.resume-now.com/cover-letter/builder)
- [Terms of use](https://www.resume-now.com/terms-of-use)

## 5. Why These Two Products Matter

These two products were chosen because together they give a more useful business benchmark than one competitor alone.

### Why Novorésumé Matters

Novorésumé is useful because it presents a relatively coherent and trust-oriented product story. Its public experience suggests a product that wants the user to feel:

- this is easy to start
- this is safe to trust
- this will help me improve, not just format
- I can upgrade when I need more, not before I even know the product

From a business perspective, that matters because it shows a strong example of value-first positioning.

### Why Resume Now Matters

Resume Now is useful because it is more explicit about guided progression, large template/example funnels, and commercially visible premium packaging. It is strong evidence for:

- guided-builder behavior
- multiple AI tools packaged as understandable value units
- ATS as a monetizable product surface
- support and billing realities as part of the customer experience

From a business perspective, Resume Now is a strong benchmark for conversion-oriented product architecture.

### Why Two, Not One

A single reference is dangerous because it conflates one company's commercial choices with what the market actually requires. By holding Novorésumé (trust/one-time-payment oriented) and Resume Now (conversion/subscription oriented) side by side, we separate:

- **market-normal expectations** (both do this) — likely table stakes
- **differentiated strategic choices** (only one does this) — genuinely open decisions for us

The evidence matrix in `competitor-evidence-matrix.BR.md` preserves this "common vs. distinct" distinction for traceability.

## 6. Market Context

**[CONTEXT]** The following framing describes the broader market and should be treated as business background rather than evidence drawn from a specific page. Figures should be validated against current market research before use in investment or pricing decisions.

- **AI has shifted the competitive bar.** General-purpose LLMs can draft resume text, so a resume product that only formats text is increasingly commoditized. The defensible value has moved toward structured output, ATS-aware formatting, job-specific tailoring, and trustworthy, reviewable AI assistance — not raw text generation.
- **The buyer is emotionally invested and time-pressed.** The purchase decision is rarely an impulse buy; it happens after a user has already invested effort in a draft and faces an application deadline. This makes the export/upgrade moment both the highest-conversion and highest-friction point in the funnel.
- **Resume builders are part of broader job-search suites.** The adjacent map (cover letters, job tracking, interview prep, career coaching) creates expansion paths, but also means the product must choose whether to be a focused resume tool or a career platform.
- **Trust signals are disproportionately important** in this category because the user's output (their resume) is personally sensitive and directly tied to livelihood. Privacy, data retention, and "my work is safe" messaging are not compliance overhead — they are conversion levers.

## 7. Executive Summary

The business case for our platform is not merely "let users write resumes online." That market position is too weak, too generic, and too easy to replace.

The product needs to solve a deeper problem:

> Job seekers are not only trying to type a resume. They are trying to reduce uncertainty, improve quality, move faster, tailor for opportunities, and feel confident enough to apply.

The product therefore needs to create business value across five layers:

1. **Start layer** — Reduce friction so users begin quickly and do not abandon before value appears.
2. **Creation layer** — Help users build or import structured resume content without formatting headaches.
3. **Improvement layer** — Help users improve wording, completeness, and relevance through AI and analysis.
4. **Conversion layer** — Turn a useful free experience into premium value at meaningful points like export, advanced templates, or deeper analysis.
5. **Retention layer** — Keep users returning through saved drafts, tailored variants, and repeat application use.

If the platform only solves one of these layers, it will likely feel incomplete and commercially weak.

## 8. Product Vision

### Vision Statement

**[RECOMMENDED]**
Build a professional resume platform that helps users go from uncertainty to application-ready output faster, with less friction and more confidence than general-purpose document tools.

### What This Means In Business Terms

The product should behave like a guided, supportive system rather than a static editor. A good user should be able to:

- discover the product from a template, example, or tool page
- start without feeling trapped by a paywall
- create or import a draft quickly
- improve content with smart assistance
- validate or tailor the resume against job expectations
- export and apply
- return later to reuse or adapt the document

That end-to-end continuity is what creates a real business platform rather than a disposable utility.

### Vision Principles

These principles should constrain every downstream product decision:

1. **Value before payment.** A user should experience the product's value before being asked to pay.
2. **Content is the asset; formatting is the service.** Users should never feel their words are trapped in a template.
3. **Guidance over judgment.** The product should coach and explain, not merely score or reject.
4. **Honesty over hype.** AI and ATS claims must be truthful and supportable, or they will erode trust.
5. **Continuity over one-shot.** The product should be a durable workspace, not a disposable utility.

## 9. Business Objectives

### Objective 1 — Reduce Time To First Resume

Users should be able to get to a meaningful draft quickly, ideally within minutes rather than hours. This matters because job seekers often enter these products under time pressure and uncertainty.

**Why it matters commercially:** time-to-value is the strongest driver of activation, and activation precedes every monetization moment.

### Objective 2 — Improve Resume Quality

The product must help users create better resumes, not only prettier ones. If the product cannot improve clarity, structure, relevance, or ATS readiness, then it is easier for the user to leave for cheaper alternatives.

**Why it matters commercially:** quality is the durable differentiator that AI alone cannot guarantee and that justifies premium pricing.

### Objective 3 — Increase User Confidence

Confidence is a real business outcome here. Users need confidence that:

- their content is saved
- their formatting is acceptable
- their resume is relevant to the target job
- their export will work

Confidence directly affects conversion and retention.

### Objective 4 — Monetize High-Value Moments

The product should not charge too early in a way that blocks trust-building, but it also must not give away all premium value. Monetization should appear at meaningful moments such as:

- better templates
- richer export
- AI depth
- ATS depth
- advanced job matching

### Objective 5 — Support Repeat Usage

A resume product is stronger when it supports multiple applications, tailored versions, and return usage. This improves retention and makes acquisition spending more valuable.

## 10. Core Personas

Each persona is described by its job-to-be-done, its dominant pain point, and the business implication for the platform.

### 10.1 New User

**Job to be done:** "Help me create my first professional resume without knowing where to start."

**Pain points:** blank-page anxiety, uncertainty about structure and tone, fear of making mistakes.

**What they need:** immediate guidance, example content, low-commitment start, visible progress.

Business implication: onboarding and first-use guidance are business-critical, not cosmetic. Empty states and prompts are product requirements, not "nice-to-have" copy.

### 10.2 Active Job Seeker

**Job to be done:** "Get a polished, trustworthy resume ready for a specific application — fast."

**Pain points:** time pressure, fear of ATS rejection, need for the right output format.

**What they need:** polish, ATS compatibility, reliable export formats, speed.

Business implication: export, quality assurance, and premium value must be very clear and must work first time.

### 10.3 Career Changer

**Job to be done:** "Reposition my experience so it is credible for a different role or industry."

**Pain points:** transferability, keyword alignment, positioning of past experience.

**What they need:** guidance on framing, skills translation, and job-match feedback.

Business implication: AI assistance and job-match analysis are particularly important for this segment; they are willing to invest more for tailored help.

### 10.4 Existing Resume Owner

**Job to be done:** "Upgrade or reuse a resume I already have without redoing the work."

**Pain points:** rebuild effort, fear of losing prior content, distrust of import quality.

**What they need:** respect for their prior work, fast import, clear review before finalization.

Business implication: import is not a convenience feature; it is an acquisition and conversion bridge for high-intent users.

### 10.5 Premium User

**Job to be done:** "Pay and immediately feel the upgrade was worth it."

**Pain points:** unclear premium boundaries, activation delays, fear of hidden recurring charges.

**What they need:** instant, visible value; transparent billing; reliable access.

Business implication: entitlement mistakes, activation delays, or unclear premium boundaries are especially damaging — they convert a satisfied user into a support case or a chargeback.

## 11. Stakeholders And Roles

| Stakeholder | Primary interest in this document | Key questions they need answered |
| --- | --- | --- |
| Product Manager | scope, sequencing, prioritization | what is MVP vs. later; where does revenue come from |
| Business Analyst | traceable requirements | which requirements trace to which evidence |
| UX / Design | experience goals and constraints | what emotional outcome must each flow achieve |
| Engineering lead | why a capability matters, complexity signals | which capabilities imply real backend work |
| Data / Analytics | success measurement | what metrics define "working as intended" |
| Legal / Compliance | privacy and claims risk | what claims must be supportable; what data is handled |
| Support / Operations | recoverability and diagnosability | what states must be inspectable and reversible |
| Finance | monetization model viability | one-time vs. subscription tradeoffs; revenue predictability |

## 12. Product Scope At The Platform Level

### In Scope

The platform-level product must account for:

- public discovery surfaces
- authentication and saved-work continuity
- template-first and import-first creation
- structured resume builder
- AI assistance
- ATS and job-match analysis
- cover-letter adjacency
- export
- premium gating / entitlements
- supportability and trust signals

### Out Of Scope For This Platform BR

This document does not fully define:

- internal admin console behavior
- enterprise seat management
- complete cover-letter domain requirements
- recruiter-side products
- finance back-office operations

These may exist later, but they are not required to explain the platform business model today.

## 13. Capability Overview

The platform capability model below is intentionally descriptive so that business readers understand why each capability exists.

### 13.1 Public Discovery And Acquisition

**[OBSERVED]**

Both competitors rely heavily on public-facing assets such as:

- template libraries
- resume examples
- cover-letter examples
- FAQs and how-to guides
- standalone tools like ATS checkers and AI generators

This tells us something important: these public surfaces are not "marketing extras." They are acquisition infrastructure. They attract traffic, reduce hesitation, and move users toward the builder.

Business requirement implication: our platform should treat public content and free tools as part of the product funnel, with measurable conversion from each surface into the builder.

### 13.2 Builder-Centered Authoring

**[OBSERVED] [DOCUMENTED]**

The builder is the operational core of both competitors. However, the strongest benchmark products do not position the builder as a dumb form. They position it as:

- guided
- structured
- design-safe
- previewable
- improvement-ready

Business requirement implication: our product must make the builder the center of value creation.

### 13.3 AI Assistance

**[OBSERVED] [DOCUMENTED]**

AI is publicly packaged as practical help:

- resume summary generation
- skill generation
- bullet improvement
- section guidance
- job-tailored content help

This matters because AI is being normalized as a standard expectation, not a novelty.

Business requirement implication: AI should be practical, scoped, and explainable — mapped to specific writing tasks rather than offered as a free-form "magic" box.

### 13.4 ATS And Job-Match Analysis

**[OBSERVED] [DOCUMENTED]**

Both references publicly promise more than "build a nice resume." They promise insight into:

- ATS compatibility
- formatting issues
- keyword alignment
- section quality
- job-match gaps

Business requirement implication: the platform must support analysis as a product experience, not just a backend utility. Analysis must be structured, actionable, and honest about its limits.

### 13.5 Export And Premium Unlock

**[OBSERVED] [DOCUMENTED]**

Export is where user effort turns into tangible value. It is also where monetization pressure becomes real.

Business implication: premium rules around export must be especially transparent and robust. The four export states (blocked-by-content, blocked-by-plan, succeeded, failed-operationally) must be distinguishable so an upsell never looks like a technical failure.

### 13.6 Saved Work And Tailoring

**[OBSERVED] [INFERRED]**

Public signals from both products indicate multi-document and repeat-usage value. Users are expected to:

- save work
- return later
- create multiple versions
- tailor by role or opportunity

Business implication: saved-document and duplicate/tailor behavior is part of the core product, not a later enhancement.

## 14. Platform-Level Business Requirements

This section states the key platform requirements with explanation.

### 14.1 Users Must Be Able To Start Without High Friction

**Need description:** Job seekers often arrive with urgency and uncertainty. If the platform demands too much commitment too early, many users will abandon before seeing value.

**Requirement:** The platform must provide a low-friction path into creation and must preserve user intent when signup sits between entry and action.

**Why it matters to business:** This directly affects activation rate and paid-conversion opportunity later.

### 14.2 Users Must Be Able To Build Or Import A Resume Quickly

**Need description:** Not every user starts from zero. Some need a guided blank-start experience, while others need a faster import-and-refine path.

**Requirement:** The platform must support at least:

- template-first creation
- import-first creation

**Why it matters to business:** Without both entry paths, the platform excludes either novice users or returning users with existing documents.

### 14.3 The Platform Must Improve Resume Quality, Not Just Appearance

**Need description:** A user does not pay only for formatting. They pay for confidence that the document is stronger, clearer, and more relevant.

**Requirement:** The product must support guidance, AI improvement, and analysis loops that improve content quality and relevance.

**Why it matters to business:** This creates differentiation and justifies premium value.

### 14.4 Users Must Understand What Is Saved, What Is Free, And What Is Premium

**Need description:** Resume workflows are emotionally sensitive. Users become frustrated quickly if they think:

- their work is lost
- export is broken
- pricing is misleading

**Requirement:** The platform must communicate save state, entitlement state, and upgrade boundaries clearly.

**Why it matters to business:** This affects trust, support load, and churn.

### 14.5 The Product Must Support Reuse And Tailoring

**Need description:** Users rarely submit a single resume once and never return. They often need several tailored versions.

**Requirement:** The product must support document persistence, duplication, and adaptation across jobs and time.

**Why it matters to business:** This increases retention and repeat usage.

### 14.6 The Platform Must Be Honest And Supportable

**Need description:** Because the output is personally sensitive and tied to livelihood, false or unverifiable claims (in AI output, ATS scores, or pricing) create outsized damage.

**Requirement:** All user-facing claims must be truthful and supportable; all failure states (billing, import, save, export, AI) must be diagnosable and recoverable by support.

**Why it matters to business:** This protects credibility, reduces chargebacks/refunds, and lowers support cost.

## 15. Non-Functional Business Requirements

These are business-level constraints that shape the product even though they are not a single feature.

### 15.1 Privacy And Data Protection

Resume content is personally sensitive (contact details, employment history, education). The platform must:

- provide clear, accessible privacy communication
- support the user's ability to access and delete their own data
- align with applicable regulation (e.g., GDPR for EU users) where relevant

**[RECOMMENDED]** Treat privacy posture as a trust/conversion feature, not only a legal checkbox — both references lean on privacy messaging as a trust signal.

### 15.2 Reliability And Save Integrity

Perceived data loss is the single most damaging failure mode in this category. The platform must ensure saved content is never silently lost and that save state is always communicated.

### 15.3 Accessibility And Inclusivity

A job-search tool should be usable by a broad population, including users of assistive technology. Accessibility is both an ethical obligation and an acquisition opportunity.

### 15.4 Performance Perception

Because users are time-pressed, perceived slowness (long preview render, slow export, slow AI responses) reads as low product quality. Performance targets should be set for the moments users feel most: first draft, preview update, export, and analysis results.

## 16. Cross-Competitor Conclusions

### Conclusion 1 — The Market Expects More Than Static Templates

Templates still matter, but they are no longer enough. The surrounding system now creates the real competitive value:

- AI
- ATS analysis
- job matching
- examples/resources
- saved documents

### Conclusion 2 — Trust Is Not Optional

The public leaders in this space lean heavily on signals such as:

- transparent pricing
- support availability
- privacy posture
- claims of ATS safety
- guided experience

This means trust is part of the product requirement, not only branding.

### Conclusion 3 — Premium Value Must Be Layered

Users need a useful free experience, but premium value must still be obvious. Competitor evidence suggests that premium value is strongest when attached to:

- richer output
- more design control
- deeper AI
- better analysis
- more document flexibility

### Conclusion 4 — The Business Model Is A Product Decision, Not An Afterthought

Novorésumé and Resume Now make materially different monetization choices (one-time vs. recurring). This proves that monetization is a strategic differentiator with real product consequences (messaging, entitlement design, support complexity) — it must be decided deliberately, not defaulted.

## 17. Platform Constraints And Ambiguities

This section is written for business readers so they understand where public research is strong and where it is uncertain.

### Novorésumé Ambiguities

- some public pages appear inconsistent about exact free/basic packaging
- public messaging around export formats is not perfectly uniform
- some public document-count limits differ across pages

Business implication: we should not copy their packaging mechanically; we should normalize our own rules cleanly and avoid public inconsistency.

### Resume Now Ambiguities

- limited-access vs full-access differentiation is not fully clear from public pricing alone
- pricing and terms suggest multiple offer behaviors across time or cohorts
- some premium output rules are more complex than the high-level marketing pages suggest

Business implication: our own billing model should be intentionally simpler at MVP unless complexity is truly needed.

### Shared Research Limits

The following cannot be confirmed from public material alone and must be treated as open items:

- exact data retention and versioning behavior
- exact backend scoring and AI provider strategy
- internal admin and content-management structures
- exact entitlement enforcement rules at the API level

## 18. Strategic Recommendations

### Recommendation 1 — Build The Product As A Connected Workflow

The product should behave like:

`discover -> create/import -> improve -> validate -> export -> return`

not like:

`fill form -> download once -> leave`

### Recommendation 2 — Let Premium Follow User Momentum

Premium prompts should appear when the user already understands the value of the product, especially around:

- export
- advanced templates
- AI depth
- analysis depth

### Recommendation 3 — Treat Supportability As Part Of Product Design

Because resume work is high-emotion and often time-sensitive, the platform must make it easy to understand and recover from:

- payment issues
- import issues
- save issues
- export issues

### Recommendation 4 — Decide The Monetization Model Deliberately

Choose one-time, subscription, or hybrid based on a reasoned tradeoff (see `billing-entitlements-domain.BR.md`), and make that choice consistent across pricing, in-product messaging, and legal/terms. Inconsistency is the single fastest way to erode the trust this category depends on.

### Recommendation 5 — Keep Claims Supportable

Any ATS score, AI guarantee, or "success" claim must have a defensible basis. Overpromising hiring success is both a credibility and regulatory risk.

## 19. Recommended MVP Scope

### Must Have

- account-based saved documents
- template-first creation
- import-and-review path
- structured builder with preview
- PDF export
- free vs premium entitlement engine
- AI assistance for core sections
- ATS-style analysis
- job-description matching

### Should Have

- DOCX export
- matching cover letters
- deeper AI actions
- richer template filtering

### Could Have

- public share links
- human expert review layer
- anonymous draft mode
- advanced multilingual and RTL support

## 20. Risk Register

| Risk | Likelihood | Impact | Mitigation |
| --- | --- | --- | --- |
| Builder is too generic / undifferentiated | High | High | Treat guidance, structure, and analysis as first-class, not cosmetic |
| Perceived data loss erodes trust | Medium | Very High | Autosave + explicit save states + recovery paths |
| Preview/export divergence feels deceptive | Medium | High | Make preview faithful to final output; gate on the same rules |
| Entitlement inconsistency across surfaces | Medium | High | Central entitlement engine, single source of truth |
| AI output is inaccurate or misleading | Medium | High | Reviewable suggestions, honesty guardrails, moderation |
| Overpromised ATS/success claims | Medium | High | Supportable, clearly heuristic messaging |
| Billing model confusion (one-time vs. recurring) | Medium | High | Transparent pricing + terms + in-product consistency |
| Monetization too early / paywall resentment | Medium | Medium | Value-first free experience; upgrade at momentum moments |

## 21. Success Measures For Business Stakeholders

The platform is meeting its business purpose when:

- users reach first draft quickly
- users complete resumes without excessive abandonment
- users perceive AI and analysis as helpful rather than confusing
- free users understand why premium is worth paying for
- premium users successfully export and reuse their documents
- support issues around billing and data loss remain low

**[RECOMMENDED]** These qualitative goals should be backed by instrumented, measurable proxies. Candidate north-star and guardrail metrics:

| Metric type | Candidate metric | Rationale |
| --- | --- | --- |
| Activation | time-to-first-draft; % of new users who create a draft | captures Objective 1 |
| Engagement | sections completed; AI actions used per draft | captures Objective 2 |
| Trust | save-state error rate; export success rate | captures Objective 3 |
| Monetization | free-to-paid conversion; upgrade completion at export | captures Objective 4 |
| Retention | return rate; documents-per-user; tailored variants created | captures Objective 5 |
| Support health | billing/data-loss support tickets per 1,000 users | captures supportability |

## 22. Assumptions And Dependencies

- The market continues to value ATS compatibility and job matching as premium, differentiated features.
- AI-assisted writing remains a differentiator only if it is scoped, reviewable, and honest; raw text generation alone is not a durable moat.
- The product will be delivered incrementally, with the builder and export forming the first monetizable core.
- User authentication and account continuity are required for saved-work value (the "existing resume owner" and "premium user" personas depend on it).

## 23. Glossary

| Term | Meaning in this document |
| --- | --- |
| ATS | Applicant Tracking System — software employers use to parse, filter, and rank resumes |
| Builder | the interactive authoring workspace where users create and edit a structured resume |
| Entitlement | the set of features/actions a given user is allowed based on plan and payment state |
| Job match | analysis that compares a resume against a specific job description |
| Template | a pre-designed resume layout/style applied to structured content |
| Tailoring | adapting a resume (or a duplicate of it) to a specific role or application |
| Structured content | resume data stored as typed sections/items rather than one free-form text block |
| Premium | paid access tier that unlocks additional capability |

## 24. Traceability

This foundation BR feeds the following domain BRs, which expand the corresponding capabilities into detailed requirements:

| Domain BR | Capability it expands |
| --- | --- |
| `builder-domain.BR.md` | creation, editing, preview, save, duplication/tailoring |
| `ai-analysis-domain.BR.md` | AI assistance, ATS analysis, job matching, recommendations |
| `billing-entitlements-domain.BR.md` | plans, entitlements, export monetization, activation |

Reference analyses (`novoresume-reference-analysis.BR.md`, `resume-now-reference-analysis.BR.md`) and the `competitor-evidence-matrix.BR.md` preserve the underlying evidence.

## 25. Next Documents That Should Build On This BR

- [builder-domain.BR.md](builder-domain.BR.md)
- [ai-analysis-domain.BR.md](ai-analysis-domain.BR.md)
- [billing-entitlements-domain.BR.md](billing-entitlements-domain.BR.md)
- future dashboard, admin, RBAC, cover letter, and support domain BRs
