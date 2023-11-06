using Microsoft.AspNetCore.Diagnostics;
using System.Text.Json;

namespace Api;

internal static class ApiErrorHandling
{
    public static void UseCustomErrorPayloads(this WebApplication app)
    {
        app.UseExceptionHandler(exceptionApp => exceptionApp.Run(async context =>
        {
            var feature = context.Features.Get<IExceptionHandlerPathFeature>();
            if (feature?.Error is BadHttpRequestException ex)
            {
                var message = ex.InnerException is JsonException
                    ? "The request body contains invalid or incorrectly formatted JSON"
                    : "Bad Request";

                await Results.Problem(detail: message, statusCode: 400).ExecuteAsync(context);
            }
            else
            {
                await Results.Problem(statusCode: 500).ExecuteAsync(context);
            }
        }));

        app.UseStatusCodePages(async statusCodeContext => 
            await Results.Problem(statusCode: statusCodeContext.HttpContext.Response.StatusCode)
                .ExecuteAsync(statusCodeContext.HttpContext));
    }
}
