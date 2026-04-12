using GerenciadorFinanceiro.Domain.Entidades;

namespace GerenciadorFinanceiro.Domain.Interfaces
{
    /// <summary>
    /// Repositório para a entidade MetaGasto.
    /// </summary>
    public interface IMetaGastoRepository
    {
        /// <summary>
        /// Obtém uma meta pelo seu identificador único.
        /// </summary>
        /// <returns><placeholder>A <see cref="Task"/> representing the asynchronous operation.</placeholder></returns>
        Task<MetaGasto?> ObterPorIdAsync(Guid id);

        /// <summary>
        /// Obtém a meta recorrente de uma categoria.
        /// </summary>
        /// <returns><placeholder>A <see cref="Task"/> representing the asynchronous operation.</placeholder></returns>
        Task<MetaGasto?> ObterRecorrentePorCategoriaAsync(Guid categoriaId);

        /// <summary>
        /// Obtém a meta específica de uma categoria para um mês e ano.
        /// </summary>
        /// <returns><placeholder>A <see cref="Task"/> representing the asynchronous operation.</placeholder></returns>
        Task<MetaGasto?> ObterEspecificaPorCategoriaAsync(Guid categoriaId, int mes, int ano);

        /// <summary>
        /// Obtém todas as metas cadastradas.
        /// </summary>
        /// <returns><placeholder>A <see cref="Task"/> representing the asynchronous operation.</placeholder></returns>
        Task<IEnumerable<MetaGasto>> ObterTodasAsync();

        /// <summary>
        /// Adiciona uma nova meta.
        /// </summary>
        /// <returns><placeholder>A <see cref="Task"/> representing the asynchronous operation.</placeholder></returns>
        Task AdicionarAsync(MetaGasto meta);

        /// <summary>
        /// Atualiza uma meta existente.
        /// </summary>
        /// <returns><placeholder>A <see cref="Task"/> representing the asynchronous operation.</placeholder></returns>
        Task AtualizarAsync(MetaGasto meta);

        /// <summary>
        /// Exclui uma meta.
        /// </summary>
        /// <returns><placeholder>A <see cref="Task"/> representing the asynchronous operation.</placeholder></returns>
        Task ExcluirAsync(Guid id);
    }
}
