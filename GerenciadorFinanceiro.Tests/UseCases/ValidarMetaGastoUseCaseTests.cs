using GerenciadorFinanceiro.Application.UseCases;
using GerenciadorFinanceiro.Domain.Entidades;
using GerenciadorFinanceiro.Domain.Filtros;
using GerenciadorFinanceiro.Domain.Interfaces;
using NSubstitute;

namespace GerenciadorFinanceiro.Tests.UseCases
{
    public class ValidarMetaGastoUseCaseTests
    {
        private readonly IMetaGastoRepository _metaRepository;
        private readonly ITransacaoRepository _transacaoRepository;
        private readonly ValidarMetaGastoUseCase _useCase;

        public ValidarMetaGastoUseCaseTests()
        {
            _metaRepository = Substitute.For<IMetaGastoRepository>();
            _transacaoRepository = Substitute.For<ITransacaoRepository>();
            _useCase = new ValidarMetaGastoUseCase(_metaRepository, _transacaoRepository);
        }

        [Fact]
        public async Task ExecutarAsync_QuandoGastoExcedeMeta_DeveRetornarAlerta()
        {
            // Arrange
            var categoriaId = Guid.NewGuid();
            var data = new DateTime(2026, 4, 15, 0, 0, 0, DateTimeKind.Utc);
            var meta = new MetaGasto(categoriaId, 1000m); // Meta recorrente de 1000

            _metaRepository.ObterEspecificaPorCategoriaAsync(categoriaId, 4, 2026)
                .Returns(Task.FromResult<MetaGasto?>(null));
            _metaRepository.ObterRecorrentePorCategoriaAsync(categoriaId)
                .Returns(Task.FromResult<MetaGasto?>(meta));

            // Simula que já gastou 900
            var transacoesExistentes = new List<Transacao>
            {
                new(data, "Gasto 1", -900m, categoriaId, Guid.NewGuid(), null),
            };
            _transacaoRepository.ObterTodasAsync(Arg.Any<FiltroTransacao>())
                .Returns(Task.FromResult<IEnumerable<Transacao>>(transacoesExistentes));

            // Act - Novo gasto de 200 (Total 1100, excede 1000)
            var resultado = await _useCase.ExecutarAsync(categoriaId, 4, 2026, -200m);

            // Assert
            Assert.True(resultado.excedeu);
            Assert.Equal(1100m, resultado.totalGasto);
            Assert.Equal(1.1m, resultado.percentualUso); // 110%
        }

        [Fact]
        public async Task ExecutarAsync_SemMetaDefinida_DeveRetornarNaoExcedeu()
        {
            // Arrange
            var categoriaId = Guid.NewGuid();
            _metaRepository.ObterRecorrentePorCategoriaAsync(categoriaId)
                .Returns(Task.FromResult<MetaGasto?>(null));

            // Act
            var resultado = await _useCase.ExecutarAsync(categoriaId, 4, 2026, -200m);

            // Assert
            Assert.False(resultado.excedeu);
            Assert.Equal(0, resultado.percentualUso);
        }
    }
}
