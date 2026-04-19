namespace GerenciadorFinanceiro.Application.DTOs.Importacao
{
    /// <summary>
    /// Representa uma sugestão de categoria existente para uma linha do extrato.
    /// </summary>
    public class CategoriaSugestaoDto
    {
        /// <summary>Gets o ID da categoria existente no banco de dados.</summary>
        public Guid CategoriaId { get; init; }

        /// <summary>Gets o nome da categoria existente.</summary>  
        public string NomeCategoria { get; init; } = null!;

        /// <summary>
        /// Gets o percentual de similaridade entre 0.0 (nada similar) e 1.0 (idêntico).
        /// </summary>
        public double Similaridade { get; init; }

        /// <summary>Gets a similaridade formatada como percentual legível.</summary>
        public string SimilaridadeFormatada => $"{Similaridade:P0}";
    }
}
