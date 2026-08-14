# Business Requirements — AI And Analysis Domain

## 1. Document Purpose

This document defines the business requirements for the combined AI and Analysis Domain.

The domain includes:

- AI-assisted writing
- AI-assisted improvement
- ATS-style evaluation
- job-description matching
- recommendation generation
- rerun and improvement loops
- quota and entitlement implications
- honesty, safety, and privacy guardrails

This document is intentionally explanatory. It is meant to help business stakeholders understand why this domain exists and what value it is supposed to create, not only to list technical features.

## 2. Domain Objective

The objective of this domain is to reduce two of the biggest pain points in resume creation:

1. **Users do not know what to write or how to phrase it well.**
2. **Users do not know whether their resume is actually strong enough for a target role.**

AI addresses the first pain point.
Analysis addresses the second.
Together, they create a feedback loop that improves the value of the entire platform.

## 3. Why This Domain Matters

Without AI and analysis, a resume platform is easier to compare against generic template sites or traditional document tools.

With AI and analysis, the platform becomes more outcome-oriented. Instead of simply saying:

> "Here is a place to type your resume,"

the platform can say:

> "Here is a place where your resume gets better."

That difference is important for:

- premium conversion
- user trust
- repeat usage
- product differentiation

## 4. Research Signals Behind This Domain

### Novorésumé Signals

**[OBSERVED] [DOCUMENTED]**

Public pages show:

- **AI assistant inside the builder** that (a) turns basic job details into achievement bullets, (b) generates content from scratch matched to industry keywords, (c) gives personalized feedback on specific highlighted sections, and (d) provides real-time coaching advice — with an explicit **"never invents qualifications" / "true to your experience"** stance
- a **10-task AI toolkit** (Resume Generator, ATS Checker, Job Matcher, Objective Generator, Rewriter, Cover Letter Generator, Summary Generator, Bullet Generator, Bio Generator, Salary Calculator) with **one-free-use** per tool
- an **ATS checker** scoring five areas (Job Match, Format & Structure, Content Quality, Section Organization, Technical Elements) with per-category sub-scores and actionable fixes, available as a free quick check vs. a deeper emailed report
- a **job matcher** with a five-category breakdown (Hard Requirements, Skills Alignment, Seniority Fit, Industry Match, Career Trajectory) and an explicit **"honest, not flattering"** posture
- **"AI that sounds like a person" / "no AI slop"** positioning

Business interpretation: AI and analysis are positioned as practical, honest decision-support tools, not novelty features, and are deliberately modular (named tasks) rather than one generic box.

### Resume Now Signals

**[DOCUMENTED]**

Public pages show:

- AI resume builder
- summary generator
- skills generator
- ATS checker
- review/improvement positioning
- a related article that separates **content improvement** from **final template formatting**

Business interpretation: users understand AI more easily when it is packaged into narrow, understandable tasks.

## 5. Domain Scope

### In Scope

- section-level AI assistance (summary, bullets, skills, objective, rewrite, tailoring)
- ATS compatibility analysis
- resume quality analysis
- job-match analysis
- recommendation generation
- rerun/improvement loops
- premium and quota boundaries for these features
- honesty/safety/privacy guardrails

### Out Of Scope

- recruiter-side hiring products
- fully autonomous application submission
- model-training platform design
- enterprise AI governance tooling

## 6. Business Problem This Domain Solves

Users have several common questions during resume work:

- "How do I say this better?"
- "What skills should I highlight?"
- "Is this resume good enough?"
- "Will ATS reject this?"
- "How well does this fit the role I want?"

If the platform can answer those questions in a credible and actionable way, it becomes much more valuable than a static builder.

## 7. Outcomes We Need From This Domain

### Outcome 1 — Users Get Unstuck Faster

AI should reduce writer's block and help users move forward.

### Outcome 2 — Users Improve Resume Quality

The product should help users produce clearer, stronger, and more relevant resume content.

