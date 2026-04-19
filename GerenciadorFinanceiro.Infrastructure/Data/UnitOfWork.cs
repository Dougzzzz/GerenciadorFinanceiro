using GerenciadorFinanceiro.Domain.Interfaces;
using Microsoft.EntityFrameworkCore.Storage;

namespace GerenciadorFinanceiro.Infrastructure.Data
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly AppDbContext _context;
        private IDbContextTransaction? _transaction;

        public UnitOfWork(AppDbContext context)
        {
            _context = context;
        }

        public async Task IniciarTransacaoAsync() => _transaction = await _context.Database.BeginTransactionAsync();

        public async Task CommitAsync()
        {
            try
            {
                await _context.SaveChangesAsync();
                if (_transaction != null)
                {
                    await _transaction.CommitAsync();
                }
            }
            catch
            {
                await RollbackAsync();
                throw;
            }
        }

        public async Task RollbackAsync()
        {
            if (_transaction != null)
            {
                await _transaction.RollbackAsync();
                await _transaction.DisposeAsync();
                _transaction = null;
            }
        }

        public async Task<int> SalvarAlteracoesAsync() => await _context.SaveChangesAsync();

        public void Dispose()
        {
            _transaction?.Dispose();
            _context.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}
