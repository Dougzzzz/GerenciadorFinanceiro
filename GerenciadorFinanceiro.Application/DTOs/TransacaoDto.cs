using System.Text.Json.Serialization;

namespace GerenciadorFinanceiro.Application.DTOs
{
    /// <summary>
    /// DTO para importação de transações.
    /// </summary>
    public class TransacaoDto
    {
        [JsonPropertyName("data")]
        public DateTime Data { get; set; }

        [JsonPropertyName("descricao")]
        public string Descricao { get; set; } = string.Empty;

        [JsonPropertyName("valor")]
        public decimal Valor { get; set; }

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

        public TransacaoDto() { }

        public TransacaoDto(DateTime data, string descricao, decimal valor, string categoria = "", string nomeCartao = "", string finalCartao = "", string parcela = "", decimal cotacao = 0m)
        {
            Data = data;
            Descricao = descricao;
            Valor = valor;
            Categoria = categoria;
            NomeCartao = nomeCartao;
            FinalCartao = finalCartao;
            Parcela = parcela;
            Cotacao = cotacao;
        }
    }
}
