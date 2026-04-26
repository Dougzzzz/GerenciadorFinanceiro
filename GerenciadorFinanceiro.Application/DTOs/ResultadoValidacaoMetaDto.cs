using System.Text.Json.Serialization;

namespace GerenciadorFinanceiro.Application.DTOs
{
    /// <summary>
    /// Resultado da validação de um gasto contra a meta definida.
    /// </summary>
    public class ResultadoValidacaoMetaDto
    {
        [JsonPropertyName("excedeu")]
        public bool Excedeu { get; set; }

        [JsonPropertyName("valorLimite")]
        public decimal ValorLimite { get; set; }

        [JsonPropertyName("totalGasto")]
        public decimal TotalGasto { get; set; }

        [JsonPropertyName("percentualUso")]
        public decimal PercentualUso { get; set; }

        public ResultadoValidacaoMetaDto() { }

        public ResultadoValidacaoMetaDto(bool excedeu, decimal valorLimite, decimal totalGasto, decimal percentualUso)
        {
            Excedeu = excedeu;
            ValorLimite = valorLimite;
            TotalGasto = totalGasto;
            PercentualUso = percentualUso;
        }
    }
}
