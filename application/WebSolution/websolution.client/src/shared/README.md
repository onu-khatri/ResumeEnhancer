# Shared Frontend Contract

`shared` contains presentation primitives and framework-neutral helpers only.
It must not import feature code, module API clients, business entities, feature
stores, authorization rules, or entitlement decisions.

Features own their API client, response-to-view-model mapping, authorization and
entitlement interpretation, route composition, and workflow state. A feature
passes presentation-ready values and callbacks to shared components.

Shared utilities are limited to cross-feature behavior: class-name composition,
formatting, normalization, safe browser storage, and error-state normalization.
Add a utility only when at least two features need it or it enforces a
foundation-wide rule.
