namespace GerenciadorFinanceiro.Application.DTOs
{
    /// <summary>
    /// DTO para exibir o resumo de uma meta com o gasto atual.
    /// </summary>
    /// <param name="Categoria">Nome da categoria.</param>
    /// <param name="Meta">Valor limite da meta.</param>
    /// <param name="GastoAtual">Valor total gasto no período (positivo).</param>
    /// <param name="Percentual">Percentagem do limite consumida.</param>
    public record MetaResumoDto(string Categoria, decimal Meta, decimal GastoAtual, decimal Percentual);
}
