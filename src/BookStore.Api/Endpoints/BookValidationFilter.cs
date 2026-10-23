using System.ComponentModel.DataAnnotations;
using BookStore.Api.Contracts;

namespace BookStore.Api.Endpoints;

public sealed class BookValidationFilter : IEndpointFilter
{
    public ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
    {
        var request = context.Arguments.OfType<BookRequest>().SingleOrDefault();
        if (request is null)
            return ValueTask.FromResult<object?>(TypedResults.ValidationProblem(
                new Dictionary<string, string[]> { ["body"] = ["Informe os dados do livro."] }));

        var results = new List<ValidationResult>();
        if (Validator.TryValidateObject(request, new ValidationContext(request), results, validateAllProperties: true))
            return next(context);

        var errors = results.SelectMany(result => result.MemberNames.Select(member => new
            { Member = member, Message = result.ErrorMessage ?? "Valor inválido." }))
            .GroupBy(error => error.Member)
            .ToDictionary(group => group.Key, group => group.Select(error => error.Message).ToArray());

        return ValueTask.FromResult<object?>(TypedResults.ValidationProblem(errors));
    }
}
