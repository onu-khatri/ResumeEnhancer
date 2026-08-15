# ResumeEnhancer.Core.CommonLibrary Project

This project is reserved for general-purpose shared helpers that are not domain model concepts.

Domain base classes live in `Core/DomainLibrary`, not here. Use ResumeEnhancer.Core.CommonLibrary for cross-cutting utility code such as small extensions, exception helpers, or reusable value helpers that do not belong to infrastructure or a specific module.

## Dependency Rule

ResumeEnhancer.Core.CommonLibrary should stay lightweight and dependency-conscious. Avoid references to infrastructure projects, module projects, or application host projects.

## Current Shared Helpers

No shared helpers are currently required here. Keep ASP.NET Core result formatting in `Core/WebLibrary`, not here.

