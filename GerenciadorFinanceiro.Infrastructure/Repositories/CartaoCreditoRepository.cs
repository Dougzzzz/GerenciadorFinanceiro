using GerenciadorFinanceiro.Domain.Entidades;
using GerenciadorFinanceiro.Domain.Interfaces;
using GerenciadorFinanceiro.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace GerenciadorFinanceiro.Infrastructure.Repositories
{
    public class CartaoCreditoRepository : ICartaoCreditoRepository
    {
        private readonly AppDbContext _context;

        public CartaoCreditoRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<CartaoCredito?> ObterPorIdAsync(Guid id) => await _context.CartoesDeCredito.FindAsync(id);

        public async Task<IEnumerable<CartaoCredito>> ObterTodosAsync() => await _context.CartoesDeCredito.ToListAsync();

        public async Task<CartaoCredito?> ObterPorNomeAsync(string nome) => await _context.CartoesDeCredito
                .FirstOrDefaultAsync(c => c.Nome.ToLower() == nome.ToLower());

        public async Task AdicionarAsync(CartaoCredito cartao)
        {
            await _context.CartoesDeCredito.AddAsync(cartao);
            await _context.SaveChangesAsync();
        }

        public async Task AtualizarAsync(CartaoCredito cartao)
        {
            _context.CartoesDeCredito.Update(cartao);
            await _context.SaveChangesAsync();
        }

        public async Task RemoverAsync(Guid id)
        {
            var cartao = await ObterPorIdAsync(id);
            if (cartao != null)
            {
                _context.CartoesDeCredito.Remove(cartao);
                await _context.SaveChangesAsync();
            }
        }
    }
}
