using GerenciadorFinanceiro.Application.Interfaces;
using GerenciadorFinanceiro.Domain.Entidades;

namespace GerenciadorFinanceiro.Infrastructure.Readers
{
    public class ExtratoReaderFactory : IExtratoReaderFactory
    {
        public IExtratoReader ObterReader(ProvedorExtrato provedor) => provedor switch
        {
            ProvedorExtrato.C6Bank => new C6ContaCorrenteCsvExtratoReader(), // Alterado para CC como padrão para contas
            ProvedorExtrato.Itau => new ItauXlsExtratoReader(),
            ProvedorExtrato.Nubank => new CsvExtratoReader(),
            ProvedorExtrato.Inter => new CsvExtratoReader(),
            ProvedorExtrato.Generico => new CsvExtratoReader(),
            _ => new CsvExtratoReader(),
        };
    }
}
