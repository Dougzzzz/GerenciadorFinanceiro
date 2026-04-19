using GerenciadorFinanceiro.Domain.Entidades;
using GerenciadorFinanceiro.Domain.Filtros;

namespace GerenciadorFinanceiro.Domain.Interfaces
{
    public interface ITransacaoRepository
    {
        Task AdicionarAsync(Transacao transacao);
        Task AtualizarAsync(Transacao transacao);
        Task<IEnumerable<Transacao>> ObterTodasAsync(FiltroTransacao? filtro = null);
        Task<Transacao?> ObterPorIdAsync(Guid id);
        Task ExcluirMuitasAsync(IEnumerable<Guid> ids);
        Task<bool> PossuiTransacoesPorCategoriaAsync(Guid categoriaId);
        Task<bool> ExisteChaveExclusivaAsync(string chaveExclusiva);
    }
}
