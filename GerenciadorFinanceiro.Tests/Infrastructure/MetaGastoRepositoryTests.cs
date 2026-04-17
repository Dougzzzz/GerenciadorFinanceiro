using GerenciadorFinanceiro.Domain.Entidades;
using GerenciadorFinanceiro.Infrastructure.Data;
using GerenciadorFinanceiro.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace GerenciadorFinanceiro.Tests.Infrastructure
{
    public class MetaGastoRepositoryTests
    {
        [Fact]
        public async Task ObterRecorrente_Deve_Filtrar_Corretamente()
        {
            // Arrange
            using var context = CriarContexto();
            var repository = new MetaGastoRepository(context);
            var catId = Guid.NewGuid();
            var metaRecorrente = new MetaGasto(catId, 500);
            var metaEspecifica = new MetaGasto(catId, 600, 4, 2026);
            
            await repository.AdicionarAsync(metaRecorrente);
            await repository.AdicionarAsync(metaEspecifica);

            // Act
            var encontrada = await repository.ObterRecorrentePorCategoriaAsync(catId);

            // Assert
            Assert.NotNull(encontrada);
            Assert.Null(encontrada.Mes);
            Assert.Equal(500, encontrada.ValorLimite);
        }

        [Fact]
        public async Task ObterEspecifica_Deve_Filtrar_Corretamente()
        {
            // Arrange
            using var context = CriarContexto();
            var repository = new MetaGastoRepository(context);
            var catId = Guid.NewGuid();
            var metaEspecifica = new MetaGasto(catId, 600, 4, 2026);
            await repository.AdicionarAsync(metaEspecifica);

            // Act
            var encontrada = await repository.ObterEspecificaPorCategoriaAsync(catId, 4, 2026);
            var naoEncontrada = await repository.ObterEspecificaPorCategoriaAsync(catId, 5, 2026);

            // Assert
            Assert.NotNull(encontrada);
            Assert.Null(naoEncontrada);
        }

        [Fact]
        public async Task ExcluirAsync_Deve_Remover_Se_Existir()
        {
            // Arrange
            using var context = CriarContexto();
            var repository = new MetaGastoRepository(context);
            var meta = new MetaGasto(Guid.NewGuid(), 100);
            await repository.AdicionarAsync(meta);

            // Act
            await repository.ExcluirAsync(meta.Id);
            var removida = await repository.ObterPorIdAsync(meta.Id);

            // Assert
            Assert.Null(removida);
        }

        private static AppDbContext CriarContexto()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: $"DbMetas_{Guid.NewGuid()}")
                .Options;
            return new AppDbContext(options);
        }
    }
}
