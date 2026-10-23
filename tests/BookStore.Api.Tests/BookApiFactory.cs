using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;

namespace BookStore.Api.Tests;

public sealed class BookApiFactory(string? databasePath = null) : WebApplicationFactory<Program>
{
    private readonly string _databasePath = databasePath
        ?? Path.Combine(Path.GetTempPath(), $"bookstore-tests-{Guid.NewGuid():N}.db");

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Development");
        builder.UseSetting("ConnectionStrings:Books", $"Data Source={_databasePath};Pooling=False");
    }

    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);
        if (disposing && databasePath is null)
            File.Delete(_databasePath);
    }
}
