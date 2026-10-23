namespace BookStore.Api.Contracts;

public sealed record BookResponse(
    Guid Id, string Title, string Author, string Description,
    string Category, string Language, int TotalPages);
