using GerenciadorFinanceiro.Domain.Entidades;

namespace GerenciadorFinanceiro.Application.DTOs
{
    public record SaveCategoriaDto(
        Guid id,
        string nome,
        TipoTransacao tipo);
}
