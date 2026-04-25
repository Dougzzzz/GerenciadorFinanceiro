using System.Data.Common;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;

namespace GerenciadorFinanceiro.Api.Middleware
{
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionHandlingMiddleware> _logger;
        private readonly IHostEnvironment _environment;

        public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger, IHostEnvironment environment)
        {
            _next = next;
            _logger = logger;
            _environment = environment;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro não tratado: {Message}", ex.Message);
                await WriteErrorResponseAsync(context, ex);
            }
        }

        private static (int statusCode, string message) MapException(Exception exception) => exception switch
        {
            ArgumentException => (StatusCodes.Status400BadRequest, exception.Message),
            KeyNotFoundException => (StatusCodes.Status404NotFound, exception.Message),
            DbUpdateException => (StatusCodes.Status500InternalServerError, "Não foi possível salvar os dados no banco."),
            DbException => (StatusCodes.Status503ServiceUnavailable, "Não foi possível conectar ao banco de dados."),
            TimeoutException => (StatusCodes.Status503ServiceUnavailable, "A operação demorou mais do que o esperado. Tente novamente."),
            _ => (StatusCodes.Status500InternalServerError, "Ocorreu um erro interno ao processar a solicitação.")
        };

        private async Task WriteErrorResponseAsync(HttpContext context, Exception exception)
        {
            if (context.Response.HasStarted)
            {
                return;
            }

            var (statusCode, message) = MapException(exception);

            var response = new
            {
                title = "Erro na Solicitação",
                status = statusCode,
                message,
                traceId = context.TraceIdentifier,
                details = _environment.IsDevelopment() ? exception.Message : null,
                stackTrace = _environment.IsDevelopment() ? exception.StackTrace : null,
            };

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = statusCode;

            await context.Response.WriteAsync(JsonSerializer.Serialize(response, new JsonSerializerOptions { PropertyNameCaseInsensitive = true, WriteIndented = true }));
        }
    }
}
