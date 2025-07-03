using Microsoft.AspNetCore.Http;
using System.Text.Json;

public class ValidationExceptionMiddleware
{
    private readonly RequestDelegate _next;

    public ValidationExceptionMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var originalBody = context.Response.Body;

        using var newBody = new MemoryStream();
        context.Response.Body = newBody;

        await _next(context);

        newBody.Seek(0, SeekOrigin.Begin);
        var responseBody = await new StreamReader(newBody).ReadToEndAsync();

        if (context.Response.StatusCode == 400 && responseBody.Contains("errors"))
        {
            try
            {
                var parsed = JsonSerializer.Deserialize<ValidationErrorWrapper>(responseBody,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                var firstMessage = parsed?.Errors?
                    .SelectMany(x => x.Value)
                    .FirstOrDefault() ?? "Invalid request";

                context.Response.Body = originalBody;
                context.Response.ContentType = "application/json";
                context.Response.StatusCode = 400;

                var result = JsonSerializer.Serialize(new
                {
                    status = false,
                    message = firstMessage
                });

                await context.Response.WriteAsync(result);
            }
            catch
            {
                // fallback: لو الـ parsing فشل لأي سبب، رجّع الرد الأصلي
                newBody.Seek(0, SeekOrigin.Begin);
                await newBody.CopyToAsync(originalBody);
            }
        }
        else
        {
            newBody.Seek(0, SeekOrigin.Begin);
            await newBody.CopyToAsync(originalBody);
        }
    }

    private class ValidationErrorWrapper
    {
        public Dictionary<string, string[]>? Errors { get; set; }
    }
}
