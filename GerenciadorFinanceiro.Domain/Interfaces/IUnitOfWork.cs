namespace GerenciadorFinanceiro.Domain.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        Task IniciarTransacaoAsync();
        Task CommitAsync();
        Task RollbackAsync();
        Task<int> SalvarAlteracoesAsync();
    }
}
