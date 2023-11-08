using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Nodes;

namespace TodoApi.Tests.AblaExtensions;

public static class AblaAssertionExtensions
{
    static AblaAssertionExtensions()
    {
        DefaultSerializerOptions = new JsonSerializerOptions(JsonSerializerDefaults.Web);
        // Needed to avoid System.InvalidOperationException : JsonSerializerOptions instance must specify a TypeInfoResolver setting before being marked as read-only.
        DefaultSerializerOptions.MakeReadOnly(true);
    }

    /// <summary>
    /// The default <see cref="JsonSerializerOptions"/> used by <see cref="ContentShouldBeEquivalentTo{T}(Scenario, T, JsonSerializerOptions?)"/>
    /// and <see cref="ContentShouldBeEquivalentTo{T}(Scenario, T[], JsonSerializerOptions?)"/>.
    /// </summary>
    /// <remarks>
    /// By default this is set to follow the <see cref="JsonSerializerDefaults.Web"/> options.
    /// </remarks>
    public static JsonSerializerOptions DefaultSerializerOptions { get; set; }

    /// <summary>
    /// Assert that the HTTP response body is parsable as JSON and is semantically equivalent to
    /// the <paramref name="expectedJson"/> JSON string.
    /// </summary>
    /// <param name="expectedJson">The expected JSON string</param>
    public static Scenario ContentShouldBeJson(this Scenario scenario, [StringSyntax(StringSyntaxAttribute.Json)] string expectedJson)
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
    public static Scenario ContentShouldBeEquivalentTo<T>(this Scenario scenario, T expected, JsonSerializerOptions? options = null)
    {
        var expectedJson = JsonSerializer.Serialize(expected, options ?? DefaultSerializerOptions);
        return scenario.ContentShouldBeJson(expectedJson);
    }

    /// <summary>
    /// Assert that the HTTP response body is parsable as JSON and is semantically equivalent to
    /// the <paramref name="expected"/> array of <typeparamref name="T"/> objects when it is serialized to JSON using <paramref name="options"/>.
    /// </summary>
    /// <param name="expected">The expect object.</param>
    /// <param name="options">The serialization options to use when converting <paramref name="expected"/> to JSON. 
    /// Defaults to <see cref="DefaultSerializerOptions"/>.</param>
    public static Scenario ContentShouldBeEquivalentTo<T>(this Scenario scenario, T[] expected, JsonSerializerOptions? options = null)
    {
        var expectedJson = JsonSerializer.Serialize(expected, options ?? DefaultSerializerOptions);
        return scenario.ContentShouldBeJson(expectedJson);
    }

    /// <summary>
    /// Assert that the HTTP response body is parsable as JSON and is semantically equivalent to
    /// the <see cref="ProblemDetails"/> type with the supplied arguments when it is serialized to JSON using <paramref name="options"/>.
    /// </summary>
    /// <param name="options">The serialization options to use when converting <paramref name="expected"/> to JSON. 
    /// Defaults to <see cref="DefaultSerializerOptions"/>.</param>
    public static Scenario ContentShouldBeProblemDetails(
        this Scenario scenario,
        string? type = null,
        string? title = null,
        int? status = null,
        string? detail = null,
        string? instance = null,
        IEnumerable<(string Key, object? Value)>? extensions = null,
        JsonSerializerOptions? options = null)
    {
        var problemDetails = new ProblemDetails
        {
            Type = type,
            Title = title,
            Status = status,
            Detail = detail,
            Instance = instance,
        };

        if (extensions is not null)
        {
            foreach (var (key, value) in extensions)
            {
                problemDetails.Extensions.Add(key, value);
            }
        }

        return scenario.ContentShouldBeProblemDetails(problemDetails, options);
    }

