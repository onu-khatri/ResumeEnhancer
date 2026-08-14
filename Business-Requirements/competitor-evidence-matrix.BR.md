# Business Requirements — Competitor Evidence Matrix

## Purpose

This document is a compact traceability matrix between public competitor evidence and requirement-relevant conclusions. It lets business stakeholders verify *where* a claim came from and *what* it implies for our product, without re-reading the full reference analyses.

Research date: **August 14, 2026**

## How To Read This Matrix

- **Classification** uses the same markers as the other BRs: `Observed` (visible on page), `Documented` (stated in pricing/FAQ/help/legal), `Inferred` (necessary implication), `Context` (market background).
- **Requirement Impact** names the capability or requirement the evidence feeds.
- Rows marked `(re-verify)` could not be re-confirmed live on the research date and rely on the prior pass; treat as pending re-validation.

## Evidence Matrix

| # | Capability / Signal | Competitor | Source URL | Classification | Requirement Impact |
| --- | --- | --- | --- | --- | --- |
| 1 | Start free / low-friction entry | Novorésumé | https://novoresume.com/ | Observed / Documented | free-first activation path |
| 2 | Start free / builder-first funnel | Resume Now | https://www.resume-now.com/ | Observed / Documented | free builder entry required |
| 3 | "No credit card required" + free 1-page download | Novorésumé | https://novoresume.com/ , /page/pricing | Observed / Documented | genuinely usable free tier |
| 4 | Guided builder flow (choose template → answer questions → fill → customize → proofread → save) | Resume Now | https://www.resume-now.com/faq | Documented | stepwise authoring flow |
| 5 | Live preview / intuitive editor / drag-and-drop | Novorésumé | https://novoresume.com/ | Observed / Documented | trustworthy preview workspace |
| 6 | Template switching without losing content | Novorésumé | https://intercom.help/novoresume/en/articles/13107488-resume-changing-template-2-0 | Documented | content/style separation |
| 7 | Layout + entry reordering (custom layout; up/down arrows) | Novorésumé | https://intercom.help/novoresume/en/articles/13127130-resume-adjust-the-layout-and-reorder-entries-2-0 | Documented | section/entry ordering model |
| 8 | Theme feature (tokenized appearance) | Novorésumé | https://intercom.help/novoresume/en/articles/13130627-using-theme-feature-2-0 | Documented | tokenized appearance layer |
| 9 | Multi-language documents (independent per language; no RTL yet) | Novorésumé | https://intercom.help/novoresume/en/articles/13130738-write-documents-in-multiple-languages-2-0 | Documented | language-aware document model |
| 10 | Saved documents / "My Documents" with Views + Tags | Novorésumé | https://intercom.help/novoresume/en/articles/13130815-my-documents-manage-all-your-resumes-and-cover-letters-2-0 | Documented | document-management workspace |
| 11 | Public ATS checker (5-area score) | Novorésumé | https://novoresume.com/tools/ats-resume-checker | Observed / Documented | ATS analysis domain |
| 12 | Public job matcher (5-category breakdown) | Novorésumé | https://novoresume.com/tools/resume-job-matcher | Observed / Documented | job-specific analysis domain |
| 13 | AI tools family (10 tasks, one-free-use) | Novorésumé | https://novoresume.com/career-ai-tools | Observed / Documented | modular AI layer |
| 14 | AI builder / summary / skills generator | Resume Now | https://www.resume-now.com/pricing | Documented | multiple scoped AI capabilities |
| 15 | ATS checker loop back into builder | Resume Now | https://www.resume-now.com/resume/ats-resume-checker | Observed / Documented | analysis-to-edit loop |
| 16 | Resume import formats (.doc, .docx, PDF, RTF, TXT) | Resume Now | https://www.resume-now.com/contact-us | Documented | parsing/file-ingestion pipeline |
| 17 | Rich template filters | Resume Now | https://www.resume-now.com/resume/templates | Observed | metadata-rich template catalog |
| 18 | ATS-friendly template rules | Resume Now | https://www.resume-now.com/resume/templates/ats-friendly | Documented | ATS-safe layout constraints |
| 19 | Cover letter builder + matching support | Resume Now | https://www.resume-now.com/cover-letter/builder | Observed / Documented | shared resume-cover-letter domain |
| 20 | Pay-once Premium, no recurring billing | Novorésumé | https://novoresume.com/page/pricing | Documented | explicit billing-policy communication |
| 21 | Recurring renewal language | Resume Now | https://www.resume-now.com/pricing | Documented | subscription-ready billing logic |
| 22 | Free TXT vs paid PDF/DOCX export | Resume Now | https://www.resume-now.com/pricing | Documented | export monetization ladder |
| 23 | Terms mention nuanced offer/export behavior | Resume Now | https://www.resume-now.com/terms-of-use | Documented | flexible entitlement and offer model |
| 24 | Privacy / legal vendor visibility | Novorésumé | https://novoresume.com/page/legal/privacy-policy | Documented | privacy/compliance requirements |
| 25 | Public support channels and hours | Resume Now | https://www.resume-now.com/contact-us | Documented | supportability requirements |
| 26 | "Preview premium features, pay to download" (white-star features) | Novorésumé | https://novoresume.com/page/pricing | Documented | trust-preserving premium gate |
| 27 | 14-day money-back guarantee | Novorésumé | https://novoresume.com/page/pricing | Documented | refund policy requirement |
| 28 | Data retained after Premium expiry | Novorésumé | https://novoresume.com/page/pricing | Documented | downgrade/data-retention policy |
| 29 | Job tracker (Kanban) as premium add-on | Novorésumé | https://novoresume.com/ | Observed / Documented | adjacent retention surface |
| 30 | AI credit counter for free users | Novorésumé | https://novoresume.com/ (account UI) | Observed | AI quota/entitlement model |
| 31 | "Honest, not flattering" scoring | Novorésumé | https://novoresume.com/tools/resume-job-matcher | Documented | analysis credibility posture |
| 32 | Standalone tool auto-deletes uploads | Novorésumé | https://novoresume.com/tools/resume-job-matcher | Documented | privacy as conversion message |
| 33 | Employer-logo + certification trust signals | Novorésumé | https://novoresume.com/ | Observed | trust-signal requirements |

