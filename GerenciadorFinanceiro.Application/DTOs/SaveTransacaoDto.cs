using System.Text.Json.Serialization;

namespace GerenciadorFinanceiro.Application.DTOs
{
    public class SaveTransacaoDto
    {
        [JsonPropertyName("id")]
        public Guid Id { get; set; }

        [JsonPropertyName("data")]
        public DateTime Data { get; set; }

        [JsonPropertyName("descricao")]
        public string Descricao { get; set; } = string.Empty;

        [JsonPropertyName("valor")]
        public decimal Valor { get; set; }

        [JsonPropertyName("categoriaId")]
        public Guid CategoriaId { get; set; }

        [JsonPropertyName("contaBancariaId")]
        public Guid? ContaBancariaId { get; set; }

        [JsonPropertyName("cartaoCreditoId")]
        public Guid? CartaoCreditoId { get; set; }

        [JsonPropertyName("categoria")]
        public string Categoria { get; set; } = string.Empty;

        [JsonPropertyName("nomeCartao")]
        public string NomeCartao { get; set; } = string.Empty;

        [JsonPropertyName("finalCartao")]
        public string FinalCartao { get; set; } = string.Empty;

        [JsonPropertyName("parcela")]
        public string Parcela { get; set; } = string.Empty;

        [JsonPropertyName("cotacao")]
        public decimal Cotacao { get; set; }

        [JsonPropertyName("tipo")]
        public int Tipo { get; set; }
    }
}
