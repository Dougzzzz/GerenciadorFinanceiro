using GerenciadorFinanceiro.Application.UseCases;
using GerenciadorFinanceiro.Domain.Entidades;
using GerenciadorFinanceiro.Domain.Filtros;
using GerenciadorFinanceiro.Domain.Interfaces;
using NSubstitute;

namespace GerenciadorFinanceiro.Tests.Application
{
    public class ObterResumoMensalUseCaseTests
    {
        private readonly ITransacaoRepository _repositoryMock;
        private readonly ObterResumoMensalUseCase _useCase;

        public ObterResumoMensalUseCaseTests()
        {
            _repositoryMock = Substitute.For<ITransacaoRepository>();
            _useCase = new ObterResumoMensalUseCase(_repositoryMock);
        }

        [Fact]
        public async Task ExecutarAsync_DeveCalcularTotaisCorretamente()
        {
            // Arrange
            var mes = 4;
            var ano = 2026;
            var catId = Guid.NewGuid();

            var transacoes = new List<Transacao>
            {
                new(new DateTime(2026, 4, 10, 0, 0, 0, DateTimeKind.Utc), "Salário", 5000, catId, null, null), // Receita
                new(new DateTime(2026, 4, 15, 0, 0, 0, DateTimeKind.Utc), "Aluguel", -1200, catId, null, null), // Despesa
                new(new DateTime(2026, 4, 20, 0, 0, 0, DateTimeKind.Utc), "Mercado", -300, catId, null, null),   // Despesa
            };

            _repositoryMock.ObterTodasAsync(Arg.Any<FiltroTransacao>()).Returns(transacoes);

            // Act
            var result = await _useCase.ExecutarAsync(mes, ano);

            // Assert
            Assert.Equal(5000, result.TotalReceitas);
            Assert.Equal(1500, result.TotalDespesas); // Deve retornar valor absoluto (positivo)
            Assert.Equal(3500, result.Saldo);         // 5000 - 1500 = 3500
            Assert.Equal(mes, result.Mes);
            Assert.Equal(ano, result.Ano);
        }

        [Fact]
        public async Task ExecutarAsync_QuandoNaoHaTransacoes_DeveRetornarZeros()
        {
            // Arrange
            _repositoryMock.ObterTodasAsync(Arg.Any<FiltroTransacao>()).Returns([]);

            // Act
            var result = await _useCase.ExecutarAsync(1, 2026);

            // Assert
            Assert.Equal(0, result.TotalReceitas);
            Assert.Equal(0, result.TotalDespesas);
            Assert.Equal(0, result.Saldo);
        }
    }
}
