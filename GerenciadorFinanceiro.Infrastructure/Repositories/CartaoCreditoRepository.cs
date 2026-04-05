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

        public async Task<IEnumerable<CartaoCredito>> ObterTodosAsync() => await _context.CartoesDeCredito.ToListAsync();

        public async Task<CartaoCredito?> ObterPorNomeAsync(string nome) => await _context.CartoesDeCredito
                .FirstOrDefaultAsync(c => c.Nome.Equals(nome, StringComparison.CurrentCultureIgnoreCase));

        public async Task AdicionarAsync(CartaoCredito cartao)
        {
            await _context.CartoesDeCredito.AddAsync(cartao);
            await _context.SaveChangesAsync();
        }
    }
}
