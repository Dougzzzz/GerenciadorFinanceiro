using System.Net;
using System.Net.Http.Json;
using System.Text.Json;

namespace GerenciadorFinanceiro.Tests.Integration
{
    public class ApiContractTests : IClassFixture<CustomWebApplicationFactory<Program>>
    {
        private readonly HttpClient _client;

        public ApiContractTests(CustomWebApplicationFactory<Program> factory)
        {
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task Post_Cartao_With_CamelCase_And_Enum_Int_Should_Return_201()
        {
            // Simula o JSON que o Angular envia (camelCase e int para enum)
            var dados = new
            {
                nome = "Cartão de Teste",
                limite = 5000.00m,
                diaFechamento = 5,
                diaVencimento = 15,
                provedor = 1, // C6Bank
            };

            var response = await _client.PostAsJsonAsync("/api/cartoes", dados);

            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
            var cartao = await response.Content.ReadFromJsonAsync<JsonElement>();
            Assert.Equal("Cartão de Teste", cartao.GetProperty("nome").GetString());
            Assert.Equal(1, cartao.GetProperty("provedor").GetInt32());
        }

        [Fact]
        public async Task Post_Categoria_With_CamelCase_And_Enum_Int_Should_Return_201()
        {
            var dados = new
            {
                nome = "Alimentação",
                tipo = 1, // Despesa
            };

            var response = await _client.PostAsJsonAsync("/api/categorias", dados);

            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
            var categoria = await response.Content.ReadFromJsonAsync<JsonElement>();
            Assert.Equal("Alimentação", categoria.GetProperty("nome").GetString());
            Assert.Equal(1, categoria.GetProperty("tipo").GetInt32());
        }

        [Fact]
        public async Task Post_Transacao_With_CamelCase_Should_Return_201()
        {
            // Primeiro cria uma categoria
            var catResponse = await _client.PostAsJsonAsync("/api/categorias", new { nome = "Lazer", tipo = 1 });
            var catJson = await catResponse.Content.ReadFromJsonAsync<JsonElement>();
            var categoriaId = catJson.GetProperty("id").GetGuid();

            var dados = new
            {
                data = DateTime.UtcNow,
                descricao = "Cinema",
                valor = -50.00m,
                categoriaId,
                tipo = 1, // Despesa
            };

            var response = await _client.PostAsJsonAsync("/api/transacoes", dados);

            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
            var transacao = await response.Content.ReadFromJsonAsync<JsonElement>();
            Assert.Equal("Cinema", transacao.GetProperty("descricao").GetString());
            Assert.Equal(-50.00m, transacao.GetProperty("valor").GetDecimal());
            Assert.Equal(categoriaId, transacao.GetProperty("categoriaId").GetGuid());
        }

        [Fact]
        public async Task Post_MetaGasto_With_CamelCase_Should_Return_201()
        {
            // Primeiro cria uma categoria
            var catResponse = await _client.PostAsJsonAsync("/api/categorias", new { nome = "Viagens", tipo = 1 });
            var catJson = await catResponse.Content.ReadFromJsonAsync<JsonElement>();
            var categoriaId = catJson.GetProperty("id").GetGuid();

            var dados = new
            {
                categoriaId,
                valorLimite = 1000.00m,
                mes = 5,
                ano = 2026,
            };

            var response = await _client.PostAsJsonAsync("/api/metasgastos", dados);

            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
            var meta = await response.Content.ReadFromJsonAsync<JsonElement>();
            Assert.Equal(1000.00m, meta.GetProperty("valorLimite").GetDecimal());
            Assert.Equal(categoriaId, meta.GetProperty("categoriaId").GetGuid());
        }

        [Fact]
        public async Task Get_WithInvalidId_Should_Return_404_With_StandardError()
        {
            var response = await _client.GetAsync($"/api/cartoes/{Guid.NewGuid()}");

            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
            var error = await response.Content.ReadFromJsonAsync<JsonElement>();
            Assert.True(error.TryGetProperty("message", out _));
            Assert.True(error.TryGetProperty("status", out _));
            Assert.Equal(404, error.GetProperty("status").GetInt32());
        }
    }
}
