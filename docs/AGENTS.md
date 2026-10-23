# Documentation guidance

## Scope and contents

Applies to `docs/` and its descendants, alongside the root `AGENTS.md`.

| Path | Purpose |
| --- | --- |
| `minimal-apis-fluxo-basico.md` | Brazilian Portuguese teaching guide: a 25-minute walkthrough of HTTP flow, real code, and a demonstration. |
| `adr/README.md` | Index, writing conventions, and status of architectural decisions. |
| `adr/NNN-title.md` | Context, decision, alternatives, and consequences of a significant choice. |

The teaching guide explains behavior implemented in `src/`; tests in `tests/` provide verifiable examples of that behavior.
The root README should link to documentation entry points rather than duplicate their contents.

## Writing and maintenance

- Write technical documentation, ADRs, and agent guidance in English. Keep `minimal-apis-fluxo-basico.md` in Brazilian Portuguese; do not duplicate the entire documentation in both languages.
- Match type names, methods, commands, and paths to the code. Use repository-relative links so they work on GitHub and other checkouts.
- Distinguish real code, excerpts with omissions, and illustrative examples; do not describe a fictional route as implemented.
- When routes, status codes, contracts, or commands change, review the teaching guide, README, and affected HTTP examples.
- Check local links and code fences. Avoid relying on test counts that can become outdated.
- Documentation-only changes require documentation checks, not new tests that mirror prose.

## ADRs

- Use concise MADR in English, ideally 200–500 words, for decisions already made.
- Use sequential three-digit numbers and kebab-case filenames; update the index.
- Include date, status, context, criteria, decision, alternatives, positive and negative consequences, and references.
- Identify retrospective records; do not invent benchmarks, approvals, or historical evaluations of alternatives.
- Keep open proposals separate from accepted decisions.
- Preserve accepted context and decisions; subsequent changes require a new ADR with supersession links. Faithful translations preserve the original decision, numbering, date, and status; note the translation in the index.

## Evolution

Keep operational instructions in `AGENTS.md`, explanations in the teaching guide, and durable rationale in ADRs.
When a new area appears, document its relationships and add local guidance only if it has distinct rules.
Review signals in the ADR index support future discussions; they neither authorize nor prescribe an architectural change.
