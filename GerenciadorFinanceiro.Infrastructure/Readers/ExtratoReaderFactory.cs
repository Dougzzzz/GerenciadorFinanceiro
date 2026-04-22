using GerenciadorFinanceiro.Application.Interfaces;
using GerenciadorFinanceiro.Domain.Entidades;

namespace GerenciadorFinanceiro.Infrastructure.Readers
{
    public class ExtratoReaderFactory : IExtratoReaderFactory
    {
        public IExtratoReader ObterReader(ProvedorExtrato provedor, bool ehCartao = false) => provedor switch
        {
            ProvedorExtrato.C6Bank => ehCartao ? new C6CsvExtratoReader() : new C6ContaCorrenteCsvExtratoReader(),
            ProvedorExtrato.Itau => new ItauXlsExtratoReader(),
            ProvedorExtrato.Nubank => new CsvExtratoReader(),
            ProvedorExtrato.Inter => new CsvExtratoReader(),
            ProvedorExtrato.Generico => new CsvExtratoReader(),
            _ => new CsvExtratoReader(),
        };
    }
}
