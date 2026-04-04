using GerenciadorFinanceiro.Domain.Entidades;

namespace GerenciadorFinanceiro.Domain.Interfaces
{
    public interface ICartaoCreditoRepository
    {
        Task<IEnumerable<CartaoCredito>> ObterTodosAsync();
        Task AdicionarAsync(CartaoCredito cartao);
    }
}
