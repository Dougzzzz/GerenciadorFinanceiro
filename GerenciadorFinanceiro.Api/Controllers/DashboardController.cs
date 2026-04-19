using GerenciadorFinanceiro.Application.DTOs;
using GerenciadorFinanceiro.Application.UseCases;
using Microsoft.AspNetCore.Mvc;

namespace GerenciadorFinanceiro.Api.Controllers
{
    /// <summary>
    /// Controller para visualização de métricas e indicadores financeiros.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class DashboardController : ControllerBase
    {
        private readonly ObterResumoMensalUseCase _obterResumoUseCase;

        /// <summary>
        /// Initializes a new instance of the <see cref="DashboardController"/> class.
        /// </summary>
        /// <param name="obterResumoUseCase">Use case para obter resumo mensal.</param>
        public DashboardController(ObterResumoMensalUseCase obterResumoUseCase)
        {
            _obterResumoUseCase = obterResumoUseCase;
        }

        /// <summary>
        /// Obtém o resumo financeiro consolidado (Receitas, Despesas e Saldo) de um determinado mês e ano.
        /// </summary>
        /// <param name="mes">Mês (1 a 12).</param>
        /// <param name="ano">Ano (ex: 2026).</param>
        /// <returns>Objeto com os totais calculados.</returns>
        [HttpGet("resumo")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<ResumoMensalDto>> GetResumo([FromQuery] int mes, [FromQuery] int ano)
        {
            if (mes is < 1 or > 12)
            {
                return BadRequest("Mês inválido. Deve estar entre 1 e 12.");
            }

            var resumo = await _obterResumoUseCase.ExecutarAsync(mes, ano);
            return Ok(resumo);
        }
    }
}
