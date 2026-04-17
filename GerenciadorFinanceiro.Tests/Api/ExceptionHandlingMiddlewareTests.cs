using System.Data.Common;
using System.Text.Json;
using GerenciadorFinanceiro.Api.Middleware;
using GerenciadorFinanceiro.Api.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging.Abstractions;

namespace GerenciadorFinanceiro.Tests.Api
{
    public class ExceptionHandlingMiddlewareTests
    {
        [Fact]
        public async Task InvokeAsync_QuandoReceberArgumentException_DeveRetornarBadRequestComMensagemOriginal()
        {
            var context = CreateHttpContext();
            var middleware = CreateMiddleware(_ => throw new ArgumentException("Nao foi possivel salvar uma transacao."));

            await middleware.InvokeAsync(context);

            var response = await ReadResponseAsync(context);

            Assert.Equal(StatusCodes.Status400BadRequest, context.Response.StatusCode);
            Assert.Equal("Nao foi possivel salvar uma transacao.", response.Message);
        }

        [Fact]
        public async Task InvokeAsync_QuandoReceberDbUpdateException_DeveRetornarMensagemAmigavel()
        {
            var context = CreateHttpContext();
            var middleware = CreateMiddleware(_ => throw new DbUpdateException("erro ao salvar", new InvalidOperationException("inner")));

            await middleware.InvokeAsync(context);

            var response = await ReadResponseAsync(context);

            Assert.Equal(StatusCodes.Status500InternalServerError, context.Response.StatusCode);
            Assert.Equal("Nao foi possivel salvar os dados no banco.", response.Message);
        }

        [Fact]
        public async Task InvokeAsync_QuandoReceberDbException_DeveRetornarErroDeConexao()
        {
            var context = CreateHttpContext();
            var middleware = CreateMiddleware(_ => throw new FakeDbException("Falha de conexao"));

            await middleware.InvokeAsync(context);

            var response = await ReadResponseAsync(context);

            Assert.Equal(StatusCodes.Status503ServiceUnavailable, context.Response.StatusCode);
            Assert.Equal("Nao foi possivel conectar ao banco de dados.", response.Message);
        }

        [Fact]
        public async Task InvokeAsync_QuandoReceberKeyNotFoundException_DeveRetornarNotFound()
        {
            var context = CreateHttpContext();
            var middleware = CreateMiddleware(_ => throw new KeyNotFoundException("Transacao nao encontrada."));

            await middleware.InvokeAsync(context);

            var response = await ReadResponseAsync(context);

            Assert.Equal(StatusCodes.Status404NotFound, context.Response.StatusCode);
            Assert.Equal("Transacao nao encontrada.", response.Message);
        }

        [Fact]
        public async Task InvokeAsync_QuandoReceberTimeoutException_DeveRetornarMensagemDeTimeout()
        {
            var context = CreateHttpContext();
            var middleware = CreateMiddleware(_ => throw new TimeoutException("Tempo esgotado."));

            await middleware.InvokeAsync(context);

            var response = await ReadResponseAsync(context);

            Assert.Equal(StatusCodes.Status503ServiceUnavailable, context.Response.StatusCode);
            Assert.Equal("A operacao demorou mais do que o esperado. Tente novamente.", response.Message);
        }

        [Fact]
        public async Task InvokeAsync_QuandoReceberExcecaoGenerica_DeveRetornarErroInternoPadrao()
        {
            var context = CreateHttpContext();
            var middleware = CreateMiddleware(_ => throw new InvalidOperationException("Falha inesperada."));

            await middleware.InvokeAsync(context);

            var response = await ReadResponseAsync(context);

            Assert.Equal(StatusCodes.Status500InternalServerError, context.Response.StatusCode);
            Assert.Equal("Ocorreu um erro interno ao processar a solicitacao.", response.Message);
        }

        [Fact]
        public async Task InvokeAsync_EmDevelopment_DevePreencherDetailsComMensagemTecnica()
        {
            var context = CreateHttpContext();
            var middleware = CreateMiddleware(_ => throw new InvalidOperationException("Detalhe tecnico."), Environments.Development);

            await middleware.InvokeAsync(context);

            var response = await ReadResponseAsync(context);

            Assert.Equal("Detalhe tecnico.", response.Details);
        }

        [Fact]
        public async Task InvokeAsync_EmProduction_NaoDeveExporDetails()
        {
            var context = CreateHttpContext();
            var middleware = CreateMiddleware(_ => throw new InvalidOperationException("Detalhe tecnico."), Environments.Production);

            await middleware.InvokeAsync(context);

            var response = await ReadResponseAsync(context);

            Assert.Null(response.Details);
        }

        private static ExceptionHandlingMiddleware CreateMiddleware(RequestDelegate next, string? environmentName = null)
        {
            environmentName ??= Environments.Development;

            return new ExceptionHandlingMiddleware(
                next,
                NullLogger<ExceptionHandlingMiddleware>.Instance,
                new FakeHostEnvironment(environmentName));
        }

        private static DefaultHttpContext CreateHttpContext()
        {
            var context = new DefaultHttpContext();
            context.Response.Body = new MemoryStream();
            return context;
        }

        private static async Task<ApiErrorResponse> ReadResponseAsync(DefaultHttpContext context)
        {
            context.Response.Body.Position = 0;
            var response = await JsonSerializer.DeserializeAsync<ApiErrorResponse>(context.Response.Body);
            Assert.NotNull(response);
            return response;
        }

        private sealed class FakeDbException : DbException
        {
            public FakeDbException(string message)
                : base(message)
            {
            }
        }

        private sealed class FakeHostEnvironment : IHostEnvironment
        {
            public FakeHostEnvironment(string environmentName)
            {
                EnvironmentName = environmentName;
            }

            public string EnvironmentName { get; set; }

            public string ApplicationName { get; set; } = "GerenciadorFinanceiro.Tests";

            public string ContentRootPath { get; set; } = AppContext.BaseDirectory;

            public IFileProvider ContentRootFileProvider { get; set; } = new NullFileProvider();
        }
    }
}
