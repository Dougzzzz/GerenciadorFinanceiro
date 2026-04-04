using GerenciadorFinanceiro.Domain.Entidades;

namespace GerenciadorFinanceiro.Domain.Interfaces
{
    public interface ITransacaoRepository
    {
        Task AdicionarAsync(Transacao transacao);
        Task<IEnumerable<Transacao>> ObterTodasAsync();
        Task<Transacao?> ObterPorIdAsync(Guid id);
    }
}
