## 1. Current-State Analysis And Test Baseline

- [x] 1.1 Confirm and document the current routes, resume shell, shared UI, shared utilities, template options, and existing theme behavior against the `frontend-foundation` spec.
- [x] 1.2 Add the frontend test runner, browser-like component-test environment, and coverage command to the client workspace.
- [x] 1.3 Define the measured `src` application-source boundary and version-controlled exclusions for generated code, type-only declarations, and test/support files.
- [ ] 1.4 Configure the frontend coverage gate to require 92% lines, branches, functions, and statements, and verify it fails on an intentionally uncovered measured branch.

## 2. Shell, Tokens, And Themes

- [x] 2.1 Define the shared route-context model for public pages, authenticated workspaces, and transient task surfaces, including route metadata and unsaved-work handling expectations.
- [x] 2.2 Define and implement shared typography, spacing, semantic-color, surface, border, motion, and breakpoint tokens, including the approved-exception rule.
- [x] 2.3 Implement multiple supported color themes with persisted same-device selection and a documented default fallback.
- [ ] 2.4 Migrate the existing resume shell, navigation, and shared status treatments to the token and theme contract, with accessibility and responsive tests.

## 3. Common Components And Utilities

- [x] 3.1 Evolve the existing button, input, text-area, dropdown, and status components into token-driven, accessible common components without moving feature logic into `shared`.
- [x] 3.2 Implement accessible common radio-button, checkbox, autocomplete, and removable-text-pill components with normal, focus, disabled, invalid, and selected state coverage.
- [x] 3.3 Implement a reusable pagination component and a sortable action-list component with accessible sort state, row actions, and a fixed scrolling header.
- [x] 3.4 Define and implement the initial shared utilities for class-name composition, display formatting, form-value normalization, safe storage access, and error normalization, including documented fallback behavior.
- [ ] 3.5 Add exhaustive unit and component tests for each common component and utility, preserving the 92% measured-source coverage gate.

## 4. Shared Resume Templates

- [x] 4.1 Define a template registry that maps supported template identifiers to presentation renderers while preserving one normalized resume-information model and allowing a future module-owned template catalog to supply metadata.
- [x] 4.2 Implement the template dropdown using the common dropdown component and preserve the selected template in the resume editing flow.
- [x] 4.3 Implement the supported template renderers so each renders the same resume information with its own presentation layout.
- [x] 4.4 Add tests that verify template selection, template switching, information preservation, renderer output for common fixture data, and catalog-metadata mapping.

## 5. Module-Ready Feature Integration

- [x] 5.1 Define and document the frontend feature-adapter convention: module API clients, response mapping, authorization and entitlement interpretation, and workflow state are feature-owned; shared components receive presentation-ready state and callbacks.
- [x] 5.2 Implement a profile-preference integration boundary that can reconcile a future authenticated preference source with the same-device theme fallback without coupling shared theming to a module client.
- [x] 5.3 Implement and test module-to-shared-state mappings for loading, empty, authorization, entitlement, offline, and error conditions without leaking transport details into common components.
- [x] 5.4 Verify the feature-sliced directory and import boundaries prevent `shared` from importing module-specific feature, API-client, or business-state code.

## 6. Adoption And Verification

- [ ] 6.1 Verify the foundation against the specified accessibility, responsive, navigation, theme, template, component, utility, module-integration, and UI-state scenarios.
- [ ] 6.2 Run the frontend check and coverage commands, record the final 92% coverage output, and resolve all measured-source failures.
- [x] 6.3 Update future frontend story templates or references so new UI work points back to the approved foundation instead of redefining baseline behavior.
