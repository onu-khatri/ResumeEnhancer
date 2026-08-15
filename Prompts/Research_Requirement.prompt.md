`Resume-Builer-Websites`: https://zety.com/, https://www.resume-now.com/, https://novoresume.com/

# Master Research & Product Requirements Prompt — Build an `Resume-Builer-Websites`-Like Resume Platform

## Role

Act as a **Principal Product Manager, Senior Business Analyst, UX Researcher, Solution Architect, Technical Product Owner, QA Lead, and Competitive Product Analyst**.

Your task is to perform an **extremely deep, end-to-end analysis of `Resume-Builer-Websites`.com**, specifically its resume-building ecosystem and all adjacent tools, and convert the findings into a **production-ready Product Requirements / Business Requirements document** that can be used by Product, UX/UI, Frontend, Backend, QA, DevOps, Security, and Architecture teams to build a functionally comparable resume platform.

The objective is **not to copy `Resume-Builer-Websites`'s proprietary code, copyrighted text, branding, or protected visual assets**. Instead, study publicly accessible product behavior, workflows, information architecture, feature concepts, interaction patterns, and business capabilities so that an independent product with comparable functionality can be designed.

---

# 1. Research Objective

Go through **`Resume-Builer-Websites`.com in maximum possible depth**.

Do not restrict the analysis to the landing page.

Explore and document all publicly accessible product areas relevant to:

* Resume Builder
* Resume Templates
* Resume Examples
* Resume Checker
* ATS Resume Checker
* Resume Review
* Resume Scoring
* Resume Optimization
* AI Resume Writing
* AI Content Generation
* AI Rewrite
* Resume Suggestions
* Job-tailored resumes
* Job Description matching
* Cover Letter Builder
* Cover Letter Examples
* CV Builder
* CV Templates
* Personal Branding features
* Career-related tools
* Job search-related tools
* Resume Sections
* Resume Customization
* Resume Formatting
* Resume Export
* Resume Import
* LinkedIn-related capabilities
* Existing Resume Upload
* PDF/DOCX parsing
* Profile management
* Workspace/Dashboard
* Document management
* Subscription plans
* Free vs paid functionality
* Billing
* Trial behavior
* Upgrade prompts
* Authentication
* Authorization
* Account management
* User preferences
* Sharing
* Public/private resume links
* Analytics, if available
* Integrations
* Help/support
* Onboarding
* Notifications
* AI features
* Administrative capabilities that can reasonably be inferred as necessary to operate the product

Search publicly accessible pages, documentation, help center content, FAQs, pricing pages, product pages, template directories, examples, onboarding flows, editor screens, account areas where accessible, and other relevant public information.

When a feature cannot be directly verified, clearly label it:

**Observed / Documented / Inferred / Recommended**

Never present an inferred feature as an observed fact.

---

# 2. Research Depth

Perform analysis at multiple levels.

## Level 1 — Product Portfolio

Identify every major:

* Product
* Module
* Tool
* Utility
* Resume feature
* Career feature
* AI feature
* Content resource
* Monetization mechanism

Build a complete capability map.

## Level 2 — Module Analysis

For every module identify:

* Purpose
* Target user
* Entry point
* Preconditions
* Primary workflow
* Alternative workflows
* Data captured
* Data generated
* Actions available
* Validations
* Error conditions
* Empty states
* Loading states
* Success states
* Navigation
* Permissions
* Upgrade restrictions
* Integrations
* Outputs

## Level 3 — Screen Analysis

For each major screen identify:

* Screen name
* Purpose
* URL/navigation path if publicly observable
* Header
* Sidebar
* Main workspace
* Primary CTA
* Secondary CTAs
* Forms
* Buttons
* Menus
* Dropdowns
* Modals
* Drawers
* Tooltips
* Toast messages
* Search
* Filters
* Pagination
* Tabs
* Accordions
* Empty states
* Loading states
* Validation states
* Error states
* Responsive behavior
* Desktop behavior
* Mobile behavior
* Accessibility considerations

## Level 4 — Interaction Analysis

Analyse interactions such as:

