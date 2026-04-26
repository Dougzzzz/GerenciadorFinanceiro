using System.Text.Json.Serialization;

namespace GerenciadorFinanceiro.Application.DTOs
{
    /// <summary>
    /// DTO para criação ou atualização de uma Meta de Gasto.
    /// </summary>
    public class SaveMetaGastoDto
    {
        [JsonPropertyName("categoriaId")]
        public Guid CategoriaId { get; set; }

        [JsonPropertyName("valorLimite")]
        public decimal ValorLimite { get; set; }

        [JsonPropertyName("mes")]
        public int? Mes { get; set; }

        [JsonPropertyName("ano")]
        public int? Ano { get; set; }
    }
}
