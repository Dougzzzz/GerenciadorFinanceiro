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

        public async Task<IEnumerable<ContaBancaria>> ObterTodasAsync()
        {
            return await _context.ContasBancarias.ToListAsync();
        }

        public async Task AdicionarAsync(ContaBancaria conta)
        {
            await _context.ContasBancarias.AddAsync(conta);
            await _context.SaveChangesAsync();
        }
    }
}
