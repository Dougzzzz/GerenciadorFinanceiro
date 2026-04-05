using GerenciadorFinanceiro.Domain.Entidades;

namespace GerenciadorFinanceiro.Domain.Interfaces
{
    public interface ICategoriaRepository
    {
        Task<IEnumerable<Categoria>> ObterTodasAsync();
        Task<Categoria?> ObterPorNomeAsync(string nome, TipoTransacao tipo);
        Task<Categoria?> ObterPorIdAsync(Guid id);
        Task AdicionarAsync(Categoria categoria);
        Task AtualizarAsync(Categoria categoria);
    }
}
