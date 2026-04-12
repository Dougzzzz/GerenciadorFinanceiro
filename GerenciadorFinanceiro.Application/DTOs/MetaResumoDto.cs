namespace GerenciadorFinanceiro.Application.DTOs
{
    /// <summary>
    /// DTO para exibir o resumo de uma meta com o gasto atual.
    /// </summary>
    /// <param name="categoria">Nome da categoria.</param>
    /// <param name="meta">Valor limite da meta.</param>
    /// <param name="gastoAtual">Valor total gasto no período (positivo).</param>
    /// <param name="percentual">Percentagem do limite consumida.</param>
    public record MetaResumoDto(string categoria, decimal meta, decimal gastoAtual, decimal percentual);
}
