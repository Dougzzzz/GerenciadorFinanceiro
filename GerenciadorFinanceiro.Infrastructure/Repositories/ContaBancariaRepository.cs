using GerenciadorFinanceiro.Domain.Entidades;
using GerenciadorFinanceiro.Domain.Interfaces;
using GerenciadorFinanceiro.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace GerenciadorFinanceiro.Infrastructure.Repositories
{
    public class ContaBancariaRepository : IContaBancariaRepository
    {
        private readonly AppDbContext _context;

        public ContaBancariaRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<ContaBancaria?> ObterPorIdAsync(Guid id) => await _context.ContasBancarias.FindAsync(id);

        public async Task<IEnumerable<ContaBancaria>> ObterTodasAsync() => await _context.ContasBancarias.ToListAsync();

        public async Task AdicionarAsync(ContaBancaria conta)
        {
            await _context.ContasBancarias.AddAsync(conta);
            await _context.SaveChangesAsync();
        }

        public async Task AtualizarAsync(ContaBancaria conta)
        {
            _context.ContasBancarias.Update(conta);
            await _context.SaveChangesAsync();
        }

        public async Task ExcluirAsync(Guid id)
        {
            var conta = await _context.ContasBancarias.FindAsync(id);
            if (conta != null)
            {
                _context.ContasBancarias.Remove(conta);
                await _context.SaveChangesAsync();
            }
        }
    }
}
