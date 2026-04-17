using GerenciadorFinanceiro.Domain.Entidades;
using GerenciadorFinanceiro.Infrastructure.Data;
using GerenciadorFinanceiro.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

namespace GerenciadorFinanceiro.Tests.Infrastructure
{
    public class CartaoCreditoRepositoryTests
    {
        [Fact]
        public async Task ObterPorNomeAsync_Deve_Ignorar_Case()
        {
            // Arrange
            using var context = CriarContexto();
            var repository = new CartaoCreditoRepository(context);
            var cartao = new CartaoCredito("Visa Platinum", 1000, 1, 10);
            await repository.AdicionarAsync(cartao);

            // Act
            var encontrado = await repository.ObterPorNomeAsync("VISA PLATINUM");

            // Assert
            Assert.NotNull(encontrado);
            Assert.Equal(cartao.Id, encontrado.Id);
        }

        [Fact]
        public async Task ObterTodosAsync_Deve_Retornar_Todos()
        {
            // Arrange
            using var context = CriarContexto();
            var repository = new CartaoCreditoRepository(context);
            await repository.AdicionarAsync(new CartaoCredito("C1", 1, 1, 1));
            await repository.AdicionarAsync(new CartaoCredito("C2", 1, 1, 1));

            // Act
            var todos = await repository.ObterTodosAsync();

            // Assert
            Assert.Equal(2, todos.Count());
        }

        private static AppDbContext CriarContexto()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: $"DbCartoes_{Guid.NewGuid()}")
                .Options;
            return new AppDbContext(options);
        }
    }
}
