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
        /// Inicializa uma nova instância da classe <see cref="ContasController"/>.
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
        /// <param name="nomeBanco">Nome da instituição financeira.</param>
        /// <param name="saldoInicial">Saldo disponível no momento do cadastro.</param>
        /// <param name="provedor">Tipo de extrato que esta conta costuma importar.</param>
        /// <returns>A conta bancária criada.</returns>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<ContaBancaria>> Post(string nomeBanco, decimal saldoInicial = 0, ProvedorExtrato provedor = ProvedorExtrato.Generico)
        {
            var conta = new ContaBancaria(nomeBanco, saldoInicial, provedor);
            await _repository.AdicionarAsync(conta);
            return Ok(conta);
        }

        /// <summary>
        /// Atualiza os dados de uma conta existente.
        /// </summary>
        /// <param name="id">Identificador único da conta.</param>
        /// <param name="nomeBanco">Novo nome do banco.</param>
        /// <param name="saldoAtual">Saldo atualizado.</param>
        /// <param name="provedor">Novo provedor padrão.</param>
        [HttpPut("{id:guid}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Put(Guid id, string nomeBanco, decimal saldoAtual, ProvedorExtrato provedor)
        {
            var conta = await _repository.ObterPorIdAsync(id);
            if (conta == null)
            {
                return NotFound();
            }

            conta.AtualizarDados(nomeBanco, saldoAtual, provedor);
            await _repository.AtualizarAsync(conta);
            return NoContent();
        }

        /// <summary>
        /// Remove uma conta bancária do sistema.
        /// </summary>
        /// <param name="id">ID da conta a ser excluída.</param>
        [HttpDelete("{id:guid}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> Delete(Guid id)
        {
            await _repository.ExcluirAsync(id);
            return NoContent();
        }
    }
}
