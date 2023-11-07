using System.Text;
using System.Text.Json.Nodes;
using System.Text.Json;
using System.Text.Json.JsonDiffPatch;

namespace TodoApi.VerifyExtensions;

internal class VerifySemanticJson
{
    public static bool Initialized { get; private set; }

    public static void Initialize()
    {
        if (Initialized)
        {
            throw new("Already Initialized");
        }

        Initialized = true;

        VerifierSettings.RegisterFileConverter<JsonDocument>(JsonDocumentConverter);
        VerifierSettings.RegisterFileConverter<JsonElement>(JsonElementConverter);
        VerifierSettings.RegisterFileConverter<JsonNode>(JsonNodeConverter);
        VerifierSettings.RegisterStringComparer("json", JsonSemanticCompare);
    }

    private static ConversionResult JsonDocumentConverter(JsonDocument target, IReadOnlyDictionary<string, object> context)
    {
        using var stream = new MemoryStream();
        Utf8JsonWriter writer = new Utf8JsonWriter(stream, new JsonWriterOptions { Indented = true });
        target.WriteTo(writer);
        writer.Flush();
        return new ConversionResult(null, "json", Encoding.UTF8.GetString(stream.ToArray()));
    }

    private static ConversionResult JsonElementConverter(JsonElement target, IReadOnlyDictionary<string, object> context)
    {
        using var stream = new MemoryStream();
        Utf8JsonWriter writer = new Utf8JsonWriter(stream, new JsonWriterOptions { Indented = true });
        target.WriteTo(writer);
        writer.Flush();
        return new ConversionResult(null, "json", Encoding.UTF8.GetString(stream.ToArray()));
    }

    private static ConversionResult JsonNodeConverter(JsonNode target, IReadOnlyDictionary<string, object> context)
    {
        using var stream = new MemoryStream();
        Utf8JsonWriter writer = new Utf8JsonWriter(stream, new JsonWriterOptions { Indented = true });
        target.WriteTo(writer);
        writer.Flush();
        return new ConversionResult(null, "json", Encoding.UTF8.GetString(stream.ToArray()));
    }

    private static Task<CompareResult> JsonSemanticCompare(string received, string verified, IReadOnlyDictionary<string, object> context)
    {
        var receivedJson = JsonDocument.Parse(received);
        var verifiedJson = JsonDocument.Parse(verified);
        var result = verifiedJson.DeepEquals(receivedJson, JsonElementComparison.Semantic);
        return Task.FromResult(new CompareResult(result));
    }
}
