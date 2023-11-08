using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace TodoApi.AblaExtensions;

public static class AblaAssertionExtensions
{
    /// <summary>
    /// The default <see cref="JsonSerializerOptions"/> used by <see cref="ContentShouldBeJsonEquivalentTo{T}(Scenario, T, JsonSerializerOptions?)"/>
    /// and <see cref="ContentShouldBeJsonEquivalentTo{T}(Scenario, T[], JsonSerializerOptions?)"/>.
    /// </summary>
    /// <remarks>
    /// By default this is set to follow the <see cref="JsonSerializerDefaults.Web"/> options.
    /// </remarks>
    public static JsonSerializerOptions DefaultSerializerOptions { get; set; } = new JsonSerializerOptions(JsonSerializerDefaults.Web);

    /// <summary>
    /// Assert that the HTTP response body is parsable as JSON and is semantically equivalent to
    /// the <paramref name="expectedJson"/> JSON string.
    /// </summary>
    /// <param name="expectedJson">The expected JSON string</param>
    public static Scenario ContentShouldBeJsonEquivalentTo(this Scenario scenario, [StringSyntax(StringSyntaxAttribute.Json)] string expectedJson)
    {
        var expected = !string.IsNullOrWhiteSpace(expectedJson) ? JsonNode.Parse(expectedJson) : null;
        return scenario
            .AssertThat(JsonContentTypeHeaderAssertion.Instance)
            .AssertThat(new SemanticJsonContentBodyAssertion(expected));
    }

    /// <summary>
    /// Assert that the HTTP response body is parsable as JSON and is semantically equivalent to
    /// the <paramref name="expected"/> <typeparamref name="T"/> object when it is serialized to JSON using <paramref name="options"/>.
    /// </summary>
    /// <param name="expected">The expect object.</param>
    /// <param name="options">The serialization options to use when converting <paramref name="expected"/> to JSON. 
    /// Defaults to <see cref="DefaultSerializerOptions"/>.</param>
    public static Scenario ContentShouldBeJsonEquivalentTo<T>(this Scenario scenario, T expected, JsonSerializerOptions? options = null)
    {
        var expectedJson = JsonSerializer.Serialize(expected, options ?? DefaultSerializerOptions);
        return ContentShouldBeJsonEquivalentTo(scenario, expectedJson);
    }

    /// <summary>
    /// Assert that the HTTP response body is parsable as JSON and is semantically equivalent to
    /// the <paramref name="expected"/> array of <typeparamref name="T"/> objects when it is serialized to JSON using <paramref name="options"/>.
    /// </summary>
    /// <param name="expected">The expect object.</param>
    /// <param name="options">The serialization options to use when converting <paramref name="expected"/> to JSON. 
    /// Defaults to <see cref="DefaultSerializerOptions"/>.</param>
    public static Scenario ContentShouldBeJsonEquivalentTo<T>(this Scenario scenario, T[] expected, JsonSerializerOptions? options = null)
    {
        var expectedJson = JsonSerializer.Serialize(expected, options ?? DefaultSerializerOptions);
        return ContentShouldBeJsonEquivalentTo(scenario, expectedJson);
    }
}