* Click
* Double-click
* Drag and drop
* Reorder
* Resize
* Inline editing
* Autosave
* Undo
* Redo
* Keyboard actions
* Copy/paste
* Duplicate
* Delete
* Restore
* Preview
* Zoom
* Page navigation
* Section navigation
* Template switching
* Theme switching
* Color selection
* Font selection
* Formatting controls

## Level 5 — Backend Behaviour

For every capability infer the likely backend requirements, including:

* API
* Database
* Storage
* Authentication
* Authorization
* Search
* Resume rendering
* PDF generation
* DOCX generation
* Parsing
* AI processing
* Background jobs
* Analytics
* Notifications
* Payments
* Subscription management
* Audit logging
* Version history
* ResumeEnhancer.Infrastructure.Caching
* Rate limiting
* Security controls

---

# 3. Competitive Product Decomposition

Create a hierarchical feature tree similar to:

```text
Resume Platform
│
├── Authentication
│   ├── Registration
│   ├── Login
│   ├── Social Login
│   ├── Password Reset
│   ├── Email Verification
│   └── Session Management
│
├── User Management
│
├── Dashboard
│
├── Resume Builder
│   ├── Create Resume
│   ├── Resume Editor
│   ├── Sections
│   ├── Templates
│   ├── Styling
│   ├── AI Assistance
│   ├── Preview
│   └── Export
│
├── Resume Checker
│
├── ATS Optimization
│
├── Job Description Matching
│
├── Cover Letter Builder
│
├── AI Services
│
├── Billing
│
├── Administration
│
└── Platform Services
```

Expand this hierarchy to **at least 4–5 levels wherever appropriate**.

---

# 4. Resume Builder — Maximum Depth Analysis

Analyse the resume builder as the core application.

Document the complete resume lifecycle:

**Start → Create → Add Content → Edit → Customize → AI Enhancement → Validate → Preview → Save → Export → Share → Duplicate → Update → Delete**

Analyse all supported resume sections that can be identified, such as:

* Personal Information
* Header
* Summary
* Professional Summary
* Objective
* Experience
* Education
* Skills
* Projects
* Certifications
* Courses
* Languages
* Achievements
* Awards
* Publications
* Volunteering
* References
* Interests
* Strengths
* Passions
* Organizations
* Technologies
* Custom sections

For each section document:

* Fields
* Field datatype
* Mandatory/optional
* Min/max length
* Validation
* Formatting
* Sorting
* Reordering
* Multiple records
* Date handling
* Present/current state
* Suggestions
* AI assistance
* Visibility
* Delete behavior
* Empty behavior

---

# 5. Resume Editor Analysis

Reverse-engineer the conceptual architecture of the editor.

Identify requirements for:

### Editor Canvas

* Resume page
* Page dimensions
* Multiple-page handling
* Overflow behavior
* Zoom
* Preview
* Page breaks
* Dynamic pagination

### Editing

* Inline editing
* Section-level editing
* Rich text
* Bold
* Italic
* Underline
* Hyperlinks
* Bullets
* Alignment
* Date formatting

### Layout

* Single column
* Multi-column
* Section placement
* Section order
* Drag/drop
* Width control
* Spacing

### Appearance

* Templates
* Fonts
* Font sizes
* Colors
* Background
* Line spacing
* Margins
* Icons
* Section titles
* Accent colors

### State Management

* Autosave
* Save indicator
* Unsaved changes
* Undo
* Redo
* Version history
* Recovery

---

# 6. Template System

Analyse the complete template ecosystem.

Define requirements for:

* Template categories
* Template browser
* Template preview
* Template search
* Template filters
* Recommended templates
* Profession-specific templates
* Seniority-specific templates
* Modern/classic/creative categories
* ATS-friendly templates
* Free templates
* Premium templates

Determine how a template should technically work.

Specify a proposed model containing:

* Template ID
* Template version
* Layout schema
* Supported sections
* Default positions
* Typography configuration
* Theme configuration
* Spacing
* Header layout
* Column configuration
* Rendering rules
* PDF rules
* Print rules

---

# 7. AI Features

Perform a separate detailed investigation into AI functionality.

Consider:

* Resume summary generation
* Bullet generation
* Rewrite
* Improve wording
* Shorten
* Expand
* Grammar correction
* Tone improvement
* Achievement generation
* Skill recommendation
* Job-specific recommendation
* Job Description analysis
* Keyword extraction
* ATS keyword recommendations
* Experience suggestions
* Cover letter generation
* Career guidance

For each AI function specify:

* Trigger
* Input
* Context supplied to AI
* Prompt concept
* Output
* Loading state
* Retry
* Regenerate
* Accept
* Reject
* Replace
* Insert
* AI quota
* Usage tracking
* Error handling
* Content moderation
* PII handling
* Cost controls

---

# 8. Resume Checker / ATS Checker

Analyse how a resume-analysis subsystem should work.

Include requirements around:

* Resume upload
* Resume parsing
* Section detection
* Contact information
* Summary analysis
* Experience analysis
* Education analysis
* Skills analysis
* ATS compatibility
* Formatting
* Keyword analysis
* Impact
* Action verbs
* Repetition
* Grammar
* Spelling
* Readability
* Resume length
* Quantified achievements
* Missing sections

Define a scoring architecture such as:

```text
Overall Resume Score
├── Content Score
├── ATS Score
├── Formatting Score
├── Completeness Score
├── Impact Score
├── Skills Score
└── Job Match Score
```

Explain proposed scoring logic without falsely claiming `Resume-Builer-Websites`'s proprietary scoring algorithm.

---

# 9. Job Description Matching

Specify a complete job-tailoring system.

Workflow:

```text
User selects resume
→ adds Job Description
→ system parses JD
→ extracts role
→ extracts skills
→ extracts keywords
→ identifies responsibilities
→ compares resume
→ calculates match
→ identifies missing keywords
→ provides recommendations
→ user applies recommendations
→ score recalculated
```

Include FE and BE requirements for the entire lifecycle.

---

# 10. Authentication Requirements

Include full authentication lifecycle.

Must include:

* Registration
* Email/password registration
* Google authentication
* Other social authentication where relevant
* Email verification
* Login
* Logout
* Remember me
* Forgot password
* Reset password
* Password policies
* Session timeout
* Refresh token
* Multiple sessions
* Session revocation
* Account lock
* Failed login detection
* CAPTCHA/rate limiting
* MFA-ready architecture
* Account deletion
* Account recovery

---

# 11. User Management

Create requirements for:

* User profile
* Personal information
* Profile image
* Email
* Preferences
* Locale
* Timezone
* Notification settings
* Password
* Connected accounts
* Billing profile
* Subscription
* Privacy settings
* Download personal data
* Delete account

---

# 12. RBAC — Role-Based Access Control

Design a complete Role and Permission architecture.

At minimum consider:

* Guest
* Free User
* Premium User
* Support Agent
* Content Manager
* Finance User
* Admin
* Super Admin

Do not hard-code the system to only these roles.

Roles must be dynamically manageable.

Define:

```text
User
↓
Role Assignment
↓
Role
↓
Access Profile
↓
Permissions
↓
Resources / Actions
```

---

# 13. Access Profile

Create a configurable **Access Profile** system.

An Access Profile may contain permissions such as:

```text
resume.create
resume.read
resume.update
resume.delete
resume.export
resume.share

template.read
template.manage

ai.generate
ai.unlimited

checker.execute

billing.read
billing.manage

user.read
user.manage

role.read
role.manage

access_profile.read
access_profile.manage

admin.dashboard
```

Include:

* Create access profile
* Edit
* Clone
* Activate/deactivate
* Map permissions
* Assign to roles
* Remove
* Audit changes

Provide a complete permission matrix.

---

# 14. Subscription and Billing

Analyse and design:

* Free plan
* Premium plan
* Monthly
* Quarterly if applicable
* Annual
* Trials
* Discounts
* Promo codes
* Upgrade
* Downgrade
* Cancellation
* Renewal
* Failed payment
* Grace period
* Invoice
* Receipt
* Refund
* Tax
* Payment method
* Subscription status

Create feature-entitlement requirements.

Example:

