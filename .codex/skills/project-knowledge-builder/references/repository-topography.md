# ResumeEnhancer Repository Topography

Read this reference when the requested knowledge spans multiple layers or when you need a fast map of where to gather evidence.

## Main evidence zones

### Product intent

- `README.md`
- `Business-Requirements/`
- `User-Stories/`

Use these to understand business vocabulary, scope boundaries, and expected behaviors before inferring how the implementation should work.

### Backend implementation

- `application/WebSolution/WebSolution.Server/`
- `application/WebSolution/ModulesComposition/`
- `application/Modules/<ModuleName>/<ModuleName>ModuleWeb/`
- `application/Modules/<ModuleName>/<ModuleName>ModuleSL/`
- `application/Modules/<ModuleName>/<ModuleName>ModulePL/`
- `application/Modules/<ModuleName>/<ModuleName>ModuleDM/`
- `application/Infrastructure/`
- `application/Core/`

Use these to trace request flow, dependency direction, validation, mapping, persistence, and module composition.

### Frontend implementation

- `application/WebSolution/websolution.client/src/app/`
- `application/WebSolution/websolution.client/src/features/resume/`
- `application/WebSolution/websolution.client/src/shared/`
- `application/WebSolution/websolution.client/src/routes/`
- `application/WebSolution/websolution.client/src/pages/`

Use these to understand routing, feature boundaries, API usage, page composition, local state, and reusable UI patterns.

### Verification

- `test/ResumeEnhancer.Tests/`
- `test/IntegrationTest/`
- `test/TestUtilities/IntegrationSupport/`

Use these to validate intended behavior, dependency constraints, integration boundaries, and test helpers.

## Common cross-layer topics

- Resume CRUD flow
- Search and paging flow
- Minimal API endpoint conventions
- FluentValidation request rules
- Mediator contract and handler patterns
- Mapster mapping responsibilities
- Repository and unit-of-work usage
- Frontend form schema and draft state
- Integration test setup and fake authentication
- Requirement-to-implementation traceability

## Knowledge artifact target

Unless the user explicitly asks for another location, save generated knowledge artifacts under:

- `KnowledgeBase/`

Suggested file naming:

- `<topic-name>.knowledge.md` for completed knowledge
- `<topic-name>.draft.md` for temporary structural drafts