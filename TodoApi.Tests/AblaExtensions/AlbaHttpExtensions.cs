using System.Diagnostics.CodeAnalysis;
using static Alba.AlbaHostExtensions;

namespace TodoApi.Tests.AblaExtensions;

public static class AlbaHttpExtensions
{
    public static ResponseExpression CreateJson<T>(
        this IAlbaHost system,
        T request,
        [StringSyntax(StringSyntaxAttribute.Uri)] string url,
        JsonStyle? jsonStyle = null) where T : class
    {
        return new ResponseExpression(system, s =>
        {
            s.WriteJson(request, jsonStyle);
            s.Post.Json(request, jsonStyle).ToUrl(url);
            s.StatusCodeShouldBe(StatusCodes.Status201Created);
        });
    }

    public static async Task<T?> TryGet<T>(
        this IAlbaHost system,
        [StringSyntax(StringSyntaxAttribute.Uri)] string url)
    {
        var result = await system.Scenario(s =>
        {
            s.Get.Url(url);
            s.IgnoreStatusCode();
        });

        return result.Context.Response.StatusCode == StatusCodes.Status404NotFound
            ? default(T)
            : await result.ReadAsJsonAsync<T>();
    }
}