```text
Feature                     Free      Premium
------------------------------------------------
Resume creation             Limited   Unlimited
Templates                    Limited   All
PDF export                   Limited   Yes
AI generation                Limited   Higher/Unlimited
Resume checking              Limited   Full
Cover letters                Limited   Full
```

Use researched evidence where available and clearly label recommendations/inferences.

---

# 15. Admin Application

Design a comprehensive administrative portal.

Cover:

* Admin Login
* Dashboard
* User Management
* Role Management
* Access Profiles
* Permission Management
* Templates
* Resume Examples
* Cover Letter Examples
* AI Prompt Management
* AI Models
* Feature Flags
* Subscription Plans
* Pricing
* Coupons
* Transactions
* Refunds
* Content
* Notifications
* Analytics
* System Configuration
* Audit Logs
* Support operations

---

# 16. Non-Functional Requirements

Create separate NFR requirements covering:

### Performance

* Page-load performance
* Editor responsiveness
* Autosave latency
* PDF generation performance
* AI response experience

### Scalability

Design for:

* 100 users
* 10,000 users
* 100,000 users
* 1M+ users

### Availability

Suggest target SLA/SLO values.

### Security

Include:

* OWASP Top 10
* XSS
* CSRF
* SQL injection
* SSRF
* Authentication attacks
* Authorization checks
* Encryption at rest
* TLS
* Password hashing
* Secret management
* Rate limiting
* Abuse prevention
* Audit logging

### Privacy

Consider:

* GDPR
* CCPA
* Consent
* Data deletion
* Data export
* PII
* AI-processing consent
* Retention

### Accessibility

Target:

**WCAG 2.2 AA**

### Compatibility

Cover:

* Chrome
* Edge
* Firefox
* Safari
* Desktop
* Tablet
* Mobile

---

# 17. System Architecture

Recommend a high-level architecture.

Cover:

```text
Web Application
       ↓
API Gateway / Backend
       ↓
------------------------------------------------
Auth Service
User Service
Resume Service
Template Service
Rendering Service
AI Service
Resume Parser
ATS Analysis Service
Job Match Service
Cover Letter Service
Billing Service
Notification Service
Admin Service
Analytics Service
------------------------------------------------
       ↓
Databases / Cache / Object Storage / Queue
```

Explain whether a modular monolith or microservices architecture is more appropriate for:

* MVP
* Growth
* Enterprise scale

---

# 18. Data Model

Propose an entity/data model containing at least:

* User
* AuthenticationIdentity
* Session
* Role
* Permission
* AccessProfile
* RolePermission
* UserRole
* Subscription
* Plan
* PlanFeature
* Entitlement
* Resume
* ResumeVersion
* ResumeSection
* ResumeSectionItem
* Template
* TemplateVersion
* Theme
* Export
* ShareLink
* ResumeAnalysis
* ATSAnalysis
* JobDescription
* JobMatch
* AIRequest
* AIUsage
* CoverLetter
* Payment
* Invoice
* Coupon
* Notification
* AuditLog
* FeatureFlag
* SystemSetting

For each major entity provide key fields and relationships.

---

# 19. API Catalogue

Produce a proposed REST or GraphQL API catalogue.

Example:

```text
POST   /auth/register
POST   /auth/login
POST   /auth/logout
POST   /auth/refresh
POST   /auth/forgot-password
POST   /auth/reset-password

GET    /users/me
PATCH  /users/me

GET    /resumes
POST   /resumes
GET    /resumes/{id}
PATCH  /resumes/{id}
DELETE /resumes/{id}

POST   /resumes/{id}/duplicate
POST   /resumes/{id}/export
POST   /resumes/{id}/analyse

GET    /templates

POST   /ai/generate
POST   /ai/rewrite

POST   /job-match
```

Expand this to cover the complete platform.

For each important endpoint include:

* Method
* Endpoint
* Purpose
* Authentication
* Permission
* Main request fields
* Main response
* Common error codes

---

# 20. User Story Requirement

This is a **critical deliverable**.

Generate **at least 120–150 detailed User Stories**.

Do NOT stop at exactly 100.

Target **150+ stories where justified**.

Every User Story must have a unique ID.

Example:

