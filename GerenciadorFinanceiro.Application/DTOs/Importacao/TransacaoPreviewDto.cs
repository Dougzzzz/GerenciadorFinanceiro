namespace GerenciadorFinanceiro.Application.DTOs.Importacao
{
    /// <summary>
    /// Representa UMA linha do extrato CSV ainda não persistida,
    /// com as sugestões de categoria para o utilizador revisar.
    /// </summary>
    public class TransacaoPreviewDto
    {
        /// <summary>Gets the temporary ID.</summary>
        public Guid IdTemporario { get; init; } = Guid.NewGuid();

        /// <summary>Gets the description.</summary>
        public string Descricao { get; init; } = null!;

        /// <summary>Gets the amount.</summary>
        public decimal Valor { get; init; }

        /// <summary>Gets the date.</summary>
        public DateTime Data { get; init; }

        /// <summary>Gets the original category from CSV.</summary>
        public string? CategoriaOriginalCsv { get; init; }

        /// <summary>Gets the suggested categories.</summary>
        public IReadOnlyList<CategoriaSugestaoDto> CategoriasSugeridas { get; init; } = [];

        /// <summary>Gets or sets the chosen category ID.</summary>
        public Guid? CategoriaEscolhidaId { get; set; }

        /// <summary>Gets or sets the custom new category name.</summary>
        public string? NovaCategoriaPersonalizada { get; set; }
    }
}
