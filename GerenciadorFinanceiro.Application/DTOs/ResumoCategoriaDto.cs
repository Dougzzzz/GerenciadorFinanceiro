namespace GerenciadorFinanceiro.Application.DTOs
{
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