```text
AUTH-FE-001
AUTH-BE-001
RES-FE-001
RES-BE-001
AI-FE-001
AI-BE-001
```

---

# 21. Frontend and Backend Separation

Separate stories into:

## FRONTEND USER STORIES

and

## BACKEND USER STORIES

Do not mix FE and BE stories.

A business capability should normally have corresponding FE and BE stories where applicable.

Example:

### FE

**AUTH-FE-001 — User Login Interface**

As a registered user,
I want to log into the platform using my email and password,
so that I can access my resume workspace.

### BE

**AUTH-BE-001 — Authenticate User Credentials**

As the authentication service,
I want to validate submitted credentials,
so that authenticated users can securely access protected resources.

---

# 22. Mandatory User Story Domains

Generate User Stories covering at least:

1. Authentication
2. Registration
3. Social Login
4. Email Verification
5. Forgot Password
6. Reset Password
7. Session Management
8. User Profile
9. User Preferences
10. Role Management
11. Permission Management
12. Access Profile Management
13. Authorization
14. Dashboard
15. Resume Creation
16. Resume Editing
17. Resume Sections
18. Experience
19. Education
20. Skills
21. Projects
22. Certifications
23. Custom Sections
24. Reordering
25. Autosave
26. Undo/Redo
27. Resume Templates
28. Template Switching
29. Resume Appearance
30. Fonts
31. Colors
32. Layout
33. Resume Preview
34. PDF Export
35. DOCX Export
36. Resume Import
37. Resume Parsing
38. Resume Duplication
39. Resume Deletion
40. Resume Sharing
41. AI Resume Generation
42. AI Rewrite
43. AI Suggestions
44. Resume Checker
45. ATS Checker
46. Resume Score
47. Job Description Analysis
48. Job Matching
49. Keyword Matching
50. Cover Letter
51. Billing
52. Subscription
53. Upgrade
54. Downgrade
55. Cancellation
56. Payments
57. Invoices
58. Notifications
59. Admin
60. User Administration
61. Template Administration
62. Content Administration
63. AI Administration
64. Feature Flags
65. Analytics
66. Audit Logging
67. Security
68. Privacy
69. Accessibility
70. Error Handling
71. Monitoring
72. Logging
73. Search
74. Performance
75. Account Deletion

Add other domains discovered during research.

---

# 23. User Story Format

Use the following format for **every User Story**.

## [Story ID] — [Story Title]

**Module:**
**Layer:** Frontend / Backend
**Priority:** Must Have / Should Have / Could Have
**Persona:**
**Dependency:**
**Source Classification:** Observed / Documented / Inferred / Recommended

### User Story

As a `<persona>`,
I want `<capability>`,
so that `<business value>`.

### Description

Explain the requirement in sufficient detail for Product, Development and QA.

### Preconditions

State conditions required before the scenario can begin.

### Acceptance Criteria

Use Given / When / Then format.

**AC1 — Happy Path**

Given ...
When ...
Then ...

**AC2 — Validation**

Given ...
When ...
Then ...

**AC3 — Error Handling**

Given ...
When ...
Then ...

**AC4 — Authorization**

Given ...
When ...
Then ...

Include additional acceptance criteria as necessary.

### Business Rules

List applicable rules.

### Validation Rules

Include:

* Required values
* Length
* Character restrictions
* Boundary cases
* Invalid states

### Error Handling

Specify expected behavior.

### Security Considerations

Specify relevant security controls.

### Analytics / Tracking

Specify relevant product events.

### API Dependencies

Specify likely endpoint/service requirements.

### Data Requirements

Identify key data persisted or retrieved.

### UX Notes

For FE stories include important interaction behavior.

### Definition of Done

Specify measurable completion conditions.

---

# 24. Acceptance Criteria Depth

Do not produce shallow acceptance criteria.

For each meaningful user story consider:

* Happy path
* Alternate path
* Invalid input
* Required validation
* Boundary conditions
* Permission failure
* Network failure
* Server error
* Empty state
* Loading
* Duplicate action
* Retry
* Timeout
* Mobile behavior
* Accessibility
* Analytics
* Security

Not every story needs every scenario, but use them when relevant.

---

# 25. Priority

