using GerenciadorFinanceiro.Domain.Entidades;

namespace GerenciadorFinanceiro.Domain.Interfaces
{
    public interface ICategoriaRepository
    {
        Task<IEnumerable<Categoria>> ObterTodasAsync();
        Task<Categoria?> ObterPorNomeAsync(string nome, TipoTransacao tipo);
        Task AdicionarAsync(Categoria categoria);
    }
}
