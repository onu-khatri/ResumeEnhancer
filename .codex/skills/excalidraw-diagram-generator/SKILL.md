---
name: excalidraw-diagram-generator
description: Generate clear architecture and workflow diagrams for ResumeEnhancer that can be rendered in Excalidraw or translated into diagram assets later. Use when Codex needs to visualize modules, flows, integrations, or story delivery plans.
---

# Excalidraw Diagram Generator

Use this skill to turn architecture or workflow understanding into diagram-ready structure.

## Use this skill when

- a module, request flow, or story delivery plan needs visual explanation
- the audience benefits from a diagram over prose

## Do not use this skill when

- a short written description is clearer than a diagram
- the structure is too unstable to represent faithfully

## Workflow

1. Confirm the diagram's purpose and audience (architecture, request flow, sequence, or delivery plan).
2. Ground the diagram in repository facts: read the actual modules, endpoints, and call paths first.
3. Map nodes, grouped boundaries, and directional flows using ResumeEnhancer terminology.
4. Label observed facts separately from inferred relationships.
5. Emit Excalidraw-compatible JSON when a renderable asset is requested.

## Output requirements

- nodes with concise labels
- grouped boundaries that match real modules (`<ModuleName>ModuleWeb`, `<ModuleName>ModuleSL`, `<ModuleName>ModulePL`, `<ModuleName>ModuleDM`, frontend features)
- directional flows with a clear start and end
- notes on what is observed versus inferred