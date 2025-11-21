using FuturoDoTrabalho.Api.Dtos;
using System.Net;

namespace FuturoDoTrabalho.Api.Middlewares
{
    // ====================================================================================
    // MIDDLEWARE: GlobalExceptionHandlerMiddleware
    // ====================================================================================
    // Provides centralized exception handling for the entire API
    // Catches all unhandled exceptions and returns standardized error responses
    // Logs exceptions for debugging and monitoring purposes
    // ====================================================================================
    public class GlobalExceptionHandlerMiddleware
    {
        // ====================
        // FIELDS
        // ====================
        // The next middleware in the request pipeline
        private readonly RequestDelegate _next;

        // Logger for recording exception details
        private readonly ILogger<GlobalExceptionHandlerMiddleware> _logger;

        // ====================
        // CONSTRUCTOR
        // ====================
        // Initializes middleware with next pipeline delegate and logger
        public GlobalExceptionHandlerMiddleware(RequestDelegate next, ILogger<GlobalExceptionHandlerMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        // ====================
        // METHOD: InvokeAsync
        // Processes the request and catches any unhandled exceptions
        // Calls next middleware in pipeline or handles exception if one occurs
        // ====================
        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ocorreu uma exceção não tratada: {Message}", ex.Message);
                await HandleExceptionAsync(context, ex);
            }
        }

        // ====================
        // METHOD: HandleExceptionAsync
        // Processes exception and returns standardized error response
        // Maps specific exception types to appropriate HTTP status codes
        // ====================
        private static Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/json";

            var response = new ApiResponse();

            switch (exception)
            {
                case ArgumentNullException argNullEx:
                    context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                    response = ApiResponse.ErrorResponse($"Argumento nulo: {argNullEx.ParamName}");
                    break;

                case ArgumentException argEx:
                    context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                    response = ApiResponse.ErrorResponse($"Argumento inválido: {argEx.Message}");
                    break;

                case UnauthorizedAccessException:
                    context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                    response = ApiResponse.ErrorResponse("Acesso não autorizado");
                    break;

                case KeyNotFoundException keyEx:
                    context.Response.StatusCode = (int)HttpStatusCode.NotFound;
                    response = ApiResponse.ErrorResponse($"Recurso não encontrado: {keyEx.Message}");
                    break;

                case InvalidOperationException invOpEx:
                    context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                    response = ApiResponse.ErrorResponse($"Operação inválida: {invOpEx.Message}");
                    break;

                default:
                    context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                    response = ApiResponse.ErrorResponse("Ocorreu um erro interno no servidor");
                    break;
            }

            return context.Response.WriteAsJsonAsync(response);
        }
    }

    // ====================================================================================
    // EXTENSION CLASS: GlobalExceptionHandlerMiddlewareExtensions
    // ====================================================================================
    // Provides convenient extension method for registering middleware in the request pipeline
    // ====================================================================================
    public static class GlobalExceptionHandlerMiddlewareExtensions
    {
        // ====================
        // EXTENSION METHOD: UseGlobalExceptionHandler
        // Registers the GlobalExceptionHandlerMiddleware in the application builder pipeline
        // ====================
        public static IApplicationBuilder UseGlobalExceptionHandler(this IApplicationBuilder app)
        {
            return app.UseMiddleware<GlobalExceptionHandlerMiddleware>();
        }
    }
}
