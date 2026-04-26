using GerenciadorFinanceiro.Application.DTOs;

namespace GerenciadorFinanceiro.Application.UseCases
{
    public interface IValidarMetaGastoUseCase
    {
        Task<ResultadoValidacaoMetaDto> ExecutarAsync(Guid categoriaId, int mes, int ano, decimal valorNovoGasto);
        Task<IEnumerable<MetaResumoDto>> ExecutarResumoMensalAsync(int mes, int ano);
    }
}
