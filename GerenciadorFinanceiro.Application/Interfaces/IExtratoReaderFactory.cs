using GerenciadorFinanceiro.Domain.Entidades;

namespace GerenciadorFinanceiro.Application.Interfaces
{
    public interface IExtratoReaderFactory
    {
        IExtratoReader ObterReader(ProvedorExtrato provedor);
    }
}
