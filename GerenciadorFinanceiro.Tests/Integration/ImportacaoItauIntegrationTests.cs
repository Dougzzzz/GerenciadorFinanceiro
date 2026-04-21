using System.Net.Http.Json;
using GerenciadorFinanceiro.Application.DTOs.Importacao;
using GerenciadorFinanceiro.Domain.Entidades;
using GerenciadorFinanceiro.Infrastructure.Data;
using Microsoft.Extensions.DependencyInjection;

namespace GerenciadorFinanceiro.Tests.Integration
{
    /// <summary>
    /// Teste de integração para o fluxo completo de importação do Itaú (.xls).
    /// </summary>
    public class ImportacaoItauIntegrationTests : IClassFixture<CustomWebApplicationFactory<Program>>
    {
        private readonly CustomWebApplicationFactory<Program> _factory;
        private readonly HttpClient _client;

        public ImportacaoItauIntegrationTests(CustomWebApplicationFactory<Program> factory)
        {
            _factory = factory;
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task FluxoCompleto_ImportacaoItauXls_DeveProcessarEPersistirCorretamente()
        {
            // --- ARRANGE: Preparar o cenário (Seed) ---
            Guid contaId;
            using (var scope = _factory.Services.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

                db.Database.EnsureDeleted();
                db.Database.EnsureCreated();

                var conta = new ContaBancaria("Itaú Teste", 0, ProvedorExtrato.Itau);
                db.ContasBancarias.Add(conta);
                await db.SaveChangesAsync();
                contaId = conta.Id;
            }

            // --- ACT: FASE 1 - Gerar Preview ---
            // Usamos o arquivo de teste que foi anonimizado ou preparado no TestFiles
            var filePath = Path.Combine(AppContext.BaseDirectory, "TestFiles", "itau-cc-test.xls");

            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException($"Arquivo de teste não encontrado: {filePath}");
            }

            using var stream = File.OpenRead(filePath);
            using var content = new MultipartFormDataContent();
            var fileContent = new StreamContent(stream);

            // O Itaú XLS costuma ser lido como application/vnd.ms-excel
            fileContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/vnd.ms-excel");
            content.Add(fileContent, "arquivo", "extrato_itau.xls");

            var responsePreview = await _client.PostAsync($"/api/transacoes/importar/preview?contaId={contaId}", content);

            responsePreview.EnsureSuccessStatusCode();
            var previewResult = await responsePreview.Content.ReadFromJsonAsync<ImportacaoPreviewResultadoDto>();

            Assert.NotNull(previewResult);

            // No XLS do Itaú real, temos diversas linhas. 
            // O teste deve garantir que pelo menos algumas transações foram identificadas.
            Assert.True(previewResult.Transacoes.Count > 0, "Nenhuma transação encontrada no preview do Itaú.");

            // --- ACT: FASE 2 - Confirmar Importação ---
            // Confirmamos todas as transações do preview
            var responseConfirmar = await _client.PostAsJsonAsync($"/api/transacoes/importar/confirmar?contaId={contaId}", previewResult.Transacoes);

            responseConfirmar.EnsureSuccessStatusCode();
            var resultadoFinal = await responseConfirmar.Content.ReadFromJsonAsync<ResultadoImportacaoDto>();

            Assert.True(resultadoFinal?.Sucesso);
            Assert.Equal(previewResult.Transacoes.Count, resultadoFinal?.TotalImportado);

            // --- ASSERT FINAL: Validar no Banco de Dados Real (InMemory) ---
            using (var scope = _factory.Services.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                var transacoes = db.Transacoes.ToList();

                Assert.Equal(previewResult.Transacoes.Count, transacoes.Count);

                // Verifica se a conta bancária está correta
                Assert.All(transacoes, t => Assert.Equal(contaId, t.ContaBancariaId));

                // Verifica se os valores mantêm o sinal (Itaú XLS já traz Saída negativa)
                var gastos = transacoes.Where(t => t.Valor < 0);
                Assert.True(gastos.Any(), "Deveria haver gastos negativos importados do Itaú.");
            }
        }
    }
}
