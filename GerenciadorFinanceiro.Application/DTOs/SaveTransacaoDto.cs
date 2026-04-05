namespace GerenciadorFinanceiro.Application.DTOs
{
    public record SaveTransacaoDto(
        Guid id,
        DateTime data,
        string descricao,
        decimal valor,
        Guid categoriaId,
        object? contaBancariaId = null,
        object? cartaoCreditoId = null,
        string categoria = "",
        string nomeCartao = "",
        string finalCartao = "",
        string parcela = "",
        decimal cotacao = 0m,
        int tipo = 0);
}