Classify requirements using MoSCoW:

* Must Have
* Should Have
* Could Have
* Won't Have for MVP

Also propose:

### MVP

Capabilities required to launch a commercially viable first version.

### Phase 2

Capabilities that materially improve product-market fit.

### Phase 3

Advanced capabilities.

---

# 26. Persona Definition

Define personas before the user stories.

At minimum:

* Anonymous Visitor
* New User
* Job Seeker
* Student
* Experienced Professional
* Career Changer
* Premium Subscriber
* Administrator
* Support Agent
* Content Manager
* Finance Administrator
* Super Administrator

For each persona specify:

* Goal
* Pain points
* Key workflows
* Permissions
* Relevant features

---

# 27. User Journeys

Create detailed user journeys for:

### Journey 1

Visitor → Registration → First Resume → PDF Download

### Journey 2

User → Upload Existing Resume → Parse → Edit → Export

### Journey 3

User → Create Resume → AI Assistance → Final Resume

### Journey 4

User → Resume Checker → Recommendations → Improvement

### Journey 5

User → Add Job Description → Job Match → Tailor Resume

### Journey 6

Free User → Encounters Premium Feature → Upgrade → Payment

### Journey 7

User → Generate Cover Letter → Customize → Download

### Journey 8

Admin → Create Template → Publish Template

### Journey 9

Admin → Create Role → Create Access Profile → Assign User

Represent complex flows using Mermaid diagrams where appropriate.

---

# 28. Screen Inventory

Produce a complete proposed screen inventory.

Example:

```text
PUBLIC
PUB-001 Homepage
PUB-002 Pricing
PUB-003 Templates
PUB-004 Resume Examples
PUB-005 Cover Letter Examples

AUTH
AUTH-001 Login
AUTH-002 Register
AUTH-003 Forgot Password

APP
APP-001 Dashboard
APP-002 Resume Builder
APP-003 Template Selector
APP-004 Resume Checker

SETTINGS
SET-001 Profile
SET-002 Subscription

ADMIN
ADM-001 Dashboard
ADM-002 Users
ADM-003 Roles
...
```

For each screen document:

* Purpose
* Persona
* Main components
* Main actions
* Permissions
* Related User Stories

---

# 29. Frontend Component Inventory

Create a reusable UI component catalogue.

Include components such as:

* Header
* Sidebar
* Breadcrumb
* Resume Card
* Template Card
* Editor Canvas
* Section Editor
* Rich Text Editor
* Drag Handle
* Color Picker
* Font Selector
* Modal
* Drawer
* Dropdown
* Tooltip
* Toast
* Alert
* Progress
* Score Widget
* Subscription Paywall
* Skeleton Loader
* Empty State
* Confirmation Dialog

Map components to related screens.

---

# 30. State Matrix

For major components/screens define:

* Default
* Hover
* Focus
* Active
* Disabled
* Loading
* Empty
* Error
* Success
* Permission denied
* Premium locked
* Offline
* Retry

---

# 31. Event Tracking

Create an analytics event taxonomy.

For example:

```text
account_created
login_success
resume_created
resume_section_added
resume_section_removed
resume_template_changed
ai_suggestion_requested
ai_suggestion_accepted
resume_export_started
resume_export_completed
resume_checker_started
resume_checker_completed
job_match_started
subscription_checkout_started
subscription_activated
subscription_cancelled
```

For every major event identify:

* Event
* Trigger
* User
* Resume ID
* Template ID
* Plan
* Metadata

---

# 32. Edge Cases

Create a dedicated edge-case catalogue.

Consider:

* User loses internet during editing
* Autosave fails
* AI service unavailable
* Resume exceeds 10 pages
* Unsupported characters
* Emoji
* RTL language
* Very long company name
* Missing dates
* Overlapping dates
* Resume parser fails
* PDF corrupted
* DOCX corrupted
* Unsupported file
* File too large
* Duplicate payment callback
* Browser closes during payment
* User subscription expires while editor is open
* Premium template becomes unavailable
* Export fails
* Session expires
* Multiple tabs modify same resume
* Resume deleted from another device

---

# 33. QA Coverage

