namespace GerenciadorFinanceiro.Application.DTOs.Importacao
{
    /// <summary>
    /// Resultado completo do parsing do CSV, retornado antes da persistência.
    /// </summary>
    public class ImportacaoPreviewResultadoDto
    {
        /// <summary>Gets as transações lidas do CSV.</summary>
        public IReadOnlyList<TransacaoPreviewDto> Transacoes { get; init; } = [];

        /// <summary>Gets o total de linhas com erro.</summary>
        public int LinhasComErro { get; init; }

        /// <summary>Gets o total de transações com sugestão.</summary>
        public int TotalComSugestao => Transacoes.Count(t => t.CategoriasSugeridas.Any());

        /// <summary>Gets o total de transações sem sugestão.</summary>
        public int TotalSemSugestao => Transacoes.Count(t => !t.CategoriasSugeridas.Any());
    }
}
