using GerenciadorFinanceiro.Domain.Entidades;
using GerenciadorFinanceiro.Domain.Interfaces;
using GerenciadorFinanceiro.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace GerenciadorFinanceiro.Infrastructure.Repositories
{
    public class TransacaoRepository : ITransacaoRepository
    {
        private readonly AppDbContext _context;

        public TransacaoRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task AdicionarAsync(Transacao transacao)
        {
            await _context.Transacoes.AddAsync(transacao);
            await _context.SaveChangesAsync();
        }

        public async Task AtualizarAsync(Transacao transacao)
        {
            _context.Transacoes.Update(transacao);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<Transacao>> ObterTodasAsync() => await _context.Transacoes.AsNoTracking().ToListAsync();

        public async Task<Transacao?> ObterPorIdAsync(Guid id) => await _context.Transacoes.FindAsync(id);

        public async Task ExcluirMuitasAsync(IEnumerable<Guid> ids)
        {
            var transacoes = await _context.Transacoes
                .Where(t => ids.Contains(t.Id))
                .ToListAsync();

            if (transacoes.Count > 0)
            {
                _context.Transacoes.RemoveRange(transacoes);
                await _context.SaveChangesAsync();
            }
        }
    }
}
