using GerenciadorFinanceiro.Domain.Entidades;

namespace GerenciadorFinanceiro.Domain.Interfaces
{
    public interface ICartaoCreditoRepository
    {
        Task<IEnumerable<CartaoCredito>> ObterTodosAsync();
        Task<CartaoCredito?> ObterPorNomeAsync(string nome);
        Task AdicionarAsync(CartaoCredito cartao);
    }
}
