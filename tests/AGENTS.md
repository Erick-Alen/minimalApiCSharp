# Test guidance

## Scope and map

Applies to `tests/` and its descendants, alongside the root `AGENTS.md`.

| Area within `BookStore.Api.Tests/` | Responsibility |
| --- | --- |
| `BookStore.Api.Tests.csproj` | xUnit project referencing `src/BookStore.Api` and using `Microsoft.AspNetCore.Mvc.Testing`. |
| `BookApiFactory.cs` | API startup with an in-memory test server and a temporary SQLite database file. |
| `BookEndpointsTests.cs` | HTTP verification of CRUD, validation, missing resources, persistence, and Swagger. |

Tests exercise the application pipeline, endpoints, services, and relational provider used in `src/`.
The teaching guide in `docs/` uses these behaviors as examples; tests should verify observable results, not documentation wording.

## Isolation and behavior

- Use a unique temporary database per test; never use `src/BookStore.Api/books.db` or an external connection by default.
- The factory uses Development to run application migrations; SQLite remains real even though the HTTP test server runs in memory.
- The factory deletes databases it owns on disposal. When a test supplies a path, that test owns cleanup in `finally`.
- In the restart test, share a path only between that test's two instances and dispose both before deleting the file.
- Create your own data; do not depend on execution order, another test's fixed GUID, or shared state.
- Check status, body, headers, and persisted effects when relevant; rejected input must not create or modify books.
- Do not replace EF Core with mocks or its InMemory provider in tests intended to verify relational persistence or migrations.
- Avoid exact assertions on internal messages, generated SQL, or default framework wording unless these are part of the contract.

## Execution and evolution

Run from the root: `dotnet test BookStore.sln`; select a case with `dotnet test BookStore.sln --filter TestName`.
Prioritize scenarios that detect regressions: input rules, HTTP responses, persistence, and request isolation.
Add unit tests when meaningful logic benefits from isolated verification; do not write tests solely to repeat the implementation.
Split files by responsibility as the suite grows; preserve independent scenarios.
If another database provider is adopted, review the integration environment and migrations in the same change. Containers and external services are not decided requirements.
