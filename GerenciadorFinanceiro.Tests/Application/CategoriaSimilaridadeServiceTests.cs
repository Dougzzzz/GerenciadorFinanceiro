using GerenciadorFinanceiro.Domain.Entidades;
using GerenciadorFinanceiro.Infrastructure.Services;

namespace GerenciadorFinanceiro.Tests.Application
{
    public class CategoriaSimilaridadeServiceTests
    {
        private readonly LevenshteinSimilaridadeService _service = new();

        [Fact]
        public void BuscarSimilares_ComNomeIdentico_DeveRetornarSimilaridadeMaxima()
        {
            var categorias = new List<Categoria>
            {
                new("Mercado", TipoTransacao.Despesa),
            };

            var sugestoes = _service.BuscarSimilares("Mercado", categorias);

            Assert.Single(sugestoes);
            Assert.Equal(1.0, sugestoes.First().Similaridade, precision: 2);
        }

        [Fact]
        public void BuscarSimilares_ComDiferencaDeMaiusculas_DeveEncontrarSimilar()
        {
            var categorias = new List<Categoria>
            {
                new("Mercado", TipoTransacao.Despesa),
            };

            var sugestoes = _service.BuscarSimilares("MERCADO EXTRA S/A", categorias, limiarMinimo: 0.5);

            Assert.NotEmpty(sugestoes);
            Assert.Contains(sugestoes, s => s.NomeCategoria == "Mercado");
        }

        [Fact]
        public void BuscarSimilares_ComNomeCompletamenteDiferente_DeveRetornarListaVazia()
        {
            var categorias = new List<Categoria>
            {
                new("Aluguel", TipoTransacao.Despesa),
            };

            var sugestoes = _service.BuscarSimilares("Netflix", categorias, limiarMinimo: 0.6);

            Assert.Empty(sugestoes);
        }

        [Fact]
        public void BuscarSimilares_ComMultiplasCategorias_DeveOrdenarPorSimilaridade()
        {
            var categorias = new List<Categoria>
            {
                new("Supermercado", TipoTransacao.Despesa),
                new("Mercado", TipoTransacao.Despesa),
                new("Aluguel", TipoTransacao.Despesa),
            };

            var sugestoes = _service
                .BuscarSimilares("Mercado Municipal", categorias, limiarMinimo: 0.4)
                .ToList();

            Assert.Equal("Mercado", sugestoes.First().NomeCategoria);
        }
    }
}
