namespace GerenciadorFinanceiro.Application.DTOs
{
    // Mantemos os três primeiros parâmetros (Data, Descricao, Valor) para compatibilidade
    // e adicionamos campos opcionais para as demais colunas do extrato.
    public record TransacaoDto(
        DateTime data,
        string descricao,
        decimal valor,
        string categoria = "",
        string nomeCartao = "",
        string finalCartao = "",
        string parcela = "",
        decimal cotacao = 0m);
}
