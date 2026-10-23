using BookStore.Api.Contracts;

namespace BookStore.Api.Services;

public interface IBookService
{
    Task<List<BookResponse>> GetBooksAsync(CancellationToken cancellationToken);
    Task<BookResponse?> GetBookByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<BookResponse> AddBookAsync(BookRequest request, CancellationToken cancellationToken);
    Task<BookResponse?> UpdateBookAsync(Guid id, BookRequest request, CancellationToken cancellationToken);
    Task<bool> DeleteBookAsync(Guid id, CancellationToken cancellationToken);
}
