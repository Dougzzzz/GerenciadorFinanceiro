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
        private readonly ICategoriaRepository _categoriaRepository;
        private readonly ValidarMetaGastoUseCase _useCase;

        public ValidarMetaGastoUseCaseTests()
        {
            _metaRepository = Substitute.For<IMetaGastoRepository>();
            _transacaoRepository = Substitute.For<ITransacaoRepository>();
            _categoriaRepository = Substitute.For<ICategoriaRepository>();
            _useCase = new ValidarMetaGastoUseCase(_metaRepository, _transacaoRepository, _categoriaRepository);
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

        [Fact]
        public async Task ExecutarResumoMensalAsync_DeveRetornarResumoDeTodasAsMetas()
        {
            // Arrange
            var cat1Id = Guid.NewGuid();
            var cat2Id = Guid.NewGuid();
            var mes = 4;
            var ano = 2026;

            var categorias = new List<Categoria>
            {
                new("Alimentação", TipoTransacao.Despesa),
                new("Lazer", TipoTransacao.Despesa),
            };

            // Forçando IDs para o mock
            typeof(Categoria).GetProperty("Id") !.SetValue(categorias[0], cat1Id);
            typeof(Categoria).GetProperty("Id") !.SetValue(categorias[1], cat2Id);

            var metas = new List<MetaGasto>
            {
                new(cat1Id, 1000m), // Recorrente
                new(cat2Id, 500m, mes, ano), // Específica para este mês
            };

            _categoriaRepository.ObterTodasAsync().Returns(categorias);
            _metaRepository.ObterTodasAsync().Returns(metas);

            // Mock de gastos individuais para o ExecutarAsync interno
            _metaRepository.ObterEspecificaPorCategoriaAsync(cat1Id, mes, ano).Returns((MetaGasto?)null);
            _metaRepository.ObterRecorrentePorCategoriaAsync(cat1Id).Returns(metas[0]);
            _metaRepository.ObterEspecificaPorCategoriaAsync(cat2Id, mes, ano).Returns(metas[1]);

            // Act
            var resultado = await _useCase.ExecutarResumoMensalAsync(mes, ano);

            // Assert
            Assert.NotNull(resultado);
            var lista = resultado.ToList();
            Assert.Equal(2, lista.Count);
            Assert.Contains(lista, r => r.categoria == "Alimentação" && r.meta == 1000m);
            Assert.Contains(lista, r => r.categoria == "Lazer" && r.meta == 500m);
        }

        [Fact]
        public async Task ExecutarResumoMensalAsync_SemMetas_DeveRetornarListaVazia()
        {
            // Arrange
            _metaRepository.ObterTodasAsync().Returns([]);
            _categoriaRepository.ObterTodasAsync().Returns([]);

            // Act
            var resultado = await _useCase.ExecutarResumoMensalAsync(4, 2026);

            // Assert
            Assert.Empty(resultado);
        }
    }
}
