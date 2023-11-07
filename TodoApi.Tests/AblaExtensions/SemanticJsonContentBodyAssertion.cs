using Microsoft.AspNetCore.Http;
using System.Text;
using System.Text.Json;
using System.Text.Json.JsonDiffPatch;
using System.Text.Json.Nodes;

namespace TodoApi.AblaExtensions;

public sealed class SemanticJsonContentBodyAssertion : IScenarioAssertion
{
    private static readonly JsonSerializerOptions SerializerOptions = new JsonSerializerOptions(JsonSerializerDefaults.Web)
    {
        WriteIndented = true
    };

    public JsonNode? Expected { get; }

    public SemanticJsonContentBodyAssertion(JsonNode? expected)
    {
        Expected = expected;
    }

    public void Assert(Scenario scenario, HttpContext context, ScenarioAssertionException ex)
    {
        var body = ex.ReadBody(context);        

        var actualJson = body is not null ? JsonNode.Parse(body) : null;
        var diffJson = JsonDiffPatcher.Diff(Expected, actualJson, new JsonDiffOptions
        {
            JsonElementComparison = JsonElementComparison.Semantic,
        });

        if (diffJson is not null)
        {
            var builder = new StringBuilder();
            builder.AppendLine("Response body does not contain the expected JSON:");
            builder.AppendLine();
            AppendJsonDiff(builder, Expected, actualJson, diffJson);
            ex.Add(builder.ToString());
        }
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
