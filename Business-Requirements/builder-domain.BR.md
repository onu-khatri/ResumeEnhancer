# Business Requirements — Builder Domain

## 1. Document Purpose

This document defines the business requirements for the Builder Domain. It is written to explain not only what the builder should do, but why the builder matters commercially and operationally.

The builder domain includes:

- draft creation
- section-based editing
- import continuation
- template use during editing
- preview
- save state
- duplication and tailoring

This is the most important product domain because almost every other feature depends on it. Templates, AI, ATS, export, and premium upgrades all become weak if the builder does not create trust and momentum.

## 2. Domain Objective

The business objective of the builder is simple to say but not simple to implement:

> Help a user turn uncertainty, scattered career information, or an existing imperfect resume into a polished, structured, reusable application document.

For business stakeholders, this means the builder must achieve several outcomes simultaneously:

- reduce blank-page anxiety
- reduce formatting effort
- reduce the chance of producing a poor-quality resume
- preserve enough flexibility for tailoring
- maintain confidence that the document is safe and real

## 3. Why The Builder Domain Matters

In both selected competitor references, the builder is not just a screen. It is the moment where product promise becomes user trust.

If the user feels:

- lost,
- unsure what to write,
- unsure if the document is saved,
- unsure if the preview is real,
- unsure whether the structure is professional,

then the platform loses both conversion and retention power.

That is why the builder must be treated as a business-critical workflow domain, not only a frontend feature.

## 4. Research Signals Behind This Domain

### Novorésumé Signals

**[OBSERVED] [DOCUMENTED]**

Public Novorésumé pages and help articles indicate:

- an intuitive editor with live preview, drag-and-drop, and straightforward navigation
- **template switching without content loss** (help center: "Your written content will not change when you switch templates, but the flow of the content may be different")
- a **theme feature** (fonts, color themes, backgrounds, picture styles, rating styles, icons) — i.e., a tokenized appearance layer
- **custom layout**: drag-and-drop reordering of *sections*; *entries* reordered via up/down arrows
- **multi-language documents** that are independent per language (no cross-language sync), with translatable section labels, and no RTL support yet
- **"My Documents"** workspace with Views (All / Resumes / Cover Letters) and custom **Tags** for organization
- a **reusable "My Content" library** (user reviews repeatedly cite this as the reason they can quickly duplicate and tailor resumes)
- **layout-aware AI** that preserves design while never inventing qualifications

Business meaning: users are expected to trust the builder as a durable, reusable working environment, not just a one-time form.

### Resume Now Signals

**[DOCUMENTED]**

Public Resume Now FAQ and support content indicate:

- guided, question-led template-first creation
- add-section behavior
- formatting controls and spell check
- save/download workflow
- upload-existing-resume entry path (`.doc`, `.docx`, PDF, RTF, TXT)

Business meaning: the builder must support guided progression and operational practicality, not only visual polish.

## 5. Domain Scope

### In Scope

- create draft from template or import
- edit structured resume sections
- reorder, hide, and manage sections/items
- template application and switching
- theme/appearance customization (within ATS-safe bounds)
- validation
- preview
- save / autosave confidence
- duplicate and tailor workflows
- document organization (views/tags/labels)

### Out Of Scope

- AI provider orchestration
- ATS scoring rules
- billing logic
- cover-letter-specific domain
- admin template publishing

## 6. Business Problem This Domain Solves

Users do not come to a resume builder merely to type content. They come to solve business-relevant personal problems such as:

- "I need a resume today and I do not know where to begin."
- "I already have a resume, but it looks outdated."
- "I need different versions for different roles."
- "I do not trust myself to format this correctly."
- "I do not want to rebuild the whole thing every time."

The builder must address all of those concerns in one experience.

## 7. Builder Outcomes We Need

### Outcome 1 — Users Reach A Real Draft Quickly

The builder must get users to an editable document quickly, whether through blank creation or import. Time-to-first-draft is important because hesitation and abandonment are highest at the start.

### Outcome 2 — Users Understand The Structure Of The Resume

The product must teach or reinforce the idea that a professional resume has meaningful sections and ordering. This is important for users who are not career experts.

### Outcome 3 — Users Trust What They See

