namespace backend.config
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionMiddleware> _logger;
        private readonly IWebHostEnvironment _env;

        public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger, IWebHostEnvironment env)
        {
            _next = next;
            _logger = logger;
            _env = env;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex);
            }
        }

        private async Task HandleExceptionAsync(HttpContext context, Exception ex)
        {
            int statusCode = StatusCodes.Status500InternalServerError;
            string message = "Error interno del servidor.";

            switch (ex)
            {
                case NotFoundException:
                    statusCode = StatusCodes.Status404NotFound;
                    message = ex.Message;
                    break;
                case ValidationException:
                    statusCode = StatusCodes.Status400BadRequest;
                    message = ex.Message;
                    break;
                case UnauthorizedException:
                    statusCode = StatusCodes.Status401Unauthorized;
                    message = ex.Message;
                    break;
            }

            _logger.LogError(ex, "Error en la API: {Message}", ex.Message);

            var response = new
            {
                StatusCode = statusCode,
                Message = message,
                Detail = ex.StackTrace
            };

            context.Response.StatusCode = statusCode;
            context.Response.ContentType = "application/json";
            await context.Response.WriteAsJsonAsync(response);
        }
    }
}
