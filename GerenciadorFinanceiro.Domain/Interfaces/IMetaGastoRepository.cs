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
        /// <param name="id">O identificador único da meta.</param>
        /// <returns>A meta encontrada ou null.</returns>
        Task<MetaGasto?> ObterPorIdAsync(Guid id);

        /// <summary>
        /// Obtém a meta recorrente de uma categoria.
        /// </summary>
        /// <param name="categoriaId">O identificador da categoria.</param>
        /// <returns>A meta recorrente ou null.</returns>
        Task<MetaGasto?> ObterRecorrentePorCategoriaAsync(Guid categoriaId);

        /// <summary>
        /// Obtém a meta específica de uma categoria para um mês e ano.
        /// </summary>
        /// <param name="categoriaId">O identificador da categoria.</param>
        /// <param name="mes">O mês da meta.</param>
        /// <param name="ano">O ano da meta.</param>
        /// <returns>A meta específica ou null.</returns>
        Task<MetaGasto?> ObterEspecificaPorCategoriaAsync(Guid categoriaId, int mes, int ano);

        /// <summary>
        /// Obtém todas as metas cadastradas.
        /// </summary>
        /// <returns>Uma lista com todas as metas.</returns>
        Task<IEnumerable<MetaGasto>> ObterTodasAsync();

        /// <summary>
        /// Adiciona uma nova meta.
        /// </summary>
        /// <param name="meta">A entidade meta a ser adicionada.</param>
        /// <returns>Uma tarefa representando a operação assíncrona.</returns>
        Task AdicionarAsync(MetaGasto meta);

        /// <summary>
        /// Atualiza uma meta existente.
        /// </summary>
        /// <param name="meta">A entidade meta com os dados atualizados.</param>
        /// <returns>Uma tarefa representando a operação assíncrona.</returns>
        Task AtualizarAsync(MetaGasto meta);

        /// <summary>
        /// Exclui uma meta.
        /// </summary>
        /// <param name="id">O identificador único da meta a ser excluída.</param>
        /// <returns>Uma tarefa representando a operação assíncrona.</returns>
        Task ExcluirAsync(Guid id);
    }
}
