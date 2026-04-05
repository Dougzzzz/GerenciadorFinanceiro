using GerenciadorFinanceiro.Application.DTOs;
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
        public async Task<ActionResult<Transacao>> Post([FromBody] SaveTransacaoDto dto)
        {
            if (dto == null)
            {
                return BadRequest();
            }

            var contaId = ParseGuid(dto.contaBancariaId);
            var cartaoId = ParseGuid(dto.cartaoCreditoId);

            var transacao = new Transacao(
                dto.data,
                dto.descricao,
                dto.valor,
                dto.categoriaId,
                contaId,
                cartaoId,
                dto.categoria,
                dto.nomeCartao,
                dto.finalCartao,
                dto.parcela,
                dto.cotacao);

            await _repository.AdicionarAsync(transacao);
            return CreatedAtAction(nameof(Get), new { id = transacao.Id }, transacao);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Put(Guid id, [FromBody] SaveTransacaoDto dto)
        {
            if (id != dto.id)
            {
                return BadRequest("ID da URL não coincide com ID do corpo.");
            }

            var existente = await _repository.ObterPorIdAsync(id);
            if (existente == null)
            {
                return NotFound();
            }

            var contaId = ParseGuid(dto.contaBancariaId);
            var cartaoId = ParseGuid(dto.cartaoCreditoId);

            existente.Atualizar(
                dto.data,
                dto.descricao,
                dto.valor,
                dto.categoriaId,
                contaId,
                cartaoId,
                dto.categoria,
                dto.nomeCartao,
                dto.finalCartao,
                dto.parcela,
                dto.cotacao);

            await _repository.AtualizarAsync(existente);
            return NoContent();
        }

        [HttpPost("importar")]
        public async Task<IActionResult> Importar(IFormFile arquivo, [FromQuery] Guid? categoriaPadraoId, [FromQuery] Guid? contaId, [FromQuery] Guid? cartaoId)
        {
            if (arquivo == null || arquivo.Length == 0)
            {
                return BadRequest("Arquivo inválido.");
            }

            try
            {
                using var stream = arquivo.OpenReadStream();
                await _importarExtratoUseCase.ExecutarAsync(stream, categoriaPadraoId, contaId, cartaoId);
                return Ok("Arquivo importado com sucesso.");
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("excluir-muitas")]
        public async Task<IActionResult> ExcluirMuitas([FromBody] IEnumerable<Guid> ids)
        {
            if (ids == null || !ids.Any())
            {
                return BadRequest("Nenhum ID fornecido.");
            }

            await _repository.ExcluirMuitasAsync(ids);
            return NoContent();
        }

        private static Guid? ParseGuid(object? value)
        {
            if (value == null)
            {
                return null;
            }

            var str = value.ToString();
            if (string.IsNullOrWhiteSpace(str) || str.Equals("undefined", StringComparison.OrdinalIgnoreCase))
            {
                return null;
            }

            if (Guid.TryParse(str, out var result))
            {
                return result;
            }

            return null;
        }
    }
}