### Outcome 3 — Users Understand Gaps Before Applying

Analysis should reduce blind application behavior by showing:

- what is strong
- what is weak
- what is missing

### Outcome 4 — Users Return To Improve, Not Just Download

This domain should create iteration loops that make the product more habit-forming and useful over time.

### Outcome 5 — Users Trust The Output

AI must be honest (no invented qualifications) and analysis must be explainable (not a bare score). Trust is the whole game; a single obviously-wrong AI suggestion can poison the relationship.

## 8. Domain Requirements

### 8.1 AI Must Be Contextual

**Need explanation:** Users do not want "AI" as an abstract concept. They want help with the exact task in front of them.

**Requirement:** AI should be attached to specific user goals such as:

- write a summary
- improve a bullet
- generate skills
- write an objective
- rewrite for a target role
- tailor content to a job description

**Business value:** This increases usability and makes premium value easier to explain.

### 8.2 AI Must Preserve User Control

**Need explanation:** Users are often wary of generated content, especially for something as personal as a resume. They need to trust that the system is helping, not taking over.

**Requirement:** The platform must not silently overwrite user content with AI output. AI suggestions must remain reviewable until explicitly accepted, with an easy "keep original" option.

**Business value:** This protects trust and reduces complaints about inaccurate or fake-sounding content.

### 8.3 AI Must Be Honest And Safe

**Need explanation:** Resume content is high-stakes. Users may be tempted to overstate skills, and AI can unintentionally generate misleading phrasing or fabricated credentials.

**Requirement:** The system must be designed to minimize fabricated or inflated content (e.g., never invent qualifications, roles, or metrics), and must handle moderation, quota, and provider failure gracefully. This mirrors Novorésumé's explicit "true to your experience" commitment.

**Business value:** This protects product credibility and reduces reputational and regulatory risk.

### 8.4 Analysis Must Be More Than A Score

**Need explanation:** Users cannot act on a number by itself. A score without explanation creates curiosity but not real improvement.

**Requirement:** The system must provide categorized findings and recommendations, not only one top-line score. A structured score (e.g., overall + per-category sub-scores) with concrete fixes is the proven pattern.

**Business value:** This increases perceived usefulness and supports an improvement loop instead of a novelty interaction.

### 8.5 Job Match Must Be Distinct From Generic Resume Quality

**Need explanation:** A resume can be "good" in a general sense and still be weak for a specific job. The platform must distinguish those two concepts.

**Requirement:** The product must support job-description-specific analysis as a separate or clearly differentiated mode, with multi-dimensional feedback (e.g., hard requirements, skills alignment, seniority fit, industry match, career trajectory).

**Business value:** This is one of the strongest premium and retention opportunities because users often apply to multiple roles.

### 8.6 Recommendations Must Be Actionable

**Need explanation:** If the system says "improve your resume," the user still does not know what to do. Recommendations need practical meaning.

**Requirement:** The system should identify what to change, ideally mapping back to a section, wording pattern, or missing signal — and distinguish "fix in the resume" from "address in the cover letter."

**Business value:** This closes the loop between analysis and editing.

### 8.7 Results Must Be Traceable

**Need explanation:** If the resume changes after a report is generated, the old result may no longer be valid. Business users and support teams also need to understand what the system actually evaluated.

**Requirement:** AI and analysis results must be tied to the content context or version they were created from.

**Business value:** This improves user trust, QA accuracy, and supportability.

### 8.8 Premium Value Must Be Understandable

**Need explanation:** AI and analysis often become premium surfaces. If the difference between free and premium is vague, users feel manipulated rather than helped.

**Requirement:** The product must clearly communicate whether a limitation is due to:

- free-plan boundary
- quota exhaustion
- unavailable service
- content validation issue

**Business value:** This supports cleaner conversion and lower frustration.

### 8.9 Privacy Must Be Explicit For Standalone Tools

