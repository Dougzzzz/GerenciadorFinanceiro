using GerenciadorFinanceiro.Domain.Entidades;
using GerenciadorFinanceiro.Domain.Filtros;
using Xunit;

namespace GerenciadorFinanceiro.Tests.Domain
{
    public class SanitizeDateTests
    {
        [Fact]
        public void Transacao_AoCriarComDataLocal_DeveConverterParaUtc()
        {
            // Arrange
            var dataLocal = new DateTime(2023, 10, 27, 10, 0, 0, DateTimeKind.Unspecified);
            var categoriaId = Guid.NewGuid();

            // Act
            var transacao = new Transacao(dataLocal, "Teste", -100m, categoriaId, null, null);

            // Assert
            Assert.Equal(DateTimeKind.Utc, transacao.Data.Kind);
            Assert.Equal(dataLocal.Year, transacao.Data.Year);
            Assert.Equal(dataLocal.Month, transacao.Data.Month);
            Assert.Equal(dataLocal.Day, transacao.Data.Day);
        }

        [Fact]
        public void Transacao_AoAtualizarComDataLocal_DeveConverterParaUtc()
        {
            // Arrange
            var transacao = new Transacao(DateTime.UtcNow, "Original", -100m, Guid.NewGuid(), null, null);
            var dataLocal = new DateTime(2023, 11, 15, 14, 30, 0, DateTimeKind.Unspecified);

            // Act
            transacao.Atualizar(dataLocal, "Atualizada", -200m, transacao.CategoriaId, null, null, "Cat", "Nome", "1234", "1/1", 0);

            // Assert
            Assert.Equal(DateTimeKind.Utc, transacao.Data.Kind);
        }

        [Fact]
        public void FiltroTransacao_AoAplicarComDatasLocais_NaoDeveFalharEUsarUtc()
        {
            // Arrange
            var dataInicialLocal = new DateTime(2023, 01, 01, 0, 0, 0, DateTimeKind.Unspecified);
            var dataFinalLocal = new DateTime(2023, 12, 31, 23, 59, 59, DateTimeKind.Unspecified);
            
            var filtro = new FiltroTransacao
            {
                DataInicial = dataInicialLocal,
                DataFinal = dataFinalLocal
            };

            // Criamos uma lista mock para testar o IQueryable
            var transacoes = new List<Transacao>
            {
                new Transacao(new DateTime(2023, 06, 15, 0, 0, 0, DateTimeKind.Utc), "Dentro", -50m, Guid.NewGuid(), null, null)
            }.AsQueryable();

            // Act
            // O teste aqui é garantir que o método Aplicar não lança exceção e processa a lógica
            var queryResult = filtro.Aplicar(transacoes);
            var result = queryResult.ToList();

            // Assert
            Assert.Single(result);
            Assert.Equal("Dentro", result[0].Descricao);
        }

        [Fact]
        public void FiltroTransacao_AoFiltrarMesmoDia_DeveEncontrarTransacoesDoDiaTodo()
        {
            // Arrange
            var dia = new DateTime(2026, 05, 05, 0, 0, 0, DateTimeKind.Unspecified);
            
            var filtro = new FiltroTransacao
            {
                DataInicial = dia,
                DataFinal = dia
            };

            var transacoes = new List<Transacao>
            {
                // Transação às 15:30 do mesmo dia
                new Transacao(new DateTime(2026, 05, 05, 15, 30, 0, DateTimeKind.Utc), "Almoço", -50m, Guid.NewGuid(), null, null)
            }.AsQueryable();

            // Act
            var result = filtro.Aplicar(transacoes).ToList();

            // Assert
            Assert.Single(result);
            Assert.Equal("Almoço", result[0].Descricao);
        }
    }
}
