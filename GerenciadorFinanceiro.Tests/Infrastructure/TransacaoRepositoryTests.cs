using GerenciadorFinanceiro.Domain.Entidades;
using GerenciadorFinanceiro.Infrastructure.Data;
using GerenciadorFinanceiro.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

namespace GerenciadorFinanceiro.Tests.Infrastructure
{
    public class TransacaoRepositoryTests
    {
        [Fact]
        public async Task AdicionarAsync_DeveSalvarTransacaoNoBanco()
        {
            using var context = CriarContextoEmMemoria();
            var repository = new TransacaoRepository(context);

            var novaTransacao = new Transacao(DateTime.Now, "Compra no Mercado", -50m, Guid.NewGuid(), null, null);

            await repository.AdicionarAsync(novaTransacao);

            var transacaoSalva = await context.Transacoes.FirstOrDefaultAsync(t => t.Id == novaTransacao.Id);

            Assert.NotNull(transacaoSalva);
            Assert.Equal("Compra no Mercado", transacaoSalva.Descricao);
            Assert.Equal(TipoTransacao.Despesa, transacaoSalva.Tipo);
        }

        [Fact]
        public async Task ObterPorIdAsync_DeveRetornarTransacaoCorreta()
        {
            using var context = CriarContextoEmMemoria();
            var repository = new TransacaoRepository(context);

            var transacao = new Transacao(DateTime.Now, "Salário", 5000m, Guid.NewGuid(), null, null);

            context.Transacoes.Add(transacao);
            await context.SaveChangesAsync();

            var transacaoEncontrada = await repository.ObterPorIdAsync(transacao.Id);

            Assert.NotNull(transacaoEncontrada);
            Assert.Equal(5000m, transacaoEncontrada.Valor);
            Assert.Equal(TipoTransacao.Receita, transacaoEncontrada.Tipo);
        }

        [Fact]
        public async Task ExcluirMuitasAsync_DeveRemoverApenasAsTransacoesEspecificadas()
        {
            // Arrange
            using var context = CriarContextoEmMemoria();
            var repository = new TransacaoRepository(context);

            var t1 = new Transacao(DateTime.Now, "T1", -10, Guid.NewGuid(), null, null);
            var t2 = new Transacao(DateTime.Now, "T2", -20, Guid.NewGuid(), null, null);
            var t3 = new Transacao(DateTime.Now, "T3", -30, Guid.NewGuid(), null, null);

            context.Transacoes.AddRange(t1, t2, t3);
            await context.SaveChangesAsync();

            // Act
            await repository.ExcluirMuitasAsync([t1.Id, t2.Id]);

            // Assert
            var restantes = await context.Transacoes.ToListAsync();
            Assert.Single(restantes);
            Assert.Equal(t3.Id, restantes[0].Id);
        }

        [Fact]
        public async Task AtualizarAsync_DevePersistirAlteracoes()
        {
            // Arrange
            using var context = CriarContextoEmMemoria();
            var repository = new TransacaoRepository(context);
            var transacao = new Transacao(DateTime.Now, "Original", 100, Guid.NewGuid(), null, null);
            context.Transacoes.Add(transacao);
            await context.SaveChangesAsync();

            // Act
            typeof(Transacao).GetProperty("Descricao")?.SetValue(transacao, "Editado");
            await repository.AtualizarAsync(transacao);

            // Assert
            var salva = await context.Transacoes.FindAsync(transacao.Id);
            Assert.Equal("Editado", salva?.Descricao);
        }

        // Cria um banco de dados na memória limpo para cada teste
        private static AppDbContext CriarContextoEmMemoria()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()

                // O Guid garante que cada execução do teste ganhe um banco zerado e com nome único
                .UseInMemoryDatabase(databaseName: $"GerenciadorDbTeste_{Guid.NewGuid()}")
                .Options;

            return new AppDbContext(options);
        }
    }
}
