using System.Data.Common;
using System.Text.Json;
using GerenciadorFinanceiro.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace GerenciadorFinanceiro.Api.Middleware
{
    /// <summary>
    /// Middleware responsável por converter exceções não tratadas em respostas HTTP legíveis pelo frontend.
    /// </summary>
    public class ExceptionHandlingMiddleware
    {
        private static readonly Action<ILogger, string, string, Exception?> LogUnhandledException =
            LoggerMessage.Define<string, string>(
                LogLevel.Error,
                new EventId(1, nameof(ExceptionHandlingMiddleware)),
                "Erro não tratado ao processar a requisição {Method} {Path}.");

        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionHandlingMiddleware> _logger;
        private readonly IHostEnvironment _environment;

        public ExceptionHandlingMiddleware(
            RequestDelegate next,
            ILogger<ExceptionHandlingMiddleware> logger,
            IHostEnvironment environment)
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
                LogUnhandledException(_logger, context.Request.Method, context.Request.Path, ex);
                await WriteErrorResponseAsync(context, ex);
            }
        }

        private static (int statusCode, string message) MapException(Exception exception) => exception switch
        {
            ArgumentException => (StatusCodes.Status400BadRequest, exception.Message),
            KeyNotFoundException => (StatusCodes.Status404NotFound, exception.Message),
            DbUpdateException => (StatusCodes.Status500InternalServerError, "Nao foi possivel salvar os dados no banco."),
            DbException => (StatusCodes.Status503ServiceUnavailable, "Nao foi possivel conectar ao banco de dados."),
            TimeoutException => (StatusCodes.Status503ServiceUnavailable, "A operacao demorou mais do que o esperado. Tente novamente."),
            _ => (StatusCodes.Status500InternalServerError, "Ocorreu um erro interno ao processar a solicitacao.")
        };

        private async Task WriteErrorResponseAsync(HttpContext context, Exception exception)
        {
            if (context.Response.HasStarted)
            {
                throw exception;
            }

            var (statusCode, message) = MapException(exception);
            var response = new ApiErrorResponse
            {
                Message = message,
                StatusCode = statusCode,
                TraceId = context.TraceIdentifier,
                Details = _environment.IsDevelopment() ? exception.Message : null,
            };

            context.Response.Clear();
            context.Response.StatusCode = statusCode;
            context.Response.ContentType = "application/json";

            await context.Response.WriteAsync(JsonSerializer.Serialize(response));
        }
    }
}
