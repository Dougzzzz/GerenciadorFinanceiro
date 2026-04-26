using System.Text.Json.Serialization;

namespace GerenciadorFinanceiro.Application.DTOs
{
    public class SaveCategoriaDto
    {
        [JsonPropertyName("id")]
        public Guid Id { get; set; }

        [JsonPropertyName("nome")]
        public string Nome { get; set; } = string.Empty;

        [JsonPropertyName("tipo")]
        public int Tipo { get; set; }
    }
}
