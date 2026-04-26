using System.Text.Json.Serialization;
using GerenciadorFinanceiro.Domain.Entidades;

namespace GerenciadorFinanceiro.Application.DTOs
{
    /// <summary>
    /// DTO para criação ou atualização de um Cartão de Crédito.
    /// </summary>
    public class SaveCartaoDto
    {
        [JsonPropertyName("nome")]
        public string Nome { get; set; } = string.Empty;

        [JsonPropertyName("limite")]
        public decimal Limite { get; set; }

        [JsonPropertyName("diaFechamento")]
        public int DiaFechamento { get; set; }

        [JsonPropertyName("diaVencimento")]
        public int DiaVencimento { get; set; }

        [JsonPropertyName("provedor")]
        public int Provedor { get; set; } = (int)ProvedorExtrato.Generico;
    }
}
