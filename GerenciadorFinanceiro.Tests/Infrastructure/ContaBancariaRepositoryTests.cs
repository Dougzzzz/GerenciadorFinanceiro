using GerenciadorFinanceiro.Domain.Entidades;
using GerenciadorFinanceiro.Infrastructure.Data;
using GerenciadorFinanceiro.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace GerenciadorFinanceiro.Tests.Infrastructure
{
    public class ContaBancariaRepositoryTests
    {
        [Fact]
        public async Task Adicionar_E_ObterPorId_Deve_Funcionar()
        {
            // Arrange
            using var context = CriarContexto();
            var repository = new ContaBancariaRepository(context);
            var conta = new ContaBancaria("Nubank", 100);

            // Act
            await repository.AdicionarAsync(conta);
            var encontrada = await repository.ObterPorIdAsync(conta.Id);

            // Assert
            Assert.NotNull(encontrada);
            Assert.Equal("Nubank", encontrada.NomeBanco);
        }

        [Fact]
        public async Task ObterTodasAsync_Deve_Retornar_Todas_As_Contas()
        {
            // Arrange
            using var context = CriarContexto();
            var repository = new ContaBancariaRepository(context);
            await repository.AdicionarAsync(new ContaBancaria("C1"));
            await repository.AdicionarAsync(new ContaBancaria("C2"));

            // Act
            var todas = await repository.ObterTodasAsync();

            // Assert
            Assert.Equal(2, todas.Count());
        }

        private static AppDbContext CriarContexto()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: $"DbContas_{Guid.NewGuid()}")
                .Options;
            return new AppDbContext(options);
        }
    }
}
