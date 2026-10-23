# ADR-004: HTTP integration tests with isolated temporary SQLite databases

- **Date:** 2026-10-23
- **Status:** Accepted
- **Record type:** Retrospective record of the current implementation.

## Context and problem

CRUD behavior depends on routing, binding, the filter, the service, and EF Core working together. Testing only isolated methods would not verify that composition. We also needed to demonstrate persistence across restarts without changing the database used during project demonstrations.

## Decision drivers

- Verify observable responses and effects through HTTP.
- Run without an external server or shared data.
- Exercise application migrations and the relational provider.
- Repeat tests without depending on scenario order.

## Options and trade-offs

Retrospective comparison; no experiment compared execution times between strategies:

| Option | Benefit | Cost |
| --- | --- | --- |
| WebApplicationFactory with temporary SQLite | Exercises actual project composition and persistence | Requires application startup and disk access |
| Isolated tests with service or context mocks | Focuses on a unit with less infrastructure | Does not cover the complete HTTP and relational flow |
| Only manual Swagger testing | Supports exploration and explanation | Requires human execution to detect regressions |

## Decision

Use xUnit and `WebApplicationFactory<Program>` to send requests to an in-memory test server. The database is real SQLite in a temporary file; an in-memory test server does not make persistence volatile.

`BookApiFactory` starts in Development and supplies a unique connection per scenario, running application migrations. It deletes files it creates; when a path is explicitly supplied, the test owns cleanup. The restart scenario shares a file only between that scenario's instances.

Tests verify CRUD, `Location`, rejected input without writes, missing resources, persistence across restarts, and Swagger documentation. `.http` examples remain available for manual demonstrations.

## Consequences

**Positive:** integration regressions are detected automatically; tests do not depend on the local database and exercise the provider used by the example.

**Negative:** startup and disk access cost more than tests of pure functions. Current Development scenarios do not demonstrate deployment, TLS, or complete production configuration. Another provider would require reviewing the test environment.

## References

- [Test factory](../../tests/BookStore.Api.Tests/BookApiFactory.cs)
- [HTTP scenarios](../../tests/BookStore.Api.Tests/BookEndpointsTests.cs)
- [Test guidance](../../tests/AGENTS.md)
