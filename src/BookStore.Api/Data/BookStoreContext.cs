using BookStore.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace BookStore.Api.Data;

public sealed class BookStoreContext(DbContextOptions<BookStoreContext> options) : DbContext(options)
{
    public DbSet<Book> Books => Set<Book>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        var book = modelBuilder.Entity<Book>();
        book.HasKey(b => b.Id);
        book.Property(b => b.Title).HasMaxLength(200).IsRequired();
        book.Property(b => b.Author).HasMaxLength(200).IsRequired();
        book.Property(b => b.Description).HasMaxLength(4000).IsRequired();
        book.Property(b => b.Category).HasMaxLength(100).IsRequired();
        book.Property(b => b.Language).HasMaxLength(50).IsRequired();
        book.ToTable("Books", table => table.HasCheckConstraint("CK_Books_TotalPages", "TotalPages > 0"));
    }
}