Create a testing strategy covering:

* Unit Testing
* Component Testing
* API Testing
* Integration Testing
* E2E Testing
* Visual Regression
* Cross-browser
* Accessibility
* Performance
* Security
* Load Testing
* Payment Testing
* AI Testing
* Resume rendering validation

Identify the highest-risk E2E scenarios.

---

# 34. Research Evidence

For every major capability discovered from `Resume-Builer-Websites` provide:

* Feature
* Source URL
* Page/title
* Observation
* Classification
* Date accessed

Do not invent URLs.

Distinguish clearly between:

**What `Resume-Builer-Websites` demonstrably provides**

and

**What the proposed platform should provide**

---

# 35. Competitor-Inspired vs Original Requirements

Use the following markers:

**[OBSERVED]** — directly observed in `Resume-Builer-Websites`
**[DOCUMENTED]** — explicitly stated by `Resume-Builer-Websites`
**[INFERRED]** — necessary technical/product capability inferred from behavior
**[RECOMMENDED]** — capability recommended for our implementation

This distinction must remain visible throughout the report.

---

# 36. Final Document Structure

Produce the final output in this structure:

## 1. Executive Summary

## 2. Research Methodology

## 3. `Resume-Builer-Websites` Product Overview

## 4. Product Capability Map

## 5. Feature Inventory

## 6. Information Architecture

## 7. Personas

## 8. User Journeys

## 9. Screen Inventory

## 10. Resume Builder Analysis

## 11. Resume Editor Analysis

## 12. Resume Section Architecture

## 13. Template System

## 14. Resume Import & Parsing

## 15. Resume Export & Rendering

## 16. AI Capabilities

## 17. Resume Checker

## 18. ATS Analysis

## 19. Job Description Matching

## 20. Cover Letter System

## 21. Authentication

## 22. User Management

## 23. Roles

## 24. Permissions

## 25. Access Profiles

## 26. Subscription & Billing

## 27. Admin Portal

## 28. Notifications

## 29. Analytics

## 30. Audit & Compliance

## 31. Proposed Architecture

## 32. Data Model

## 33. API Catalogue

## 34. Frontend Component Inventory

## 35. Frontend User Stories

Target: **70–100+ FE stories**

## 36. Backend User Stories

Target: **70–100+ BE stories**

Total target: **150+ detailed stories**

## 37. Acceptance Criteria

Acceptance criteria should be embedded in individual stories.

## 38. Non-Functional Requirements

## 39. Security Requirements

## 40. Privacy Requirements

## 41. Accessibility Requirements

## 42. Error & Edge Cases

## 43. Analytics Event Catalogue

## 44. QA Strategy

## 45. MVP Scope

## 46. Phase 2

## 47. Phase 3

## 48. Requirement Traceability Matrix

## 49. Assumptions & Open Questions

## 50. Research Sources

---

# 37. Requirement Traceability Matrix

Create a table connecting:

| Requirement | Module | Screen | FE Story | BE Story | API | Entity | Priority | Phase |
| ----------- | ------ | ------ | -------- | -------- | --- | ------ | -------- | ----- |

Every major capability should be traceable.

---

# 38. Feature Matrix

Provide a matrix:

| Module | Feature | `Resume-Builer-Websites` Observed | Proposed Product | MVP | Phase 2 | Phase 3 |
| ------ | ------- | ---------------: | ---------------: | --: | ------: | ------: |

---

# 39. FE/BE Mapping Matrix

Provide:

| Business Capability | FE Story IDs | BE Story IDs | API | Data Entities |
| ------------------- | ------------ | ------------ | --- | ------------- |

This is important for estimating implementation.

---

# 40. User Story Quantity and Quality Gate

Before finalizing, perform a self-audit.

The document is incomplete if:

* Fewer than **120 User Stories** exist.
* FE and BE stories are not separated.
* Authentication stories are missing.
* Role stories are missing.
* Permission stories are missing.
* Access Profile stories are missing.
* Admin stories are missing.
* Resume Builder stories are shallow.
* AI stories are missing.
* ATS stories are missing.
* Subscription stories are missing.
* Every significant story does not contain Acceptance Criteria.
* Acceptance Criteria are merely one-line statements.
* Requirement traceability is missing.

