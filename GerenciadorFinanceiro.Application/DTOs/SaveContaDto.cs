using System.Text.Json.Serialization;

namespace GerenciadorFinanceiro.Application.DTOs
{
    public class SaveContaDto
    {
        [JsonPropertyName("nomeBanco")]
        public string NomeBanco { get; set; } = string.Empty;

        [JsonPropertyName("saldo")]
        public decimal Saldo { get; set; }

        [JsonPropertyName("provedor")]
        public int Provedor { get; set; } // Usando int para facilitar a recepção do JSON
    }
}
