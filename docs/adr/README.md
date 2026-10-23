# Architecture decision records

These records retrospectively document choices already present in the implementation. Their recorded date is 2026-10-23, using the America/Sao_Paulo time zone. **Accepted** describes the current project baseline; it does not imply external studies or approvals.

Format: concise MADR, following the Tech Leads Club `create-adr` skill available in the authoring environment. Option comparisons explain trade-offs using current knowledge; they do not represent benchmarks or proofs of concept performed during the initial implementation.

The records were translated from Brazilian Portuguese to English without changing their decisions, numbers, recorded dates, or status. Filenames and links were updated to match the repository's language convention.

| ADR | Decision | Status |
| --- | --- | --- |
| [001](001-minimal-api-and-organization.md) | .NET 8 Minimal API with separated responsibilities in one project | Accepted |
| [002](002-sqlite-persistence-and-migrations.md) | EF Core with file-based SQLite and versioned migrations | Accepted |
| [003](003-contracts-validation-and-http-responses.md) | Explicit contracts, a validation filter, and standardized HTTP responses | Accepted |
| [004](004-isolated-integration-tests.md) | HTTP integration tests with isolated temporary SQLite databases | Accepted |

## Conventions

1. Check existing numbers and create `NNN-kebab-case-title.md` for the next significant decision.
2. Record context, criteria, options, decision, positive and negative consequences, and links to code or other ADRs.
3. Use the actual recording date; distinguish a retrospective record from a new decision.
4. Do not rewrite an accepted decision to reflect a later choice. Create a new ADR, update the previous record's status and supersession link, and retain its historical rationale.
5. Identify pending decisions as open questions; do not list them here as Accepted.

## Open architecture questions

| Future signal | Question to discuss |
| --- | --- |
| New functionality makes operations hard to find or change | Keep folders by responsibility or organize by feature? |
| Business rules grow or consumers beyond HTTP appear | Would separating application and domain into projects bring a concrete benefit? |
| Operational requirements exceed local file-based storage | Which provider and deployment strategy meet those requirements? |
| Contracts must change for existing consumers | How should compatibility and versioning work beyond the literal `/v1` prefix? |

These are topics for future discussion, without a chosen solution or schedule. `AGENTS.md` files explain how to work within the current structure; ADRs preserve why decisions were made.
