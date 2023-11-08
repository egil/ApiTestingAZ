using Microsoft.AspNetCore.Http;

namespace TodoApi.Tests.AblaExtensions;

public sealed class JsonContentTypeHeaderAssertion : JsonContentTypeHeaderWithOptionalCharsetAssertion
{
    public static JsonContentTypeHeaderAssertion Instance = new();

    protected override string JsonContentType { get; } = MediaTypeNames.Application.Json;

    private JsonContentTypeHeaderAssertion()
    {
    }
}

public sealed class ProblemDetailsContentTypeHeaderAssertion : JsonContentTypeHeaderWithOptionalCharsetAssertion
{
    public static ProblemDetailsContentTypeHeaderAssertion Instance = new();

    protected override string JsonContentType { get; } = MediaTypeNames.Application.ProblemJson;

    private ProblemDetailsContentTypeHeaderAssertion()
    {
    }
}

public abstract class JsonContentTypeHeaderWithOptionalCharsetAssertion : IScenarioAssertion
{
    protected abstract string JsonContentType { get; }

    public void Assert(Scenario scenario, HttpContext context, ScenarioAssertionException ex)
    {
        if (!MediaTypeHeaderValue.TryParse(context.Response.ContentType, out var mediaType))
        {
            ex.Add($"Unable to parse content-type header. Value is = {context.Response.ContentType}");
            return;
        }

        if (mediaType.MediaType != JsonContentType)
        {
            ex.Add($"Wrong content-type.{Environment.NewLine}{Environment.NewLine}Expected: {JsonContentType}{Environment.NewLine}Actual: {context.Response.ContentType}");
        }
    }
}