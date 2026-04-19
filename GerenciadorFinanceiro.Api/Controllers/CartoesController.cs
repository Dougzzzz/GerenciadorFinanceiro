using GerenciadorFinanceiro.Application.DTOs;
using GerenciadorFinanceiro.Domain.Entidades;
using GerenciadorFinanceiro.Domain.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace GerenciadorFinanceiro.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CartoesController : ControllerBase
    {
        private readonly ICartaoCreditoRepository _repository;

        public CartoesController(ICartaoCreditoRepository repository)
        {
            _repository = repository;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<CartaoCredito>>> Get()
        {
            var cartoes = await _repository.ObterTodosAsync();
            return Ok(cartoes);
        }

        [HttpGet("{id:guid}")]
        public async Task<ActionResult<CartaoCredito>> GetById(Guid id)
        {
            var cartao = await _repository.ObterPorIdAsync(id);
            if (cartao == null)
            {
                return NotFound();
            }

            return Ok(cartao);
        }

        [HttpPost]
        public async Task<ActionResult<CartaoCredito>> Post([FromBody] SaveCartaoDto dto)
        {
            var cartao = new CartaoCredito(dto.nome, dto.limite, dto.diaFechamento, dto.diaVencimento, dto.provedor);
            await _repository.AdicionarAsync(cartao);
            return CreatedAtAction(nameof(GetById), new { id = cartao.Id }, cartao);
        }

        [HttpPut("{id:guid}")]
        public async Task<IActionResult> Put(Guid id, [FromBody] SaveCartaoDto dto)
        {
            var cartao = await _repository.ObterPorIdAsync(id);
            if (cartao == null)
            {
                return NotFound();
            }

            cartao.Atualizar(dto.nome, dto.limite, dto.diaFechamento, dto.diaVencimento, dto.provedor);
            await _repository.AtualizarAsync(cartao);

            return NoContent();
        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var cartao = await _repository.ObterPorIdAsync(id);
            if (cartao == null)
            {
                return NotFound();
            }

            await _repository.RemoverAsync(id);
            return NoContent();
        }
    }
}
