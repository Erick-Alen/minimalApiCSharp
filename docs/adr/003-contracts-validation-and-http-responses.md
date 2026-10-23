# ADR-003: Explicit contracts, a validation filter, and standardized HTTP responses

- **Date:** 2026-10-23
- **Status:** Accepted
- **Record type:** Retrospective record of the current implementation.

## Context and problem

Creation and update accept the same book fields. We needed to reject invalid input before the operation, distinguish input errors from missing resources, and avoid exposing the persisted entity directly. The example also needed to clearly show how a handler chooses an HTTP response.

## Decision drivers

- Small, predictable contracts for consumers and examples.
- Validation reused by POST and PUT in .NET 8.
- Expected situations handled without artificial business exceptions.

## Options and trade-offs

Retrospective comparison, without trials of alternative libraries:

| Option | Benefit | Cost |
| --- | --- | --- |
| Contracts, Data Annotations, a filter, and Problem Details | Uses available features and centralizes validation | Requires explicitly executing the filter |
| FluentValidation and distinct contracts per operation | Supports independent rules and evolution | Adds a dependency and types for currently identical fields |
| Accept the entity and validate inside each handler | Reduces auxiliary types | Couples persistence to input and repeats rules |

## Decision

Use `BookRequest` shared by POST and PUT and a `BookResponse` separate from the `Book` entity. PUT replaces every field of an existing book; it does not implicitly create one.

`BookValidationFilter` runs Data Annotations, interrupting invalid input with `400` and field errors in Problem Details. Malformed JSON also returns `400`, but not necessarily with field validation errors. Metadata such as `ProducesValidationProblem` only describes the response.

Handlers return `TypedResults`: creation with `201` and `Location`, queries and updates with `200`, deletion with `204`, and missing resources with `404`. An empty collection returns `200` with `[]`. The pipeline uses global exception handling and Problem Details without exposing internal exception messages to clients.

## Consequences

**Positive:** examples and tests share a clear contract; input rules are not repeated inside handlers; expected errors have identifiable status codes.

**Negative:** new fields require reviewing mappings and documentation; future differences between creation and update may justify separate contracts. Attribute validation fits the current rules but does not automatically solve more complex business rules.

## References

- [Endpoints](../../src/BookStore.Api/Endpoints/BookEndpoints.cs)
- [Validation filter](../../src/BookStore.Api/Endpoints/BookValidationFilter.cs)
- [HTTP walkthrough (Brazilian Portuguese)](../minimal-apis-fluxo-basico.md)
