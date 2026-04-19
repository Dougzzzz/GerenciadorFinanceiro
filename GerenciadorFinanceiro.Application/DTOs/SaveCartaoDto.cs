using GerenciadorFinanceiro.Domain.Entidades;

namespace GerenciadorFinanceiro.Application.DTOs
{
    /// <summary>
    /// DTO para criação ou atualização de um Cartão de Crédito.
    /// </summary>
    /// <param name="Nome">Nome do cartão.</param>
    /// <param name="Limite">Limite de crédito total.</param>
    /// <param name="DiaFechamento">Dia do mês em que a fatura fecha.</param>
    /// <param name="DiaVencimento">Dia do mês em que a fatura vence.</param>
    /// <param name="Provedor">Provedor de extrato associado.</param>
    public record SaveCartaoDto(
        string Nome,
        decimal Limite,
        int DiaFechamento,
        int DiaVencimento,
        ProvedorExtrato Provedor = ProvedorExtrato.Generico);
}