**Need explanation:** Public AI/ATS tools that accept uploaded resumes create privacy anxiety. Novorésumé addresses this by stating uploads are "stored securely and automatically deleted."

**Requirement:** Any standalone (or no-signup) AI/analysis surface must clearly state what happens to uploaded data (retention, deletion) and how it is protected.

**Business value:** Privacy is a conversion message in this category and reduces abandonment on upload steps.

## 9. Business Rules

- AI output is advisory until accepted.
- Analysis scores are heuristic guidance, not promises of interview success.
- Job-match results must relate to a specific job-description context.
- Re-runs should generate fresh result records, not silently overwrite history.
- The product must not present stale results as current truth.
- AI must not fabricate qualifications, metrics, or experience; it may only rephrase/restructure user-provided facts.
- Standalone-tool uploads must follow a clearly communicated retention/deletion policy.

## 10. Frontend And Experience Implications

For business people, this means the UI must behave in a particular way:

- AI should feel embedded in the builder, not detached from it.
- Analysis should feel like a readable review surface, not a backend debug dump.
- Users must have a clear route from feedback back into editing.
- Premium prompts must appear in context, not as generic interruptions.
- AI actions should be named, discrete tasks (summary, bullet, skills, rewrite) so users know what they are invoking.
- Quota/credit state should be visible before it becomes a blocker.

## 11. Backend And Operations Implications

For operations and engineering, this domain implies:

- quota tracking (credits/usage per user per period)
- entitlement checks
- stable suggestion/result contracts (typed, versioned)
- snapshot-aware analysis (results linked to content version)
- supportable failure handling (provider timeout, rate limit, partial output)
- provider abstraction and cost controls (AI costs are variable and can scale unexpectedly)
- logging/audit of what was generated vs. accepted (for support and trust)

This is important because the business promise of AI and analysis cannot be delivered reliably without clear backend state management.

## 12. Risks

### Risk 1 — AI Feels Gimmicky

If the AI is too generic or too detached from real editing tasks, users will try it once and ignore it.

### Risk 2 — Analysis Feels Arbitrary

If users see scores without useful explanations, they may not trust the product.

### Risk 3 — Quota Confusion

If users cannot tell the difference between quota, premium gating, and service failure, frustration will rise quickly.

### Risk 4 — Overpromising Outcomes

If public or in-product messaging implies guaranteed hiring success, the product becomes less credible and more risky.

### Risk 5 — AI Fabrication

If AI invents credentials or metrics, users can submit misleading resumes, creating both user harm and reputational/regulatory exposure.

### Risk 6 — Variable AI Cost

If AI usage is unlimited and unmonitored, cost can grow unpredictably against a fixed-price plan.

## 13. Recommended MVP Boundaries

### Must Have

- summary/bullet/section-level AI help
- ATS-style feedback (categorized, actionable)
- job-description match flow (multi-dimensional)
- actionable recommendations
- version-linked or context-linked results
- no-fabrication guardrails + reviewable suggestions
- visible quota/credit state

### Should Have

- skills generation
- richer rewrite options
- more nuanced severity levels
- standalone no-signup ATS quick-check funnel

### Could Have

- human-assisted review
- advanced industry-specific scoring models
- public no-signup result-delivery funnels
- objective/bio/salary tool expansion

## 14. Open Decisions

- Which AI actions are included in MVP?
- How much ATS/job-match detail is free versus premium?
- Which parts of analysis are synchronous versus queued?
- How much score explanation should be visible to users?
- What is the AI quota model (credits/month) for free vs. premium, and how is it communicated?
- How do we enforce "no fabrication" technically (prompting + output validation)?

## 15. Related Story Packs

- [ai-assistance-frontend.US.md](ai-assistance-frontend.US.md)
- [ai-assistance-backend.US.md](ai-assistance-backend.US.md)
- [ats-job-match-frontend.US.md](ats-job-match-frontend.US.md)
- [ats-job-match-backend.US.md](ats-job-match-backend.US.md)
