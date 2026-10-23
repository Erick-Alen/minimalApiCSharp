using BookStore.Api.Data;
using BookStore.Api.Endpoints;
using BookStore.Api.Services;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

var connection = new SqliteConnectionStringBuilder(builder.Configuration.GetConnectionString("Books")
    ?? throw new InvalidOperationException("ConnectionStrings:Books não configurada."));
if (!string.IsNullOrEmpty(connection.DataSource) && connection.DataSource != ":memory:"
    && !Path.IsPathRooted(connection.DataSource))
    connection.DataSource = Path.Combine(builder.Environment.ContentRootPath, connection.DataSource);

builder.Services.AddDbContext<BookStoreContext>(options => options.UseSqlite(connection.ToString()));
builder.Services.AddScoped<IBookService, BookService>();
// Erros de binding devem produzir 400 também em Development, sem virar exceções 500.
builder.Services.Configure<RouteHandlerOptions>(options => options.ThrowOnBadRequest = false);
builder.Services.AddProblemDetails();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();
app.UseExceptionHandler();
app.UseStatusCodePages();

if (app.Environment.IsDevelopment())
{
    // Conveniência local. Nos demais ambientes, aplique as migrations antes de iniciar.
    using var scope = app.Services.CreateScope();
    await scope.ServiceProvider.GetRequiredService<BookStoreContext>().Database.MigrateAsync();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapBookEndpoints();
app.Run();

public partial class Program;
