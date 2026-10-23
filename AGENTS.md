# Repository guidance

## Scope and starting points

These instructions apply throughout the repository. Nested `AGENTS.md` files add guidance for their area. Explicit task instructions take precedence over these conventions.
This is an educational book CRUD API built with C# / .NET 8 Minimal APIs. Keep it easy to run, test, and explain a request from end to end.
Read the [README](README.md) and the [ADR index](docs/adr/README.md) before changing structural decisions.

## Repository map and relationships

| Path | Responsibility and relationship to other areas |
| --- | --- |
| `src/` | API implementation: handles HTTP, operates on books, and accesses the database. |
| `tests/` | References the API and verifies HTTP behavior and persistence using isolated databases. |
| `docs/` | Explains the code and preserves decisions; follows observable changes in `src/` and `tests/`. |
| `.config/` | Local `dotnet-ef` tool manifest, aligned with the API's EF Core major version. |
| `BookStore.sln` | Groups the API and test projects. |
| `README.md` | Entry point for setup, commands, and documentation navigation. |

The current flow is `HTTP → Endpoints → IBookService/BookService → BookStoreContext → SQLite`; contracts define input and output. `Program.cs` composes services and configures the pipeline.
`tests` depends on `src`; production code does not reference tests. Documentation is not a runtime dependency.

## Working conventions

- Use English for code, identifiers, filenames, the main README, ADRs, and `AGENTS.md` files. Keep the teaching guide in Brazilian Portuguese and identify its language when linking to it.
- Preserve the book domain and `/api/v1/books` routes unless the task explicitly changes the contract.
- Contract changes require reviewing affected tests, `.http` examples, and documentation.
- Run commands from the repository root: `dotnet restore`, `dotnet build BookStore.sln`, and `dotnet test BookStore.sln`.
- For migrations: `dotnet tool restore` and `dotnet ef database update --project src/BookStore.Api`.
- Do not commit local databases, `bin/`, `obj/`, test results, or secrets.
- For documentation-only changes, check links, commands, and consistency with the code; API tests are not required.

## Architecture evolution

- Start with small changes within existing responsibilities, supported by evidence of the problem they solve.
- Do not add layers, projects, or abstractions solely to anticipate hypothetical growth.
- When dependencies, persistence, or public contracts need to change, explain the options, impact, and trade-offs.
- Record an architectural decision in an ADR once made; keep unresolved questions separate from accepted decisions.
- Preserve history: supersede an existing decision through a new ADR, without rewriting its original rationale. Faithful language translations may preserve the decision and its original metadata.
- Organization by feature, separate projects, and a different database remain open questions, not an implementation plan.