Preview must feel believable, and save state must feel reliable. If either fails, the builder loses authority as a professional tool. The preview and the export must reflect the same document (Novorésumé's "what you design is what hiring managers see" is the bar).

### Outcome 4 — Users Can Reuse Their Work

The builder must support not only creation but adaptation. Reuse is a direct business enabler for retention. A reusable content library (content separated from design) is the enabling architecture for this.

## 8. Domain Requirements

### 8.1 Draft Creation

**Need explanation:** The platform must turn user intent into a usable working draft immediately. A builder that delays draft creation or makes the user repeat decisions loses momentum.

**Requirement:** The system must support draft creation from:

- a selected template
- an approved import result
- a default starter flow

**Business value:** This protects activation and reduces abandonment during the first product session.

### 8.2 Structured Resume Model

**Need explanation:** A free-form text editor is too weak for the product we are trying to build. AI, ATS checks, template switching, import correction, and targeted editing all work better when content has structure.

**Requirement:** The builder must work on structured sections and section items rather than one opaque block of text.

**Business value:** This increases quality, enables better automation, and improves scalability of product features.

### 8.3 Guided Authoring

**Need explanation:** Many users do not know the correct order, tone, or completeness level of a resume. If the product expects them to be experts already, it will underserve the largest market.

**Requirement:** The builder must provide a guided section-by-section experience with helpful empty states, example placeholder text, and clear progression cues. Novorésumé reviews explicitly praise being "guided through the entire resume with proper keywords."

**Business value:** This broadens addressable market beyond users who are already confident writers.

### 8.4 Section Management

**Need explanation:** A professional resume is not static. Users need to add, remove, reorder, hide, and adapt sections depending on role, experience level, and industry.

**Requirement:** The builder must support:

- add/edit/remove items
- reorder sections (drag-and-drop where practical)
- reorder items (arrows or drag)
- hide/show sections
- preserve hidden content where appropriate
- a catalog of standard plus specialized sections (e.g., projects, publications, certifications, volunteering, languages)

**Business value:** This makes tailoring practical, which supports repeat usage and better job-fit outcomes.

### 8.5 Template Application And Switching

**Need explanation:** Users should not feel punished for changing design direction after they have entered content. Public competitor evidence strongly confirms that preserving content across template changes is expected (Novorésumé documents this explicitly).

**Requirement:** The system must allow template changes without content loss, while clearly managing layout or pagination changes (content may re-flow across pages because templates have different dimensions).

**Business value:** This encourages experimentation and reduces restart frustration. It also proves content and design are cleanly separated, which is foundational to premium "custom layout" and "theme" features.

### 8.6 Appearance And Theme Customization

**Need explanation:** Visual control is a major premium lever. Novorésumé gates fonts (3 vs 12), color themes (30 vs 74), backgrounds, picture styles, rating styles, and icons behind Premium.

**Requirement:** The builder must expose a tokenized appearance layer (fonts, colors, accents, spacing) that can be changed independently of content, with ATS-safe limits enforced (e.g., block unsupported characters/tables in ATS-friendly modes).

**Business value:** This creates a clean, monetizable customization ladder and prevents "design breaks parsing" errors.

### 8.7 Validation

**Need explanation:** Users need more than error checking. They need to know the difference between:

- something that prevents a valid export or analysis
- something that is merely weakening the document

**Requirement:** The builder must support at least two validation severities:

- blocking issue
- non-blocking quality warning

**Business value:** This enables clearer UX, better support, and more trustworthy downstream tools.

### 8.8 Save Confidence

**Need explanation:** Resume writing is often interrupted, emotional, and time-sensitive. Users are highly sensitive to perceived data loss.

**Requirement:** The builder must communicate whether the draft is:

- saving
- saved
- failed
- in conflict

**Business value:** Strong save confidence reduces abandonment and support cost.

### 8.9 Preview Trust

**Need explanation:** Users use preview to decide whether the document is truly professional. If preview and export diverge, the product feels deceptive or low quality.

**Requirement:** The preview must reflect actual document structure and layout closely enough to be trusted for decision-making, and must apply the same entitlement rules as the final export.

**Business value:** This supports conversion, especially near export.

### 8.10 Duplication And Tailoring

**Need explanation:** Resume creation is rarely one-and-done. Tailoring is one of the most important repeat-use behaviors, and competitor reviews cite duplication as a top reason for premium value.

**Requirement:** Users must be able to duplicate or branch from an existing draft and adapt it for specific roles, reusing a shared content library.

**Business value:** This supports retention and premium usage.

### 8.11 Document Organization

**Need explanation:** As users accumulate multiple resumes and cover letters (and multi-language variants), they need to find and manage them.

**Requirement:** The builder/workspace must support document organization: at minimum list views filtered by type, plus user-defined tags/labels for language, role, or market.

**Business value:** Organization reduces abandonment at the "many documents" stage and supports multi-market users.

## 9. Business Rules

- A document belongs to its owner unless explicitly shared.
- Visibility is not the same as deletion (hiding a section must not delete its content).
- Section order is a meaningful business property, not cosmetic metadata.
- Imported content should not become final output until the user has a chance to review it.
- Template changes must preserve content integrity.
- Content and design are separate concerns; changing one must not corrupt the other.
- ATS-safe constraints apply to the appearance layer and must be enforced (or at minimum warned) before export.

## 10. Operational And Technical Implications

### For Frontend

The builder must feel like a workspace, not like a long settings page. That means:

- strong section orientation
- visible preview where possible
- stable save feedback
- clear recovery states
- responsive/mobile-tolerant editing (Novorésumé markets mobile-friendliness)

### For Backend

The backend must support:

- partial updates (structured section-level writes, not whole-document rewrites)
- stable versioning/conflict handling
- structured validation payloads
- reliable draft retrieval
- a content model that supports document duplication and a reusable content library
- metadata for organization (tags/labels, document type, language)

### For QA

QA must validate more than happy-path editing. It must also cover:

- interrupted saves
- template switching after content entry (content preserved, pagination may shift)
- theme changes after content entry
- hidden sections
- imported incomplete data
- multiple-tab edit conflict
- preview-vs-export parity
- entitlement gating on preview/export

## 11. Risks

### Risk 1 — Builder Too Generic

If the builder behaves like a basic form, the platform will be hard to differentiate and difficult to extend with smart features.

### Risk 2 — Preview Not Trusted

If users feel they must export repeatedly to "see the real result," the experience becomes fragile and frustrating.

### Risk 3 — Save State Too Weak

If users cannot trust persistence, premium conversion is less meaningful because the underlying product feels unsafe.

### Risk 4 — Import Path Too Fragile

If import exists but is poor, the product disappoints an important user segment and wastes a high-intent entry path.

### Risk 5 — Design Breaks ATS

If customization is too free (tables, graphics, non-standard headings), exported resumes may parse poorly, contradicting the "ATS-friendly" promise and damaging trust.

### Risk 6 — Content/Design Coupling

If content and design are tightly coupled in the data model, template switching, theming, and premium layout features become expensive and fragile to build later.

## 12. Recommended MVP Boundaries

### Must Have

- core sections (contact, summary, experience, education, skills)
- template-first start
- import-to-review-to-draft path
- save/autosave confidence
- live or near-live preview
- duplicate/tailor flow
- content/style separation (template switching without content loss)

### Should Have

- deeper section variety (projects, certifications, languages, volunteering)
- richer layout customization within ATS-safe bounds
- document views + basic tags/labels
- multilingual support beyond basic needs

### Could Have

- user-visible version history
- advanced custom sections
- collaborative editing
- full theme system (backgrounds, rating styles, icons)

## 13. Open Decisions

- How much customization belongs in MVP without weakening ATS safety?
- Which optional sections are first-release versus later-release?
- Should duplicate/tailor be a visible primary action or a secondary management action?
- Do we model a reusable "content library" in MVP, or defer it (knowing it delays the tailoring advantage)?
- What is the document-count and page-count limit for MVP, and is it single-sourced across all surfaces?

## 14. Related Story Packs

- [resume-builder-frontend.US.md](resume-builder-frontend.US.md)
- [resume-builder-backend.US.md](resume-builder-backend.US.md)
- [template-import-frontend.US.md](template-import-frontend.US.md)
- [template-import-backend.US.md](template-import-backend.US.md)
