using GerenciadorFinanceiro.Application.DTOs;
using GerenciadorFinanceiro.Domain.Entidades;
using GerenciadorFinanceiro.Domain.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace GerenciadorFinanceiro.Api.Controllers
{
    /// <summary>
    /// Controller para gestão de Contas Bancárias.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class ContasController : ControllerBase
    {
        private readonly IContaBancariaRepository _repository;

        /// <summary>
        /// Initializes a new instance of the <see cref="ContasController"/> class.
        /// </summary>
        /// <param name="repository">Repositório de contas bancárias.</param>
        public ContasController(IContaBancariaRepository repository)
        {
            _repository = repository;
        }

        /// <summary>
        /// Obtém todas as contas bancárias cadastradas.
        /// </summary>
        /// <returns>Lista de contas bancárias.</returns>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ContaBancaria>>> Get()
        {
            var contas = await _repository.ObterTodasAsync();
            return Ok(contas);
        }

        /// <summary>
        /// Cadastra uma nova conta bancária.
        /// </summary>
        /// <param name="dados">Dados da conta bancária.</param>
        /// <returns>A conta bancária criada.</returns>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<ContaBancaria>> Post([FromBody] SaveContaDto dados)
        {
            var conta = new ContaBancaria(dados.NomeBanco, dados.Saldo, (ProvedorExtrato)dados.Provedor);
            await _repository.AdicionarAsync(conta);
            return Ok(conta);
        }

        /// <summary>
        /// Atualiza os dados de uma conta existente.
        /// </summary>
        /// <param name="id">Identificador único da conta.</param>
        /// <param name="dados">Dados atualizados da conta.</param>
        /// <returns>NoContent em caso de sucesso.</returns>
        [HttpPut("{id:guid}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Put(Guid id, [FromBody] SaveContaDto dados)
        {
            var conta = await _repository.ObterPorIdAsync(id);
            if (conta == null)
            {
                return NotFound();
            }

            conta.AtualizarDados(dados.NomeBanco, dados.Saldo, (ProvedorExtrato)dados.Provedor);
            await _repository.AtualizarAsync(conta);
            return NoContent();
        }

        /// <summary>
        /// Remove uma conta bancária do sistema.
        /// </summary>
        /// <param name="id">ID da conta a ser excluída.</param>
        /// <returns>NoContent em caso de sucesso.</returns>
        [HttpDelete("{id:guid}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> Delete(Guid id)
        {
            await _repository.ExcluirAsync(id);
            return NoContent();
        }
    }
}
