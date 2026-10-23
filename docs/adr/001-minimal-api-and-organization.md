# ADR-001: .NET 8 Minimal API with separated responsibilities

- **Date:** 2026-10-23
- **Status:** Accepted
- **Record type:** Retrospective record of the current implementation.

## Context and problem

The project started as a book CRUD implementation and explanation based on a .NET 8 Minimal API tutorial. The SDK was already available. We needed to keep the relationship between an HTTP request and its operation visible, without concentrating persistence, validation, and responses in the startup file.

## Decision drivers

- Simple setup and reading for developers learning C# and ASP.NET Core.
- Enough separation to locate HTTP handling, contracts, and persistence.
- Few projects and dependencies for a catalog of this scope.

## Options and trade-offs

Retrospective comparison, without experimental evaluation of alternatives:

| Option | Benefit | Cost |
| --- | --- | --- |
| Minimal APIs in one project with folders by responsibility | Exposes the HTTP flow with a small structure | Internal boundaries depend on conventions |
| All CRUD code in `Program.cs` | Fewer files to start with | Mixes startup and operation behavior |
| Controllers and separate projects by layer | Offers other ways to organize and isolate dependencies | Adds structure to an introductory example |

## Decision

Use .NET 8 Minimal APIs in `BookStore.Api`. `Program.cs` composes the application. `Endpoints` handles HTTP and calls `IBookService`; `Services` coordinates catalog operations using contracts, models, and the context. `Data` handles relational mapping. `Models` and `Contracts` do not depend on endpoints.

This separation lets readers follow an operation without introducing a generic repository or additional projects. .NET 8 records the tutorial and initial environment baseline; it is not a decision to keep that version indefinitely.

## Consequences

**Positive:** each responsibility has an identifiable location; handlers remain short; injection connects the parts without constructing them inside endpoints.

**Negative:** there are more files than in the smallest example; a single assembly does not prevent inappropriate references; the service remains coupled to EF Core and catalog contracts.

Future reorganization requires an observed problem and another decision, without a predetermined architecture migration.

## References

- [Application composition](../../src/BookStore.Api/Program.cs)
- [Implementation guidance](../../src/AGENTS.md)
- [ADR-002: persistence](002-sqlite-persistence-and-migrations.md)
