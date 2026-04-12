namespace GerenciadorFinanceiro.Application.DTOs
{
    /// <summary>
    /// DTO para criação ou atualização de uma Meta de Gasto.
    /// </summary>
    /// <param name="categoriaId">ID da categoria associada.</param>
    /// <param name="valorLimite">Valor máximo permitido (deve ser positivo).</param>
    /// <param name="mes">Opcional: Mês da meta (1-12).</param>
    /// <param name="ano">Opcional: Ano da meta.</param>
    public record SaveMetaGastoDto(Guid categoriaId, decimal valorLimite, int? mes = null, int? ano = null);
}
