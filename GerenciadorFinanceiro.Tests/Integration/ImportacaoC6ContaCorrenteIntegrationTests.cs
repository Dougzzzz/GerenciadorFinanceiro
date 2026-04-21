using System.Net.Http.Json;
using GerenciadorFinanceiro.Application.DTOs.Importacao;
using GerenciadorFinanceiro.Domain.Entidades;
using GerenciadorFinanceiro.Infrastructure.Data;
using Microsoft.Extensions.DependencyInjection;

namespace GerenciadorFinanceiro.Tests.Integration
{
    public class ImportacaoC6ContaCorrenteIntegrationTests : IClassFixture<CustomWebApplicationFactory<Program>>
    {
        private readonly CustomWebApplicationFactory<Program> _factory;
        private readonly HttpClient _client;

        public ImportacaoC6ContaCorrenteIntegrationTests(CustomWebApplicationFactory<Program> factory)
        {
            _factory = factory;
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task FluxoCompleto_ImportacaoC6CC_DevePersistirCorretamente()
        {
            // --- ARRANGE: Preparar o cenário (Seed) ---
            Guid contaId;
            using (var scope = _factory.Services.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

                db.Database.EnsureDeleted();
                db.Database.EnsureCreated();

                var conta = new ContaBancaria("C6 Bank CC", 0, ProvedorExtrato.C6Bank);
                db.ContasBancarias.Add(conta);
                await db.SaveChangesAsync();
                contaId = conta.Id;
            }

            // --- ACT: FASE 1 - Gerar Preview ---
            var filePath = Path.Combine(AppContext.BaseDirectory, "TestFiles", "c6-cc-test.csv");
            using var stream = File.OpenRead(filePath);
            using var content = new MultipartFormDataContent();
            var fileContent = new StreamContent(stream);
            fileContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("text/csv");
            content.Add(fileContent, "arquivo", "extrato_c6.csv");

            var responsePreview = await _client.PostAsync($"/api/transacoes/importar/preview?contaId={contaId}", content);

            responsePreview.EnsureSuccessStatusCode();
            var previewResult = await responsePreview.Content.ReadFromJsonAsync<ImportacaoPreviewResultadoDto>();

            Assert.NotNull(previewResult);
            Assert.Equal(5, previewResult.Transacoes.Count);

            // Validação de sinais: Entrada (Positivo), Saída (Negativo)
            Assert.Contains(previewResult.Transacoes, t => t.Descricao.Contains("Pix recebido") && t.Valor == 100.00m);
            Assert.Contains(previewResult.Transacoes, t => t.Descricao.Contains("PGTO FAT CARTAO C6") && t.Valor == -150.00m);

            // --- ACT: FASE 2 - Confirmar Importação ---
            var responseConfirmar = await _client.PostAsJsonAsync($"/api/transacoes/importar/confirmar?contaId={contaId}", previewResult.Transacoes);

            responseConfirmar.EnsureSuccessStatusCode();
            var resultadoFinal = await responseConfirmar.Content.ReadFromJsonAsync<ResultadoImportacaoDto>();

            Assert.True(resultadoFinal?.Sucesso);
            Assert.Equal(5, resultadoFinal?.TotalImportado);

            // --- ASSERT FINAL ---
            using (var scope = _factory.Services.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                var transacoes = db.Transacoes.ToList();

                Assert.Equal(5, transacoes.Count);

                var pix = transacoes.First(t => t.Descricao.Contains("Pix recebido") && t.Valor == 100.00m);
                Assert.Equal(TipoTransacao.Receita, pix.Tipo);

                var fatura = transacoes.First(t => t.Descricao.Contains("PGTO FAT CARTAO C6"));
                Assert.Equal(-150.00m, fatura.Valor);
                Assert.Equal(TipoTransacao.Despesa, fatura.Tipo);
            }
        }
    }
}
