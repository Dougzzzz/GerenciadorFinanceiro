namespace GerenciadorFinanceiro.Application.DTOs
{
    /// <summary>
    /// Transporta o resumo financeiro consolidado de um determinado período.
    /// </summary>
    public class ResumoMensalDto
    {
        /// <summary>
        /// Gets or sets o somatório de todas as receitas (valores positivos).
        /// </summary>
        public decimal TotalReceitas { get; set; }

        /// <summary>
        /// Gets or sets o somatório de todas as despesas (valores negativos, retornados como valor absoluto para exibição).
        /// </summary>
        public decimal TotalDespesas { get; set; }

        /// <summary>
        /// Gets or sets o saldo final (Receitas + Despesas).
        /// </summary>
        public decimal Saldo { get; set; }

        /// <summary>
        /// Gets or sets o mês de referência do resumo.
        /// </summary>
        public int Mes { get; set; }

        /// <summary>
        /// Gets or sets o ano de referência do resumo.
        /// </summary>
        public int Ano { get; set; }

        /// <summary>
        /// Gets or sets a distribuição de gastos por categoria.
        /// </summary>
        public List<ResumoCategoriaDto> GastosPorCategoria { get; set; } = new();
    }

    /// <summary>
    /// Representa o total de gastos em uma categoria específica.
    /// </summary>
    public class ResumoCategoriaDto
    {
        /// <summary>Gets or sets o nome da categoria.</summary>
        public string Categoria { get; set; } = string.Empty;

        /// <summary>Gets or sets o valor total gasto na categoria (valor absoluto).</summary>
        public decimal Valor { get; set; }
    }
}
