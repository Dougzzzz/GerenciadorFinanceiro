using GerenciadorFinanceiro.Application.DTOs;

namespace GerenciadorFinanceiro.Application.Interfaces
{
    public interface IExtratoReader
    {
        Task<IEnumerable<TransacaoDto>> LerArquivoAsync(Stream arquivo);
    }
}
