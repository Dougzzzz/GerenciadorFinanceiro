using GerenciadorFinanceiro.Application.DTOs;
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
        private readonly ICategoriaRepository _categoriaRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="ValidarMetaGastoUseCase"/> class.
        /// </summary>
        /// <param name="metaRepository">O repositório de metas.</param>
        /// <param name="transacaoRepository">O repositório de transações.</param>
        /// <param name="categoriaRepository">O repositório de categorias.</param>
        public ValidarMetaGastoUseCase(
            IMetaGastoRepository metaRepository,
            ITransacaoRepository transacaoRepository,
            ICategoriaRepository categoriaRepository)
        {
            _metaRepository = metaRepository;
            _transacaoRepository = transacaoRepository;
            _categoriaRepository = categoriaRepository;
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

        /// <summary>
        /// Obtém o resumo de todas as metas para o mês e ano informados.
        /// </summary>
        /// <param name="mes">Mês de referência.</param>
        /// <param name="ano">Ano de referência.</param>
        /// <returns>Uma lista de resumos por categoria.</returns>
        public async Task<IEnumerable<MetaResumoDto>> ExecutarResumoMensalAsync(int mes, int ano)
        {
            var todasMetas = await _metaRepository.ObterTodasAsync();
            var categorias = await _categoriaRepository.ObterTodasAsync();

            var resumos = new List<MetaResumoDto>();

            // Filtrar metas relevantes para o mês (específicas do mês ou recorrentes que não tenham específica)
            var metasNoMes = todasMetas
                .GroupBy(m => m.CategoriaId)
                .Select(g => g.FirstOrDefault(m => m.Mes == mes && m.Ano == ano) ?? g.FirstOrDefault(m => m.EhRecorrente))
                .Where(m => m != null)
                .ToList();

            foreach (var meta in metasNoMes)
            {
                var resultado = await ExecutarAsync(meta!.CategoriaId, mes, ano, 0);
                var categoria = categorias.FirstOrDefault(c => c.Id == meta.CategoriaId);
                var nomeCategoria = categoria?.Nome ?? "Sem Categoria";

                resumos.Add(new MetaResumoDto(nomeCategoria, resultado.valorLimite, resultado.totalGasto, resultado.percentualUso));
            }

            return resumos;
        }
    }
}
