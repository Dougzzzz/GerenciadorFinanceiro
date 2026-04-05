using GerenciadorFinanceiro.Domain.Entidades;

namespace GerenciadorFinanceiro.Domain.Interfaces
{
    public interface ITransacaoRepository
    {
        Task AdicionarAsync(Transacao transacao);
        Task AtualizarAsync(Transacao transacao);
        Task<IEnumerable<Transacao>> ObterTodasAsync();
        Task<Transacao?> ObterPorIdAsync(Guid id);
        Task ExcluirMuitasAsync(IEnumerable<Guid> ids);
    }
}
