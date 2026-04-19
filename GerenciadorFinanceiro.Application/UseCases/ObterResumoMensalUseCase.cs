using GerenciadorFinanceiro.Application.DTOs;
using GerenciadorFinanceiro.Domain.Filtros;
using GerenciadorFinanceiro.Domain.Interfaces;

namespace GerenciadorFinanceiro.Application.UseCases
{
    /// <summary>
    /// Realiza o cálculo do resumo financeiro para um mês e ano específicos.
    /// </summary>
    public class ObterResumoMensalUseCase
    {
        private readonly ITransacaoRepository _transacaoRepository;

        public ObterResumoMensalUseCase(ITransacaoRepository transacaoRepository)
        {
            _transacaoRepository = transacaoRepository;
        }

        public async Task<ResumoMensalDto> ExecutarAsync(int mes, int ano)
        {
            // 1. Definir o intervalo do mês
            var dataInicial = new DateTime(ano, mes, 1, 0, 0, 0, DateTimeKind.Utc);
            var dataFinal = dataInicial.AddMonths(1).AddDays(-1);

            // 2. Buscar todas as transações do período usando o filtro de domínio
            var filtro = new FiltroTransacao
            {
                DataInicial = dataInicial,
                DataFinal = dataFinal,
            };

            var transacoes = await _transacaoRepository.ObterTodasAsync(filtro);

            // 3. Calcular totais respeitando os sinais
            var totalReceitas = transacoes.Where(t => t.Valor > 0).Sum(t => t.Valor);
            var totalDespesas = transacoes.Where(t => t.Valor < 0).Sum(t => t.Valor);

            // 4. Calcular gastos por categoria (apenas despesas)
            var gastosPorCategoria = transacoes
                .Where(t => t.Valor < 0)
                .GroupBy(t => t.CategoriaNavigation?.Nome ?? "Outros")
                .Select(g => new ResumoCategoriaDto
                {
                    Categoria = g.Key,
                    Valor = Math.Abs(g.Sum(t => t.Valor)),
                })
                .OrderByDescending(x => x.Valor)
                .ToList();

            return new ResumoMensalDto
            {
                TotalReceitas = totalReceitas,
                TotalDespesas = Math.Abs(totalDespesas),
                Saldo = totalReceitas + totalDespesas,
                Mes = mes,
                Ano = ano,
                GastosPorCategoria = gastosPorCategoria,
            };
        }
    }
}
