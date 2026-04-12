using GerenciadorFinanceiro.Domain.Entidades;
using GerenciadorFinanceiro.Domain.Interfaces;
using GerenciadorFinanceiro.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace GerenciadorFinanceiro.Infrastructure.Repositories
{
    /// <summary>
    /// Implementação do repositório para a entidade MetaGasto utilizando Entity Framework Core.
    /// </summary>
    public class MetaGastoRepository : IMetaGastoRepository
    {
        private readonly AppDbContext _context;

        /// <summary>
        /// Initializes a new instance of the <see cref="MetaGastoRepository"/> class.
        /// </summary>
        /// <param name="context">O contexto do banco de dados.</param>
        public MetaGastoRepository(AppDbContext context)
        {
            _context = context;
        }

        /// <inheritdoc/>
        public async Task<MetaGasto?> ObterPorIdAsync(Guid id) => await _context.MetasGastos.FindAsync(id);

        /// <inheritdoc/>
        public async Task<MetaGasto?> ObterRecorrentePorCategoriaAsync(Guid categoriaId) => await _context.MetasGastos
                .FirstOrDefaultAsync(m => m.CategoriaId == categoriaId && m.Mes == null && m.Ano == null);

        /// <inheritdoc/>
        public async Task<MetaGasto?> ObterEspecificaPorCategoriaAsync(Guid categoriaId, int mes, int ano) => await _context.MetasGastos
                .FirstOrDefaultAsync(m => m.CategoriaId == categoriaId && m.Mes == mes && m.Ano == ano);

        /// <inheritdoc/>
        public async Task<IEnumerable<MetaGasto>> ObterTodasAsync() => await _context.MetasGastos.ToListAsync();

        /// <inheritdoc/>
        public async Task AdicionarAsync(MetaGasto meta)
        {
            await _context.MetasGastos.AddAsync(meta);
            await _context.SaveChangesAsync();
        }

        /// <inheritdoc/>
        public async Task AtualizarAsync(MetaGasto meta)
        {
            _context.MetasGastos.Update(meta);
            await _context.SaveChangesAsync();
        }

        /// <inheritdoc/>
        public async Task ExcluirAsync(Guid id)
        {
            var meta = await _context.MetasGastos.FindAsync(id);
            if (meta != null)
            {
                _context.MetasGastos.Remove(meta);
                await _context.SaveChangesAsync();
            }
        }
    }
}
