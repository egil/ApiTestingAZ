using FluentValidation;
using System.Text.Json;

namespace TodoApi;

internal static class ApiSerialization
{
    public static void ConfigureApiSerialization(this WebApplicationBuilder builder)
    {
        builder.Services.ConfigureHttpJsonOptions(options =>
        {
            options.SerializerOptions.WriteIndented = true;
            options.SerializerOptions.IncludeFields = true;
            options.SerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
        });

        // Override fluent validation property name resolver 
        // such that it uses the same casing and style as the json serialization.
        ValidatorOptions.Global.DisplayNameResolver = (type, member, expression) =>
        {
            if (member is not null)
            {
                return JsonNamingPolicy.CamelCase.ConvertName(member.Name);
            }

            return null;
        };
    }

    public static IDictionary<string, string[]> ToValidationProblemErrors(this FluentValidation.Results.ValidationResult result) 
        => result
            .Errors
            .GroupBy(x => x.PropertyName)
            .ToDictionary(
                g => JsonNamingPolicy.CamelCase.ConvertName(g.Key),
                g => g.Select(x => x.ErrorMessage).ToArray()
            );
}
