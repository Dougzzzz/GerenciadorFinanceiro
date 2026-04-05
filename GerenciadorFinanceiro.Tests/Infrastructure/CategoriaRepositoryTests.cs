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

        [Fact]
        public async Task ObterPorIdAsync_DeveRetornarCategoriaExistente()
        {
            // Arrange
            using var context = CriarContexto();
            var repository = new CategoriaRepository(context);
            var categoria = new Categoria("Aluguel", TipoTransacao.Despesa);
            await repository.AdicionarAsync(categoria);

            // Act
            var encontrada = await repository.ObterPorIdAsync(categoria.Id);

            // Assert
            Assert.NotNull(encontrada);
            Assert.Equal(categoria.Nome, encontrada.Nome);
        }

        [Fact]
        public async Task AtualizarAsync_DeveModificarCategoriaExistente()
        {
            // Arrange
            using var context = CriarContexto();
            var repository = new CategoriaRepository(context);
            var categoria = new Categoria("Original", TipoTransacao.Despesa);
            await repository.AdicionarAsync(categoria);

            // Act
            categoria.Atualizar("Atualizada", TipoTransacao.Receita);
            await repository.AtualizarAsync(categoria);

            // Assert
            var noDb = await repository.ObterPorIdAsync(categoria.Id);
            Assert.NotNull(noDb);
            Assert.Equal("Atualizada", noDb.Nome);
            Assert.Equal(TipoTransacao.Receita, noDb.Tipo);
        }

        [Fact]
        public async Task ExcluirMuitasAsync_DeveRemoverCategorias()
        {
            // Arrange
            using var context = CriarContexto();
            var repository = new CategoriaRepository(context);
            var cat1 = new Categoria("C1", TipoTransacao.Despesa);
            var cat2 = new Categoria("C2", TipoTransacao.Despesa);
            await repository.AdicionarAsync(cat1);
            await repository.AdicionarAsync(cat2);

            // Act
            await repository.ExcluirMuitasAsync([cat1.Id, cat2.Id]);

            // Assert
            var todas = await repository.ObterTodasAsync();
            Assert.Empty(todas);
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
