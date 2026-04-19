using GerenciadorFinanceiro.Application.DTOs.Importacao;
using GerenciadorFinanceiro.Domain.Entidades;

namespace GerenciadorFinanceiro.Application.Interfaces
{
    /// <summary>
    /// Define o contrato para o serviço que compara descrições do CSV
    /// com categorias já existentes no sistema.
    /// </summary>
    public interface ICategoriaSimilaridadeService
    {
        /// <summary>
        /// Busca categorias existentes que são textualmente similares à descrição fornecida.
        /// </summary>
        /// <param name="descricao">Descrição vinda do CSV.</param>
        /// <param name="categorias">Lista de categorias existentes para comparação.</param>
        /// <param name="limiarMinimo">Valor mínimo de similaridade (0 a 1).</param>
        /// <returns>Uma lista de sugestões de categoria ordenadas por similaridade.</returns>
        IEnumerable<CategoriaSugestaoDto> BuscarSimilares(
            string descricao,
            IEnumerable<Categoria> categorias,
            double limiarMinimo = 0.5);
    }
}
