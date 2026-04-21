using GerenciadorFinanceiro.Application.Interfaces;
using GerenciadorFinanceiro.Domain.Entidades;

namespace GerenciadorFinanceiro.Infrastructure.Readers
{
    public class ExtratoReaderFactory : IExtratoReaderFactory
    {
        public IExtratoReader ObterReader(ProvedorExtrato provedor) => provedor switch
        {
            ProvedorExtrato.C6Bank => new C6CsvExtratoReader(),
            ProvedorExtrato.Itau => new ItauXlsExtratoReader(),
            ProvedorExtrato.Nubank => new CsvExtratoReader(), // Implementação futura específica
            ProvedorExtrato.Inter => new CsvExtratoReader(), // Implementação futura específica
            ProvedorExtrato.Generico => new CsvExtratoReader(),
            _ => new CsvExtratoReader(),
        };
    }
}