    /// <summary>
    /// Assert that the HTTP response body is parsable as JSON and is semantically equivalent to
    /// the <paramref name="problemDetails"/> when it is serialized to JSON using <paramref name="options"/>.
    /// </summary>
    /// <param name="options">The serialization options to use when converting <paramref name="expected"/> to JSON. 
    /// Defaults to <see cref="DefaultSerializerOptions"/>.</param>
    public static Scenario ContentShouldBeProblemDetails<TProblemDetails>(
        this Scenario scenario,
        TProblemDetails problemDetails,
        JsonSerializerOptions? options = null) where TProblemDetails : ProblemDetails
    {
        var expectedJson = JsonSerializer.Serialize(problemDetails, options ?? DefaultSerializerOptions);
        var expected = problemDetails is not null ? JsonNode.Parse(expectedJson) : null;

        return scenario
            .AssertThat(ProblemDetailsContentTypeHeaderAssertion.Instance)
            .AssertThat(new SemanticJsonContentBodyAssertion(expected));
    }

    /// <summary>
    /// Asserts that the HTTP response has status code <see cref="StatusCodes.Status404NotFound"/>
    /// and that the body is semantically equivalent to a rfc9110 <see cref="ProblemDetails"/> 
    /// object with the properties <c>status: 404</c> and property <c>title: "Not Found"</c> set,
    /// when it is serialized to JSON using <paramref name="options"/>.
    /// </summary>
    /// <param name="options">The serialization options to use when converting <paramref name="expected"/> to JSON. 
    /// Defaults to <see cref="DefaultSerializerOptions"/>.</param>
    public static Scenario ResponseShouldBeNotFound(
        this Scenario scenario,
        JsonSerializerOptions? options = null)
    {
        scenario.StatusCodeShouldBe(StatusCodes.Status404NotFound);
        scenario.ContentShouldBeProblemDetails(
            type: "https://tools.ietf.org/html/rfc9110#section-15.5.5",
            status: StatusCodes.Status404NotFound,
            title: "Not Found",
            options: options);

        return scenario;
    }

    /// <summary>
    /// Asserts that the HTTP response has status code <see cref="StatusCodes.Status400BadRequest"/>
    /// and that the body is semantically equivalent to a rfc9110 <see cref="ProblemDetails"/> 
    /// object with the properties <c>status: 400</c> and properties <paramref name="title"/>,
    /// <paramref name="detail"/> when it is serialized to JSON using <paramref name="options"/>.
    /// </summary>
    /// <param name="options">The serialization options to use when converting <paramref name="expected"/> to JSON. 
    /// Defaults to <see cref="DefaultSerializerOptions"/>.</param>
    public static Scenario ResponseShouldBeBadRequest(
        this Scenario scenario,
        string title = "Bad Request",
        string? detail = null,
        JsonSerializerOptions? options = null)
    {
        scenario.ContentShouldBeProblemDetails(new ProblemDetails
        {
            Type = "https://tools.ietf.org/html/rfc9110#section-15.5.1",
            Title = title,
            Status = StatusCodes.Status400BadRequest,
            Detail = detail,
        }, options);

        scenario.StatusCodeShouldBe(StatusCodes.Status400BadRequest);

        return scenario;
    }

    /// <summary>
    /// Asserts that the HTTP response has status code <see cref="StatusCodes.Status400BadRequest"/>
    /// and that the body is semantically equivalent to a rfc9110 <see cref="ValidationProblemDetails"/> 
    /// object with the properties <c>status: 400</c> and properties <paramref name="title"/>,
    /// <paramref name="detail"/>, and <paramref name="errors"/> added when it is serialized to JSON using <paramref name="options"/>.
    /// </summary>
    /// <param name="options">The serialization options to use when converting <paramref name="expected"/> to JSON. 
    /// Defaults to <see cref="DefaultSerializerOptions"/>.</param>
    public static Scenario ResponseShouldBeBadRequest(
        this Scenario scenario,
        (string PropertyName, string[] Errors)[] errors,
        string title = "Bad Request",
        string? detail = null,
        JsonSerializerOptions? options = null)
    {
        var errrorsDict = errors.ToDictionary(x => x.PropertyName, x => x.Errors);
        scenario.ContentShouldBeProblemDetails(new ValidationProblemDetails(errrorsDict)
        {
            Type = "https://tools.ietf.org/html/rfc9110#section-15.5.1",
            Title = title,
            Status = StatusCodes.Status400BadRequest,
            Detail = detail,
        }, options);

        scenario.StatusCodeShouldBe(StatusCodes.Status400BadRequest);

        return scenario;
    }
}