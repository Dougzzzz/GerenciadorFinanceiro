using GerenciadorFinanceiro.Domain.Entidades;
using GerenciadorFinanceiro.Domain.Filtros;
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

        public async Task<IEnumerable<Transacao>> ObterTodasAsync(FiltroTransacao? filtro = null)
        {
            var query = _context.Transacoes
                .Include(t => t.CategoriaNavigation)
                .Include(t => t.ContaBancariaNavigation)
                .Include(t => t.CartaoCreditoNavigation)
                .AsNoTracking();

            if (filtro != null)
            {
                // Aplica os filtros e ordenação definidos no domínio
                query = filtro.Aplicar(query);
            }
            else
            {
                // Ordenação padrão se nenhum filtro/ordenação for informado
                query = query.OrderByDescending(t => t.Data);
            }

            return await query.ToListAsync();
        }

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

        public async Task<bool> PossuiTransacoesPorCategoriaAsync(Guid categoriaId) => await _context.Transacoes.AnyAsync(t => t.CategoriaId == categoriaId);
    }
}
