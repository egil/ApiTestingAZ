using System.Diagnostics.CodeAnalysis;
using static Alba.AlbaHostExtensions;

namespace TodoApi.AblaExtensions;

public static class AlbaHttpExtensions
{
    public static ResponseExpression CreateJson<T>(this IAlbaHost system, T request, [StringSyntax(StringSyntaxAttribute.Uri)] string url, JsonStyle? jsonStyle = null) where T : class
    {
        return new ResponseExpression(system, s =>
        {
            s.WriteJson(request, jsonStyle);
            s.Post.Json(request, jsonStyle).ToUrl(url);
            s.StatusCodeShouldBe(HttpStatusCode.Created);
        });
    }
}
