using GerenciadorFinanceiro.Application.DTOs;
using GerenciadorFinanceiro.Domain.Entidades;
using GerenciadorFinanceiro.Domain.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace GerenciadorFinanceiro.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CategoriasController : ControllerBase
    {
        private readonly ICategoriaRepository _repository;

        public CategoriasController(ICategoriaRepository repository)
        {
            _repository = repository;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Categoria>>> Get()
        {
            var categorias = await _repository.ObterTodasAsync();
            return Ok(categorias);
        }

        [HttpPost]
        public async Task<ActionResult<Categoria>> Post([FromBody] SaveCategoriaDto dto)
        {
            if (dto == null)
            {
                return BadRequest();
            }

            var categoria = new Categoria(dto.nome, dto.tipo);
            await _repository.AdicionarAsync(categoria);
            return CreatedAtAction(nameof(Get), new { id = categoria.Id }, categoria);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Put(Guid id, [FromBody] SaveCategoriaDto dto)
        {
            if (dto == null || id != dto.id)
            {
                return BadRequest("ID da URL não coincide com ID do corpo.");
            }

            var existente = await _repository.ObterPorIdAsync(id);
            if (existente == null)
            {
                return NotFound();
            }

            existente.Atualizar(dto.nome, dto.tipo);
            await _repository.AtualizarAsync(existente);
            return NoContent();
        }
    }
}
