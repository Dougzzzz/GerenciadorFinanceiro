using GerenciadorFinanceiro.Domain.Entidades;
using GerenciadorFinanceiro.Infrastructure.Data;
using GerenciadorFinanceiro.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

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

        [Fact]
        public async Task AtualizarAsync_Deve_Persistir_Alteracoes()
        {
            // Arrange
            using var context = CriarContexto();
            var repository = new ContaBancariaRepository(context);
            var conta = new ContaBancaria("Original", 100);
            await repository.AdicionarAsync(conta);

            // Act
            conta.AtualizarDados("Alterada", 200, ProvedorExtrato.C6Bank);
            await repository.AtualizarAsync(conta);

            // Assert
            var noDb = await repository.ObterPorIdAsync(conta.Id);
            Assert.NotNull(noDb);
            Assert.Equal("Alterada", noDb.NomeBanco);
            Assert.Equal(200, noDb.SaldoAtual);
            Assert.Equal(ProvedorExtrato.C6Bank, noDb.Provedor);
        }

        [Fact]
        public async Task ExcluirAsync_Deve_Remover_Conta()
        {
            // Arrange
            using var context = CriarContexto();
            var repository = new ContaBancariaRepository(context);
            var conta = new ContaBancaria("Para Deletar");
            await repository.AdicionarAsync(conta);

            // Act
            await repository.ExcluirAsync(conta.Id);

            // Assert
            var noDb = await repository.ObterPorIdAsync(conta.Id);
            Assert.Null(noDb);
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
