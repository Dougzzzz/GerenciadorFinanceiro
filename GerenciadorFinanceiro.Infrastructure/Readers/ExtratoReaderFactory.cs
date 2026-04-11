using GerenciadorFinanceiro.Application.Interfaces;
using GerenciadorFinanceiro.Domain.Entidades;

namespace GerenciadorFinanceiro.Infrastructure.Readers
{
    public class ExtratoReaderFactory : IExtratoReaderFactory
    {
        public IExtratoReader ObterReader(ProvedorExtrato provedor) => provedor switch
        {
            ProvedorExtrato.C6Bank => new C6CsvExtratoReader(),
            ProvedorExtrato.Generico => throw new NotImplementedException(),
            ProvedorExtrato.Nubank => throw new NotImplementedException(),
            ProvedorExtrato.Inter => throw new NotImplementedException(),
            _ => new CsvExtratoReader(),
        };
    }
}
