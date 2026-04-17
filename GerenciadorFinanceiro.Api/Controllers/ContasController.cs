using GerenciadorFinanceiro.Domain.Entidades;
using GerenciadorFinanceiro.Domain.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace GerenciadorFinanceiro.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ContasController : ControllerBase
    {
        private readonly IContaBancariaRepository _repository;

        public ContasController(IContaBancariaRepository repository)
        {
            _repository = repository;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ContaBancaria>>> Get()
        {
            var contas = await _repository.ObterTodasAsync();
            return Ok(contas);
        }

        [HttpPost]
        public async Task<ActionResult<ContaBancaria>> Post(string nomeBanco, decimal saldoInicial = 0, ProvedorExtrato provedor = ProvedorExtrato.Generico)
        {
            var conta = new ContaBancaria(nomeBanco, saldoInicial, provedor);
            await _repository.AdicionarAsync(conta);
            return Ok(conta);
        }

        [HttpPut("{id}")]
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

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            await _repository.ExcluirAsync(id);
            return NoContent();
        }
    }
}
