using System.Net;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using BookStore.Api.Contracts;

namespace BookStore.Api.Tests;

public sealed class BookEndpointsTests
{
    private const string Url = "/api/v1/books";

    private static BookRequest ValidBook => new()
    {
        Title = "Dom Casmurro",
        Author = "Machado de Assis",
        Description = "Romance brasileiro.",
        Category = "Romance",
        Language = "Português",
        TotalPages = 256
    };

    [Fact]
    public async Task Crud_returns_expected_statuses_and_persists_all_fields()
    {
        using var factory = new BookApiFactory();
        using var client = factory.CreateClient();
        Assert.Empty((await client.GetFromJsonAsync<List<BookResponse>>(Url))!);

        var create = await client.PostAsJsonAsync(Url, ValidBook with { Title = "  Dom Casmurro  " });
        Assert.Equal(HttpStatusCode.Created, create.StatusCode);
        var book = (await create.Content.ReadFromJsonAsync<BookResponse>())!;
        Assert.NotEqual(Guid.Empty, book.Id);
        Assert.Equal(ValidBook.Title, book.Title);
        Assert.EndsWith($"{Url}/{book.Id}", create.Headers.Location!.ToString());
        Assert.Equal(book, await client.GetFromJsonAsync<BookResponse>(create.Headers.Location));
        Assert.Equal(book, Assert.Single((await client.GetFromJsonAsync<List<BookResponse>>(Url))!));

        var replacement = ValidBook with
        {
            Title = "O Alienista", Author = "M. de Assis", Description = "Nova descrição.",
            Category = "Novela", Language = "pt-BR", TotalPages = 96
        };
        var update = await client.PutAsJsonAsync($"{Url}/{book.Id}", replacement);
        Assert.Equal(HttpStatusCode.OK, update.StatusCode);
        var expected = new BookResponse(book.Id, replacement.Title, replacement.Author,
            replacement.Description, replacement.Category, replacement.Language, replacement.TotalPages);
        Assert.Equal(expected, await update.Content.ReadFromJsonAsync<BookResponse>());
        Assert.Equal(expected, await client.GetFromJsonAsync<BookResponse>($"{Url}/{book.Id}"));

        var delete = await client.DeleteAsync($"{Url}/{book.Id}");
        Assert.Equal(HttpStatusCode.NoContent, delete.StatusCode);
        Assert.Empty(await delete.Content.ReadAsStringAsync());
        Assert.Equal(HttpStatusCode.NotFound, (await client.GetAsync($"{Url}/{book.Id}")).StatusCode);
        Assert.Empty((await client.GetFromJsonAsync<List<BookResponse>>(Url))!);
    }

    [Fact]
    public async Task Missing_books_return_404_for_read_update_and_delete()
    {
        using var factory = new BookApiFactory();
        using var client = factory.CreateClient();
        var url = $"{Url}/{Guid.NewGuid()}";
        foreach (var response in new[]
        {
            await client.GetAsync(url), await client.PutAsJsonAsync(url, ValidBook), await client.DeleteAsync(url)
        })
        {
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
            Assert.Equal("application/problem+json", response.Content.Headers.ContentType!.MediaType);
        }
        Assert.Empty((await client.GetFromJsonAsync<List<BookResponse>>(Url))!);
    }

    [Theory]
    [InlineData("Title", "")]
    [InlineData("Author", "   ")]
    [InlineData("Description", null)]
    [InlineData("Category", "")]
    [InlineData("Language", "")]
    [InlineData("TotalPages", "0")]
    [InlineData("TotalPages", "-1")]
    [InlineData("Title", "LONG")]
    public async Task Invalid_books_return_400_without_creating_or_modifying_data(string field, string? value)
    {
        using var factory = new BookApiFactory();
        using var client = factory.CreateClient();
        var create = await client.PostAsJsonAsync(Url, ValidBook);
        var original = (await create.Content.ReadFromJsonAsync<BookResponse>())!;
        var invalid = JsonSerializer.SerializeToNode(ValidBook)!.AsObject();
        invalid[field] = field == "TotalPages" ? JsonSerializer.SerializeToNode(int.Parse(value!))
            : JsonSerializer.SerializeToNode(value == "LONG" ? new string('x', 201) : value);

        foreach (var response in new[]
        {
            await client.PostAsJsonAsync(Url, invalid),
            await client.PutAsJsonAsync($"{Url}/{original.Id}", invalid)
        })
        {
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
            Assert.Equal("application/problem+json", response.Content.Headers.ContentType!.MediaType);
            var error = await response.Content.ReadFromJsonAsync<JsonElement>();
            Assert.True(error.GetProperty("errors").TryGetProperty(field, out _));
        }

        Assert.Equal(original, await client.GetFromJsonAsync<BookResponse>($"{Url}/{original.Id}"));
        Assert.Single((await client.GetFromJsonAsync<List<BookResponse>>(Url))!);
    }

    [Theory]
    [InlineData("{")]
    [InlineData("")]
    [InlineData("null")]
    [InlineData("{}")]
    [InlineData("{\"totalPages\":\"abc\"}")]
    public async Task Malformed_or_incomplete_requests_return_400(string json)
    {
        using var factory = new BookApiFactory();
        using var client = factory.CreateClient();
        var response = await client.PostAsync(Url, new StringContent(json, Encoding.UTF8, "application/json"));
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal("application/problem+json", response.Content.Headers.ContentType!.MediaType);
        Assert.Empty((await client.GetFromJsonAsync<List<BookResponse>>(Url))!);
    }

    [Fact]
    public async Task Books_survive_application_restart()
    {
        var path = Path.Combine(Path.GetTempPath(), $"bookstore-persistence-{Guid.NewGuid():N}.db");
        try
        {
            BookResponse book;
            using (var factory = new BookApiFactory(path))
            using (var client = factory.CreateClient())
            {
                var response = await client.PostAsJsonAsync(Url, ValidBook);
                book = (await response.Content.ReadFromJsonAsync<BookResponse>())!;
            }
            using var restartedFactory = new BookApiFactory(path);
            using var restartedClient = restartedFactory.CreateClient();
            Assert.Equal(book, await restartedClient.GetFromJsonAsync<BookResponse>($"{Url}/{book.Id}"));
        }
        finally
        {
            File.Delete(path);
        }
    }

    [Fact]
    public async Task Swagger_describes_the_five_crud_operations()
    {
        using var factory = new BookApiFactory();
        using var client = factory.CreateClient();
        var document = await client.GetFromJsonAsync<JsonElement>("/swagger/v1/swagger.json");
        var paths = document.GetProperty("paths");
        var collection = paths.GetProperty(Url);
        Assert.True(collection.TryGetProperty("get", out _));
        Assert.True(collection.TryGetProperty("post", out _));
        var item = paths.GetProperty(Url + "/{id}");
        Assert.True(item.TryGetProperty("get", out _));
        Assert.True(item.TryGetProperty("put", out _));
        Assert.True(item.TryGetProperty("delete", out _));
        Assert.Equal(HttpStatusCode.OK, (await client.GetAsync("/swagger/index.html")).StatusCode);
    }
}
