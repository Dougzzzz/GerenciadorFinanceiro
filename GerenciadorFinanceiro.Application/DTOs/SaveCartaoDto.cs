using GerenciadorFinanceiro.Domain.Entidades;

namespace GerenciadorFinanceiro.Application.DTOs
{
    public record SaveCartaoDto(
        string nome,
        decimal limite,
        int diaFechamento,
        int diaVencimento,
        ProvedorExtrato provedor = ProvedorExtrato.Generico);
}
