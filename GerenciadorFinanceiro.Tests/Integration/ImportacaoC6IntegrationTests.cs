using System.Net.Http.Json;
using GerenciadorFinanceiro.Application.DTOs.Importacao;
using GerenciadorFinanceiro.Domain.Entidades;
using GerenciadorFinanceiro.Infrastructure.Data;
using Microsoft.Extensions.DependencyInjection;

namespace GerenciadorFinanceiro.Tests.Integration
{
    public class ImportacaoC6IntegrationTests : IClassFixture<CustomWebApplicationFactory<Program>>
    {
        private readonly CustomWebApplicationFactory<Program> _factory;
        private readonly HttpClient _client;

        public ImportacaoC6IntegrationTests(CustomWebApplicationFactory<Program> factory)
        {
            _factory = factory;
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task FluxoCompleto_ImportacaoC6_DeveInverterSinaisEPersistirCorretamente()
        {
            // --- ARRANGE: Preparar o cenário (Seed) ---
            Guid cartaoId;
            using (var scope = _factory.Services.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

                db.Database.EnsureDeleted();
                db.Database.EnsureCreated();

                var cartao = new CartaoCredito("C6 Bank Teste", 10000, 10, 20, ProvedorExtrato.C6Bank);
                db.CartoesDeCredito.Add(cartao);
                await db.SaveChangesAsync();
                cartaoId = cartao.Id;
            }

            // --- ACT: FASE 1 - Gerar Preview ---
            var filePath = Path.Combine(AppContext.BaseDirectory, "TestFiles", "fatura-c6-test.csv");
            using var stream = File.OpenRead(filePath);
            using var content = new MultipartFormDataContent();
            var fileContent = new StreamContent(stream);
            fileContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("text/csv");
            content.Add(fileContent, "arquivo", "fatura.csv");

            var responsePreview = await _client.PostAsync($"/api/transacoes/importar/preview?cartaoId={cartaoId}", content);

            responsePreview.EnsureSuccessStatusCode();
            var previewResult = await responsePreview.Content.ReadFromJsonAsync<ImportacaoPreviewResultadoDto>();

            Assert.NotNull(previewResult);
            Assert.Equal(10, previewResult.Transacoes.Count);

            // Validação de sinais invertidos (Regra C6)
            // Gastos (Positivos no CSV) viram Negativos no Preview
            Assert.Contains(previewResult.Transacoes, t => t.Descricao.Contains("LANCHONETE") && t.Valor == -89.10m);

            // Pagamentos (Negativos no CSV) viram Positivos no Preview
            Assert.Contains(previewResult.Transacoes, t => t.Descricao.Contains("Inclusao de Pagamento") && t.Valor == 5465.99m);

            // --- ACT: FASE 2 - Confirmar Importação ---
            var responseConfirmar = await _client.PostAsJsonAsync($"/api/transacoes/importar/confirmar?cartaoId={cartaoId}", previewResult.Transacoes);

            responseConfirmar.EnsureSuccessStatusCode();
            var resultadoFinal = await responseConfirmar.Content.ReadFromJsonAsync<ResultadoImportacaoDto>();

            Assert.True(resultadoFinal?.Sucesso);
            Assert.Equal(10, resultadoFinal?.TotalImportado);

            // --- ASSERT FINAL: Validar no Banco de Dados Real (InMemory) ---
            using (var scope = _factory.Services.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                var transacoes = db.Transacoes.ToList();
                var categorias = db.Categorias.ToList();

                Assert.Equal(10, transacoes.Count);
                Assert.Contains(categorias, c => c.Nome.Contains("Restaurante"));

                var lanche = transacoes.First(t => t.Descricao.Contains("LANCHONETE"));
                Assert.Equal(-89.10m, lanche.Valor);

                var pagamento = transacoes.First(t => t.Descricao.Contains("Inclusao de Pagamento") && t.Valor == 5465.99m);
                Assert.Equal(TipoTransacao.Receita, pagamento.Tipo);
            }
        }
    }
}