Prefer **150–200 high-quality stories** if the analysis supports them.

Do not inflate the story count by splitting trivial UI controls into meaningless stories.

---

# 41. Product Build Recommendation

After requirements analysis, recommend a product implementation strategy.

Include:

### MVP

What should be built first.

### Build Order

For example:

```text
Foundation
→ Authentication/RBAC
→ User Workspace
→ Resume Data Model
→ Resume Editor
→ Template Renderer
→ PDF Export
→ AI
→ Resume Checker
→ Job Matching
→ Billing
→ Admin
→ Advanced Features
```

Explain dependencies.

---

# 42. Suggested Technology Architecture

Without treating this as mandatory, recommend an appropriate modern stack for implementing the product.

Evaluate options for:

### Frontend

* React / Next.js
* TypeScript
* State management
* Rich-text editing
* Drag/drop
* Real-time preview

### Backend

* Node.js/NestJS
* Java/Spring Boot
* .NET
* Python

Recommend one based on the requirements.

### Storage

Evaluate:

* PostgreSQL
* Redis
* Object Storage

### AI

Recommend an AI abstraction layer so providers/models can be changed without rewriting application logic.

### Document Rendering

Evaluate approaches for:

* HTML → PDF
* Headless Chrome
* Server-side rendering
* DOCX generation

### Infrastructure

Include:

* CDN
* API gateway
* queues
* caching
* object storage
* monitoring
* logs
* secrets
* CI/CD

---

# 43. Important Research Rules

Do not merely paraphrase marketing text.

Think like a product engineering team that must actually **build, test, deploy, operate, monetize, and scale** the platform.

When examining a feature ask:

1. What does the user see?
2. What can the user do?
3. What data is required?
4. What validation is required?
5. What API is required?
6. What backend process is required?
7. What data must be persisted?
8. What permissions apply?
9. What happens when something fails?
10. What happens on mobile?
11. What analytics should fire?
12. Is this free or premium?
13. What security risks exist?
14. How is this tested?
15. What admin capability is required to operate it?

Do not leave important hidden/backend functionality unspecified simply because it is not visible in the UI.

---

# 44. Quality Expectations

The output must be usable by:

* Founder
* Product Manager
* Business Analyst
* UX Designer
* UI Designer
* Frontend Engineer
* Backend Engineer
* Solution Architect
* QA Engineer
* DevOps Engineer
* Security Engineer
* Project Manager

Avoid generic statements such as:

> “The user should be able to manage their resume.”

Instead specify precisely what **manage** means.

Example:

> The authenticated user can create, rename, duplicate, preview, edit, archive, restore, delete, export and share resumes owned by their account, subject to plan entitlements and authorization rules.

---

# 45. Do Not Skip Hidden Platform Requirements

Public UI analysis will not expose every backend capability.

Explicitly derive requirements for:

* Authentication tokens
* Authorization
* Object ownership
* Tenant/user isolation
* Autosave concurrency
* Optimistic locking
* Resume versions
* Background processing
* Queues
* Retry mechanisms
* Idempotency
* Webhooks
* Payment reconciliation
* AI usage accounting
* Plan entitlement evaluation
* Rate limiting
* Abuse prevention
* File virus scanning
* PDF storage
* Secure signed URLs
* Audit trails
* Soft deletion
* Data retention
* Backups
* Disaster recovery
* Metrics
* Alerts
* Feature flags
* Configuration

Mark these as **[INFERRED]** or **[RECOMMENDED]** unless explicitly documented.

---

# 46. Final Deliverable Standard

The final output should resemble a combined:

* Competitive Product Analysis
* Product Requirements Document
* Business Requirements Document
* Software Requirements Specification
* UX Functional Specification
* Technical Architecture Specification
* API Specification
* User Story Backlog
* QA Acceptance Specification

It should contain enough detail that the product can subsequently be:

**estimated → architected → designed → broken into sprints → implemented → tested → launched.**

Do not provide a superficial overview.

Go to **nth-level functional depth** while maintaining clear organization, traceability, and distinction between evidence and inference.

