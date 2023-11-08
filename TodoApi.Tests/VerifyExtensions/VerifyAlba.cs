using System.Runtime.CompilerServices;
using System.Text.Json.Nodes;

namespace TodoApi.Tests.VerifyExtensions;

public static class VerifyAlba
{
    private static readonly JsonSerializerOptions SerializerOptions = new JsonSerializerOptions(JsonSerializerDefaults.Web)
    {
        WriteIndented = true
    };

    public static bool Initialized { get; private set; }

    public static void Initialize()
    {
        if (Initialized)
        {
            throw new("Already Initialized");
        }

        Initialized = true;
        SerializerOptions.MakeReadOnly(true);
        VerifierSettings.RegisterFileConverter<IScenarioResult>(ScenarioResultConverter);
    }

    private static ConversionResult ScenarioResultConverter(IScenarioResult target, IReadOnlyDictionary<string, object> context)
    {
        var isJsonContent = MediaTypeHeaderValue.TryParse(target.Context.Response.ContentType, out var mediaType)
            && mediaType.MediaType!.EndsWith("json", StringComparison.OrdinalIgnoreCase);

        var result = new
        {
            request = target.Context.Request.Method,
            status = $"{target.Context.Response.StatusCode} {(HttpStatusCode)target.Context.Response.StatusCode}",
            headers = target.Context.Response.Headers,
            body = isJsonContent ? target.ReadAsJson<JsonNode>() : target.ReadAsText()
        };

        var stringResult = JsonSerializer.Serialize(result, SerializerOptions);

        return new ConversionResult(
            null,
            "json",
            stringResult);
    }

    public static async Task Verify(this Task<IScenarioResult> scenario, [CallerFilePath] string sourceFile = "")
    {
        var result = await scenario;
        await Verifier.Verify(result, sourceFile: sourceFile);
    }
}
