using System.Text.Json.Serialization;

namespace GerenciadorFinanceiro.Application.DTOs
{
    /// <summary>
    /// Representa o total de gastos em uma categoria específica.
    /// </summary>
    public class ResumoCategoriaDto
    {
        /// <summary>Gets or sets o nome da categoria.</summary>
        [JsonPropertyName("categoria")]
        public string Categoria { get; set; } = string.Empty;

        /// <summary>Gets or sets o valor total gasto na categoria (valor absoluto).</summary>
        [JsonPropertyName("valor")]
        public decimal Valor { get; set; }
    }
}
