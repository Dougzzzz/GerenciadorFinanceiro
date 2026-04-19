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

                // Limpa o banco para garantir isolamento
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

            // Assert Preview
            responsePreview.EnsureSuccessStatusCode();
            var previewResult = await responsePreview.Content.ReadFromJsonAsync<ImportacaoPreviewResultadoDto>();

            Assert.NotNull(previewResult);
            Assert.Equal(4, previewResult.Transacoes.Count);

            // Regra C6: Valor 120.50 (positivo no CSV) deve virar -120.50 (negativo no preview)
            Assert.Contains(previewResult.Transacoes, t => t.Descricao == "RESTAURANTE LOCAL" && t.Valor == -120.50m);

            // Regra C6: Pagamento -185.50 (negativo no CSV) deve virar 185.50 (positivo no preview)
            Assert.Contains(previewResult.Transacoes, t => t.Descricao == "PAGAMENTO FATURA" && t.Valor == 185.50m);

            // --- ACT: FASE 2 - Confirmar Importação ---
            // Vamos simular que o usuário aceitou o preview
            var responseConfirmar = await _client.PostAsJsonAsync($"/api/transacoes/importar/confirmar?cartaoId={cartaoId}", previewResult.Transacoes);

            // Assert Confirmação
            responseConfirmar.EnsureSuccessStatusCode();
            var resultadoFinal = await responseConfirmar.Content.ReadFromJsonAsync<ResultadoImportacaoDto>();

            Assert.True(resultadoFinal?.Sucesso);
            Assert.Equal(4, resultadoFinal?.TotalImportado);

            // --- ASSERT FINAL: Validar no Banco de Dados Real (InMemory) ---
            using (var scope = _factory.Services.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                var transacoes = db.Transacoes.ToList();
                var categorias = db.Categorias.ToList();

                Assert.Equal(4, transacoes.Count);

                // Verifica se criou categorias automáticas para as novas descrições
                Assert.Contains(categorias, c => c.Nome == "Serviços");
                Assert.Contains(categorias, c => c.Nome == "Alimentação");

                // Valida persistência dos valores invertidos
                var restaurante = transacoes.First(t => t.Descricao == "RESTAURANTE LOCAL");
                Assert.Equal(-120.50m, restaurante.Valor);
                Assert.Equal(cartaoId, restaurante.CartaoCreditoId);
            }
        }
    }
}
