namespace GerenciadorFinanceiro.Api.Models
{
    /// <summary>
    /// Representa um erro padronizado retornado pela API para facilitar a exibição no frontend.
    /// </summary>
    public class ApiErrorResponse
    {
        /// <summary>
        /// Gets mensagem amigável exibida para o usuário.
        /// </summary>
        public string Message { get; init; } = string.Empty;

        /// <summary>
        /// Gets código HTTP associado ao erro.
        /// </summary>
        public int StatusCode { get; init; }

        /// <summary>
        /// Gets identificador da requisição para rastreamento.
        /// </summary>
        public string TraceId { get; init; } = string.Empty;

        /// <summary>
        /// Gets detalhe técnico opcional. Só deve ser exposto quando fizer sentido.
        /// </summary>
        public string? Details { get; init; }
    }
}
