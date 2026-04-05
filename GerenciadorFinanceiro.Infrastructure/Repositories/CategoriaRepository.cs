using GerenciadorFinanceiro.Domain.Entidades;
using GerenciadorFinanceiro.Domain.Interfaces;
using GerenciadorFinanceiro.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace GerenciadorFinanceiro.Infrastructure.Repositories
{
    public class CategoriaRepository : ICategoriaRepository
    {
        private readonly AppDbContext _context;

        public CategoriaRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Categoria>> ObterTodasAsync() => await _context.Categorias.ToListAsync();

        public async Task<Categoria?> ObterPorNomeAsync(string nome, TipoTransacao tipo) => await _context.Categorias
                .FirstOrDefaultAsync(c => c.Nome.Equals(nome, StringComparison.CurrentCultureIgnoreCase) && c.Tipo == tipo);

        public async Task AdicionarAsync(Categoria categoria)
        {
            await _context.Categorias.AddAsync(categoria);
            await _context.SaveChangesAsync();
        }
    }
}
