using System.Text.Json.Serialization;

namespace GerenciadorFinanceiro.Application.DTOs
{
    /// <summary>
    /// DTO para exibir o resumo de uma meta com o gasto atual.
    /// </summary>
    public class MetaResumoDto
    {
        [JsonPropertyName("categoria")]
        public string Categoria { get; set; } = string.Empty;

        [JsonPropertyName("meta")]
        public decimal Meta { get; set; }

        [JsonPropertyName("gastoAtual")]
        public decimal GastoAtual { get; set; }

        [JsonPropertyName("percentual")]
        public decimal Percentual { get; set; }

        public MetaResumoDto() { }

        public MetaResumoDto(string categoria, decimal meta, decimal gastoAtual, decimal percentual)
        {
            Categoria = categoria;
            Meta = meta;
            GastoAtual = gastoAtual;
            Percentual = percentual;
        }
    }
}
