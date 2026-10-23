using System.ComponentModel.DataAnnotations;

namespace BookStore.Api.Contracts;

// POST e PUT recebem os mesmos campos; PUT substitui todos os dados do livro.
public sealed record BookRequest
{
    [Required, StringLength(200)]
    public string Title { get; init; } = string.Empty;

    [Required, StringLength(200)]
    public string Author { get; init; } = string.Empty;

    [Required, StringLength(4000)]
    public string Description { get; init; } = string.Empty;

    [Required, StringLength(100)]
    public string Category { get; init; } = string.Empty;

    [Required, StringLength(50)]
    public string Language { get; init; } = string.Empty;

    [Range(1, int.MaxValue)]
    public int TotalPages { get; init; }
}
