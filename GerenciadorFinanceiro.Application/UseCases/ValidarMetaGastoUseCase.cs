using GerenciadorFinanceiro.Domain.Filtros;
using GerenciadorFinanceiro.Domain.Interfaces;

namespace GerenciadorFinanceiro.Application.UseCases
{
    /// <summary>
    /// Resultado da validação de um gasto contra a meta definida.
    /// </summary>
    /// <param name="excedeu">Indica se o limite foi ultrapassado.</param>
    /// <param name="valorLimite">O valor limite definido.</param>
    /// <param name="totalGasto">O valor total gasto no período (incluindo o novo gasto).</param>
    /// <param name="percentualUso">A percentagem do limite que foi consumida (1.0 = 100%).</param>
    public record ResultadoValidacaoMeta(bool excedeu, decimal valorLimite, decimal totalGasto, decimal percentualUso);

    /// <summary>
    /// Caso de uso para validar se um novo gasto ultrapassa o limite da categoria.
    /// </summary>
    public class ValidarMetaGastoUseCase
    {
        private readonly IMetaGastoRepository _metaRepository;
        private readonly ITransacaoRepository _transacaoRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="ValidarMetaGastoUseCase"/> class.
        /// </summary>
        /// <param name="metaRepository">O repositório de metas.</param>
        /// <param name="transacaoRepository">O repositório de transações.</param>
        public ValidarMetaGastoUseCase(IMetaGastoRepository metaRepository, ITransacaoRepository transacaoRepository)
        {
            _metaRepository = metaRepository;
            _transacaoRepository = transacaoRepository;
        }

        /// <summary>
        /// Executa a validação do gasto contra a meta da categoria no mês/ano informados.
        /// </summary>
        /// <param name="categoriaId">ID da categoria.</param>
        /// <param name="mes">Mês do gasto.</param>
        /// <param name="ano">Ano do gasto.</param>
        /// <param name="valorNovoGasto">O valor da nova transação (espera-se negativo para despesas).</param>
        /// <returns>O resultado da validação com indicadores de excesso.</returns>
        public async Task<ResultadoValidacaoMeta> ExecutarAsync(Guid categoriaId, int mes, int ano, decimal valorNovoGasto)
        {
            // Busca meta específica ou recorrente
            var meta = await _metaRepository.ObterEspecificaPorCategoriaAsync(categoriaId, mes, ano)
                       ?? await _metaRepository.ObterRecorrentePorCategoriaAsync(categoriaId);

            if (meta == null)
            {
                return new ResultadoValidacaoMeta(false, 0, 0, 0);
            }

            // Define o período (primeiro e último dia do mês)
            var dataInicial = new DateTime(ano, mes, 1, 0, 0, 0, DateTimeKind.Utc);
            var dataFinal = dataInicial.AddMonths(1).AddDays(-1);

            // Busca transações já realizadas no período para esta categoria
            var filtro = new FiltroTransacao
            {
                CategoriaId = categoriaId,
                DataInicial = dataInicial,
                DataFinal = dataFinal,
            };

            var transacoes = await _transacaoRepository.ObterTodasAsync(filtro);

            // Soma apenas despesas (valores negativos)
            // Lembre-se: No sistema, despesas são armazenadas como negativas.
            decimal totalGastoNoMes = Math.Abs(transacoes.Where(t => t.Valor < 0).Sum(t => t.Valor)) + Math.Abs(valorNovoGasto);

            bool excedeu = totalGastoNoMes > meta.ValorLimite;
            decimal percentual = totalGastoNoMes / meta.ValorLimite;

            return new ResultadoValidacaoMeta(excedeu, meta.ValorLimite, totalGastoNoMes, percentual);
        }
    }
}
