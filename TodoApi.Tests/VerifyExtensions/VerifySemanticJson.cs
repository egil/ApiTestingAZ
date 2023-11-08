using System.Text;
using System.Text.Json.Nodes;
using System.Text.Json;
using System.Text.Json.JsonDiffPatch;

namespace TodoApi.VerifyExtensions;

public class VerifySemanticJson
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
        var verifiedJson = !string.IsNullOrWhiteSpace(verified) ? JsonNode.Parse(verified) : null;
        var receivedJson = !string.IsNullOrWhiteSpace(received) ? JsonNode.Parse(received) : null;
        var diffJson = JsonDiffPatcher.Diff(verifiedJson, receivedJson, new JsonDiffOptions
        {
            JsonElementComparison = JsonElementComparison.Semantic,
        });

        if (diffJson is null)
        {
            return Task.FromResult(CompareResult.Equal);
        }

        var errMsgBuilder = new StringBuilder();
        errMsgBuilder.AppendLine("The received JSON does not matched the verified JSON:");
        errMsgBuilder.AppendLine();
        AppendJsonDiff(errMsgBuilder, verifiedJson, receivedJson, diffJson);

        return Task.FromResult(CompareResult.NotEqual(errMsgBuilder.ToString()));
    }

    private static void AppendJsonDiff(StringBuilder builder, JsonNode? expected, JsonNode? actual, JsonNode diff)
    {
        builder.Append("Expected:");
        builder.AppendLine();
        builder.Append((expected is null) ? "null" : expected.ToJsonString(SerializerOptions));
        builder.AppendLine();
        builder.Append("Actual:");
        builder.AppendLine();
        builder.Append((actual is null) ? "null" : actual.ToJsonString(SerializerOptions));
        builder.AppendLine();
        builder.Append("Delta:");
        builder.AppendLine();
        builder.Append(diff.ToJsonString(SerializerOptions));
    }
}
