using GerenciadorFinanceiro.Domain.Entidades;
using GerenciadorFinanceiro.Infrastructure.Data;
using GerenciadorFinanceiro.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

namespace GerenciadorFinanceiro.Tests.Infrastructure
{
    public class CategoriaRepositoryTests
    {
        [Fact]
        public async Task ObterPorNomeAsync_DeveSerCaseInsensitive()
        {
            // Arrange
            using var context = CriarContexto();
            var repository = new CategoriaRepository(context);
            var categoria = new Categoria("Alimentação", TipoTransacao.Despesa);
            await repository.AdicionarAsync(categoria);

            // Act
            var encontrada = await repository.ObterPorNomeAsync("ALIMENTAÇÃO", TipoTransacao.Despesa);
            var encontradaMinusculo = await repository.ObterPorNomeAsync("alimentação", TipoTransacao.Despesa);

            // Assert
            Assert.NotNull(encontrada);
            Assert.NotNull(encontradaMinusculo);
            Assert.Equal(categoria.Id, encontrada.Id);
            Assert.Equal(categoria.Id, encontradaMinusculo.Id);
        }

        [Fact]
        public async Task ObterPorNomeAsync_ComTipoDiferente_DeveRetornarNulo()
        {
            // Arrange
            using var context = CriarContexto();
            var repository = new CategoriaRepository(context);
            var categoria = new Categoria("Salário", TipoTransacao.Receita);
            await repository.AdicionarAsync(categoria);

            // Act
            var encontrada = await repository.ObterPorNomeAsync("Salário", TipoTransacao.Despesa);

            // Assert
            Assert.Null(encontrada);
        }

        private static AppDbContext CriarContexto()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: $"DbCategorias_{Guid.NewGuid()}")
                .Options;
            return new AppDbContext(options);
        }
    }
}
