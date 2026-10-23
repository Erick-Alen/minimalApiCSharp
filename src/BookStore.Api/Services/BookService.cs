using System.Linq.Expressions;
using BookStore.Api.Contracts;
using BookStore.Api.Data;
using BookStore.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace BookStore.Api.Services;

public sealed class BookService(BookStoreContext context) : IBookService
{
    private static readonly Expression<Func<Book, BookResponse>> Project = book => new BookResponse(
        book.Id, book.Title, book.Author, book.Description, book.Category, book.Language, book.TotalPages);

    public Task<List<BookResponse>> GetBooksAsync(CancellationToken cancellationToken) =>
        context.Books.AsNoTracking().OrderBy(book => book.Title).ThenBy(book => book.Id)
            .Select(Project).ToListAsync(cancellationToken);

    public Task<BookResponse?> GetBookByIdAsync(Guid id, CancellationToken cancellationToken) =>
        context.Books.AsNoTracking().Where(book => book.Id == id)
            .Select(Project).SingleOrDefaultAsync(cancellationToken);

    public async Task<BookResponse> AddBookAsync(BookRequest request, CancellationToken cancellationToken)
    {
        var book = new Book { Id = Guid.NewGuid() };
        Apply(request, book);
        context.Books.Add(book);
        await context.SaveChangesAsync(cancellationToken);
        return ToResponse(book);
    }

    public async Task<BookResponse?> UpdateBookAsync(Guid id, BookRequest request, CancellationToken cancellationToken)
    {
        var book = await context.Books.FindAsync([id], cancellationToken);
        if (book is null)
            return null;

        Apply(request, book);
        try
        {
            await context.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateConcurrencyException)
        {
            // Outro pedido pode ter excluído o livro depois da leitura.
            if (!await context.Books.AnyAsync(book => book.Id == id, cancellationToken))
                return null;
            throw;
        }

        return ToResponse(book);
    }

    public async Task<bool> DeleteBookAsync(Guid id, CancellationToken cancellationToken) =>
        await context.Books.Where(book => book.Id == id).ExecuteDeleteAsync(cancellationToken) > 0;

    private static void Apply(BookRequest request, Book book)
    {
        book.Title = request.Title.Trim();
        book.Author = request.Author.Trim();
        book.Description = request.Description.Trim();
        book.Category = request.Category.Trim();
        book.Language = request.Language.Trim();
        book.TotalPages = request.TotalPages;
    }

    private static BookResponse ToResponse(Book book) => new(
        book.Id, book.Title, book.Author, book.Description, book.Category, book.Language, book.TotalPages);
}
