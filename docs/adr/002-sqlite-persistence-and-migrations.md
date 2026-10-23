# ADR-002: EF Core with file-based SQLite and versioned migrations

- **Date:** 2026-10-23
- **Status:** Accepted
- **Record type:** Retrospective record of the current implementation.

## Context and problem

The reference tutorial uses SQL Server, but the initial CRUD application needed to run locally without preparing an external server. Books also needed to survive restarts, and table creation had to be reproducible. Keeping objects only in memory would not provide the desired persistence.

## Decision drivers

- Simple local setup and durable data.
- An introduction to relational persistence with EF Core.
- Schema history tracked with the code and isolated tests.

## Options and trade-offs

Retrospective comparison; no load tests or provider equivalence studies were performed:

| Option | Benefit | Cost |
| --- | --- | --- |
| File-based SQLite with EF Core | Requires no server and preserves data | Has its own type, SQL, and concurrency characteristics |
| SQL Server from the tutorial | Keeps the original provider | Requires setting up and administering an external instance |
| A list or in-memory-only database | Simplifies a short demonstration | Does not preserve data after application shutdown |

## Decision

Use `BookStoreContext` with the SQLite provider and a configurable connection in `ConnectionStrings:Books`. The default is a `books.db` file, with relative paths resolved against the API content root. The local database is not committed.

Version migrations and the snapshot in `Data/Migrations`, with `dotnet-ef` in the local tool manifest. In Development, the application applies migrations during startup. In other environments, migrations are run separately before starting the API.

The service coordinates persistence through EF Core. POST and PUT use `SaveChangesAsync`; DELETE uses `ExecuteDeleteAsync`, without an additional save step.

## Consequences

**Positive:** the example works without external infrastructure, preserves data across runs, and provides a reproducible schema history. Tests can use the same provider with temporary files.

**Negative:** migrations and behavior depend on the provider; another database would require reviewing queries, schema, and tests. Automatic migrations are a development convenience, not a production deployment strategy.

## References

- [Context and relational configuration](../../src/BookStore.Api/Data/BookStoreContext.cs)
- [EF CLI manifest](../../.config/dotnet-tools.json)
- [ADR-004: isolated tests](004-isolated-integration-tests.md)
