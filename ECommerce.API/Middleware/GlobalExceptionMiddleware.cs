namespace ECommerce.API.Middleware
{
    public class GlobalExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<GlobalExceptionMiddleware> _logger;

        public GlobalExceptionMiddleware(RequestDelegate next, ILogger<GlobalExceptionMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context); // مرر الطلب عادي
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Exception occurred");

                context.Response.ContentType = "application/json";
                context.Response.StatusCode = StatusCodes.Status500InternalServerError;

                var response = new
                {
                    status = false,
                    message = "❌ Internal Server Error",
                    code = 500
                };

                await context.Response.WriteAsJsonAsync(response);
            }
        }
    }

}
