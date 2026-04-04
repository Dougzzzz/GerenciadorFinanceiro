namespace GerenciadorFinanceiro.Application.DTOs
{
    // Mantemos os três primeiros parâmetros (Data, Descricao, Valor) para compatibilidade
    // e adicionamos campos opcionais para as demais colunas do extrato.
    public record TransacaoDto(
        DateTime Data,
        string Descricao,
        decimal Valor,
        string Categoria = "",
        string NomeCartao = "",
        string FinalCartao = "",
        string Parcela = "",
        decimal Cotacao = 0m
    );
}
