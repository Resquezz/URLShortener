namespace URLShortener.Middlewares
{
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionHandlingMiddleware> _logger;

        public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }
        
        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (ArgumentNullException ex)
            {
                _logger.LogWarning(ex, "Request validation failed.");
                await WriteProblemDetailsAsync(context, StatusCodes.Status400BadRequest, "Invalid request.", ex.Message);
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogInformation(ex, "Requested resource was not found.");
                await WriteProblemDetailsAsync(context, StatusCodes.Status404NotFound, "Resource not found.", ex.Message);
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogWarning(ex, "Request is forbidden for current user.");
                await WriteProblemDetailsAsync(context, StatusCodes.Status403Forbidden, "Forbidden.", ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex, "Request conflicts with current state.");
                await WriteProblemDetailsAsync(context, StatusCodes.Status409Conflict, "Conflict.", ex.Message);
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Request validation failed.");
                await WriteProblemDetailsAsync(context, StatusCodes.Status400BadRequest, "Invalid request.", ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unhandled server error.");
                await WriteProblemDetailsAsync(context, StatusCodes.Status500InternalServerError, "Internal server error.");
            }
        }

        private static Task WriteProblemDetailsAsync(HttpContext context, int statusCode, string title, string? detail = null)
        {
            context.Response.StatusCode = statusCode;
            context.Response.ContentType = "application/problem+json";

            var problem = new Microsoft.AspNetCore.Mvc.ProblemDetails
            {
                Status = statusCode,
                Title = title,
                Type = $"https://httpstatuses.com/{statusCode}",
                Instance = context.Request.Path,
                Detail = detail
            };
            problem.Extensions["traceId"] = context.TraceIdentifier;

            return context.Response.WriteAsJsonAsync(problem);
        }
    }
}
