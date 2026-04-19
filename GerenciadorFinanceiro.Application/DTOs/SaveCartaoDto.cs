using GerenciadorFinanceiro.Domain.Entidades;

namespace GerenciadorFinanceiro.Application.DTOs
{
    /// <summary>
    /// DTO para criação ou atualização de um Cartão de Crédito.
    /// </summary>
    /// <param name="nome">Nome do cartão.</param>
    /// <param name="limite">Limite de crédito total.</param>
    /// <param name="diaFechamento">Dia do mês em que a fatura fecha.</param>
    /// <param name="diaVencimento">Dia do mês em que a fatura vence.</param>
    /// <param name="provedor">Provedor de extrato associado.</param>
    public record SaveCartaoDto(
        string nome,
        decimal limite,
        int diaFechamento,
        int diaVencimento,
        ProvedorExtrato provedor = ProvedorExtrato.Generico);
}
