using GerenciadorFinanceiro.Domain.Entidades;

namespace GerenciadorFinanceiro.Domain.Interfaces
{
    public interface IContaBancariaRepository
    {
        Task<IEnumerable<ContaBancaria>> ObterTodasAsync();
        Task AdicionarAsync(ContaBancaria conta);
    }
}
