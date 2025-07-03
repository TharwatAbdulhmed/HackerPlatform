using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace ECommerce.API.Middleware
{
    public class ValidationExceptionMiddleware
    {
        private readonly RequestDelegate _next;

        public ValidationExceptionMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            // امسك الـ Response وراقبه
            var originalBody = context.Response.Body;

            using var newBody = new MemoryStream();
            context.Response.Body = newBody;

            await _next(context);

            // ارجع جسم الاستجابة وشيّك على الـ 400
            if (context.Response.StatusCode == 400 &&
                context.Response.ContentType?.Contains("application/problem+json") == true)
            {
                context.Response.Body.Seek(0, SeekOrigin.Begin);
                var problemJson = await new StreamReader(context.Response.Body).ReadToEndAsync();

                var problemDetails = JsonSerializer.Deserialize<ValidationProblemDetails>(problemJson, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                var firstError = problemDetails?.Errors?.Values.FirstOrDefault()?.FirstOrDefault();

                context.Response.Body = originalBody;
                context.Response.ContentType = "application/json";

                var customError = new
                {
                    status = false,
                    message = $"❌ {firstError ?? "Validation error"}"
                };

                await context.Response.WriteAsJsonAsync(customError);
            }
            else
            {
                context.Response.Body.Seek(0, SeekOrigin.Begin);
                await newBody.CopyToAsync(originalBody);
            }
        }
    }

}
