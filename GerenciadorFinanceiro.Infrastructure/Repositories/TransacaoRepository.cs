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

        public async Task<IEnumerable<Transacao>> ObterTodasAsync()
        {
            return await _context.Transacoes.AsNoTracking().ToListAsync();
        }

        public async Task<Transacao?> ObterPorIdAsync(Guid id)
        {
            return await _context.Transacoes.FindAsync(id);
        }
    }
}
