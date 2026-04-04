using GerenciadorFinanceiro.Application.UseCases;
using GerenciadorFinanceiro.Domain.Entidades;
using GerenciadorFinanceiro.Domain.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace GerenciadorFinanceiro.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TransacoesController : ControllerBase
    {
        private readonly ITransacaoRepository _repository;
        private readonly ImportarExtratoUseCase _importarExtratoUseCase;

        public TransacoesController(ITransacaoRepository repository, ImportarExtratoUseCase importarExtratoUseCase)
        {
            _repository = repository;
            _importarExtratoUseCase = importarExtratoUseCase;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Transacao>>> Get()
        {
            var transacoes = await _repository.ObterTodasAsync();
            return Ok(transacoes);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Transacao>> Get(Guid id)
        {
            var transacao = await _repository.ObterPorIdAsync(id);
            if (transacao == null)
            {
                return NotFound();
            }

            return Ok(transacao);
        }

        [HttpPost]
        public async Task<ActionResult<Transacao>> Post([FromBody] Transacao transacao)
        {
            if (transacao == null)
            {
                return BadRequest();
            }

            await _repository.AdicionarAsync(transacao);
            return CreatedAtAction(nameof(Get), new { id = transacao.Id }, transacao);
        }

        [HttpPost("importar")]
        public async Task<IActionResult> Importar(IFormFile arquivo, [FromQuery] Guid categoriaPadraoId, [FromQuery] Guid? contaId, [FromQuery] Guid? cartaoId)
        {
            if (arquivo == null || arquivo.Length == 0)
            {
                return BadRequest("Arquivo inválido.");
            }

            using var stream = arquivo.OpenReadStream();
            await _importarExtratoUseCase.ExecutarAsync(stream, categoriaPadraoId, contaId, cartaoId);

            return Ok("Arquivo importado com sucesso.");
        }
    }
}
