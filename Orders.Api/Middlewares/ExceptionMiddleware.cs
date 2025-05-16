using Orders.Domain.Exceptions;
using System.Net;
using System.Text.Json;

namespace Orders.Api.Middlewares
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionMiddleware> _logger;
        private readonly IHostEnvironment _env;

        public ExceptionMiddleware(
            RequestDelegate next,
            ILogger<ExceptionMiddleware> logger,
            IHostEnvironment env)
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
                _logger.LogError(ex, ex.Message);
                await HandleExceptionAsync(context, ex);
            }
        }

        private async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/json";
            var response = context.Response;

            var errorDetails = _env.IsDevelopment()
                ? new ErrorDetails(exception)
                : new ErrorDetails("Ocorreu um erro interno no servidor");

            switch (exception)
            {
                case BusinessException:
                    response.StatusCode = (int)HttpStatusCode.BadRequest;
                    errorDetails.Title = "Erro de negócio";
                    break;

                case NotFoundException:
                    response.StatusCode = (int)HttpStatusCode.NotFound;
                    errorDetails.Title = "Recurso não encontrado";
                    break;

                case UnauthorizedAccessException:
                    response.StatusCode = (int)HttpStatusCode.Unauthorized;
                    errorDetails.Title = "Acesso não autorizado";
                    break;

                default:
                    response.StatusCode = (int)HttpStatusCode.InternalServerError;
                    errorDetails.Title = "Erro interno no servidor";
                    break;
            }

            var jsonOptions = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
            var jsonResponse = JsonSerializer.Serialize(errorDetails, jsonOptions);

            await context.Response.WriteAsync(jsonResponse);
        }
    }

    public class ErrorDetails
    {
        public string Title { get; set; }
        public string Message { get; set; }

        public ErrorDetails(string message)
        {
            Message = message;
        }

        public ErrorDetails(Exception ex)
        {
            Message = ex.Message;
        }
    }
}
