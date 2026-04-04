using GerenciadorFinanceiro.Domain.Entidades;

namespace GerenciadorFinanceiro.Domain.Interfaces
{
    public interface ICategoriaRepository
    {
        Task<IEnumerable<Categoria>> ObterTodasAsync();
        Task AdicionarAsync(Categoria categoria);
    }
}
