using BookStore.Api.Contracts;
using BookStore.Api.Services;
using Microsoft.AspNetCore.Http.HttpResults;

namespace BookStore.Api.Endpoints;

public static class BookEndpoints
{
    public static IEndpointRouteBuilder MapBookEndpoints(this IEndpointRouteBuilder app)
    {
        var books = app.MapGroup("/api/v1/books").WithTags("Books");

        books.MapGet("/", async (IBookService service, CancellationToken cancellationToken) =>
            TypedResults.Ok(await service.GetBooksAsync(cancellationToken)))
            .WithSummary("Listar livros");

        books.MapGet("/{id:guid}", async Task<Results<Ok<BookResponse>, NotFound>> (
            Guid id, IBookService service, CancellationToken cancellationToken) =>
        {
            var book = await service.GetBookByIdAsync(id, cancellationToken);
            return book is null ? TypedResults.NotFound() : TypedResults.Ok(book);
        }).WithName("GetBookById").WithSummary("Buscar livro pelo ID");

        books.MapPost("/", async (BookRequest request, IBookService service, CancellationToken cancellationToken) =>
        {
            var book = await service.AddBookAsync(request, cancellationToken);
            return TypedResults.CreatedAtRoute(book, "GetBookById", new { id = book.Id });
        }).AddEndpointFilter<BookValidationFilter>()
            .ProducesValidationProblem().WithSummary("Cadastrar livro");

        books.MapPut("/{id:guid}", async Task<Results<Ok<BookResponse>, NotFound>> (
            Guid id, BookRequest request, IBookService service, CancellationToken cancellationToken) =>
        {
            var book = await service.UpdateBookAsync(id, request, cancellationToken);
            return book is null ? TypedResults.NotFound() : TypedResults.Ok(book);
        }).AddEndpointFilter<BookValidationFilter>()
            .ProducesValidationProblem().WithSummary("Substituir dados do livro");

        books.MapDelete("/{id:guid}", async Task<Results<NoContent, NotFound>> (
            Guid id, IBookService service, CancellationToken cancellationToken) =>
            await service.DeleteBookAsync(id, cancellationToken)
                ? TypedResults.NoContent() : TypedResults.NotFound())
            .WithSummary("Excluir livro");

        return app;
    }
}
