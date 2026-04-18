using GerenciadorFinanceiro.Domain.Entidades;

namespace GerenciadorFinanceiro.Domain.Interfaces
{
    public interface ICartaoCreditoRepository
    {
        Task<CartaoCredito?> ObterPorIdAsync(Guid id);
        Task<IEnumerable<CartaoCredito>> ObterTodosAsync();
        Task<CartaoCredito?> ObterPorNomeAsync(string nome);
        Task AdicionarAsync(CartaoCredito cartao);
        Task AtualizarAsync(CartaoCredito cartao);
        Task RemoverAsync(Guid id);
    }
}