## Business-Model Comparison

| Dimension | Novorésumé | Resume Now |
| --- | --- | --- |
| Billing cadence | Pay-once, time-boxed (Month/Quarter/Year) | Trial / recurring subscription |
| Auto-renewal | Explicitly none | Yes (recurring) |
| Free tier | Build + download 1-page PDF free | Free plain-text; richer formats paid |
| Premium gate | Preview free, pay to download | Pay to unlock richer exports/features |
| Refund posture | 14-day money-back guarantee | Not clearly stated publicly |
| Trust emphasis | No hidden costs, no auto-renewal | Conversion-led, more fine print |
| AI model | 10-task toolkit, one free use each | Multiple named generators |
| Analysis depth | 5-area ATS + 5-category job match | ATS check + grammar/proofread |

## Feature Ladder (Novorésumé, indicative)

| Capability | Basic (Free) | Premium |
| --- | --- | --- |
| Document versions | Single | Multiple |
| Page length | 1 page | Up to 10 pages |
| Cover letter | None | Matching cover letter |
| Templates | 8 | 16 |
| Fonts / color themes | 3 / 30 | 12 / 74 |
| Layout control | Predefined | Custom layout (drag-and-drop) |
| Creative options | Basic | Backgrounds, picture styles, rating styles, icons |
| Job tracker | 3 cards | Unlimited |
| AI tools | Limited + monthly credits | Extended / unlimited |
| E-learning (Novocareer) | 3 courses | All + AI course generator + AI coach |

*Note: figures are region-localized and show minor public inconsistencies (e.g., document counts of 72 vs 144 across surfaces). Use as directional only; see the Novorésumé reference analysis.*

## Key Cross-Product Conclusions

- The builder is the center, but not the whole product.
- AI and analysis are integrated value loops, not standalone gimmicks.
- Export and premium gating are major conversion moments (both competitors gate export differently: free-TXT vs pay-once-download).
- Template catalogs and examples are acquisition infrastructure.
- Support and privacy maturity influence trust enough to be requirement-worthy.
- The two competitors represent two distinct monetization strategies; choosing between them (or blending) is a deliberate product decision, not a default.

## Evidence Confidence Summary

| Source | Confidence | Notes |
| --- | --- | --- |
| Novorésumé homepage / pricing / tools | High | Re-verified live on research date |
| Novorésumé help-center articles | High | Re-verified live on research date |
| Resume Now pages | Medium | Prior pass; live re-fetch timed out on research date (re-verify) |
| Market context statements | Directional | General background, validate before external use |
