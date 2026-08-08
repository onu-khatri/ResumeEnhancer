# WebSolution.Server Project

This project is the API host for the application.

It should compose modules and infrastructure through dependency injection, expose HTTP endpoints, and read runtime configuration. Database migrations and seed execution are handled by `Infrastructure/Migration`, not by normal web startup.
