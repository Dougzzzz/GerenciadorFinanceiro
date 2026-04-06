namespace GerenciadorFinanceiro.Domain.Filtros
{
    /// <summary>
    /// Classe base para filtros do sistema, contendo propriedades comuns como ordenação.
    /// </summary>
    public abstract class FiltroBase
    {
        /// <summary>
        /// Gets or sets o nome da propriedade pela qual os dados devem ser ordenados.
        /// </summary>
        public string? OrdenarPor { get; set; }

        /// <summary>
        /// Gets or sets a direção da ordenação: "Asc" para Ascendente ou "Desc" para Descendente.
        /// </summary>
        public string? Direcao { get; set; }
    }
}
