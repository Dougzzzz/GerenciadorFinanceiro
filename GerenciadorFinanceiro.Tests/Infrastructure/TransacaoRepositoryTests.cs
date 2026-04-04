using GerenciadorFinanceiro.Domain.Entidades;
using GerenciadorFinanceiro.Infrastructure.Data;
using GerenciadorFinanceiro.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

namespace GerenciadorFinanceiro.Tests.Infrastructure
{
    public class TransacaoRepositoryTests
    {
        // Cria um banco de dados na memória limpo para cada teste
        private AppDbContext CriarContextoEmMemoria()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                // O Guid garante que cada execução do teste ganhe um banco zerado e com nome único
                .UseInMemoryDatabase(databaseName: $"GerenciadorDbTeste_{Guid.NewGuid()}")
                .Options;

            return new AppDbContext(options);
        }

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
    }
}
