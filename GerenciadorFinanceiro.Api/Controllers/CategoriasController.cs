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
        private readonly ITransacaoRepository _transacaoRepository;

        public CategoriasController(ICategoriaRepository repository, ITransacaoRepository transacaoRepository)
        {
            _repository = repository;
            _transacaoRepository = transacaoRepository;
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

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var existente = await _repository.ObterPorIdAsync(id);
            if (existente == null)
            {
                return NotFound();
            }

            if (await _transacaoRepository.PossuiTransacoesPorCategoriaAsync(id))
            {
                return BadRequest($"A categoria '{existente.Nome}' não pode ser excluída porque existem transações vinculadas a ela.");
            }

            await _repository.ExcluirMuitasAsync([id]);
            return NoContent();
        }

        [HttpDelete("excluir-muitas")]
        public async Task<IActionResult> ExcluirMuitas([FromBody] IEnumerable<Guid> ids)
        {
            if (ids == null || !ids.Any())
            {
                return BadRequest("Nenhum ID fornecido.");
            }

            var categoriasComTransacoes = new List<string>();
            foreach (var id in ids)
            {
                var cat = await _repository.ObterPorIdAsync(id);
                if (cat != null && await _transacaoRepository.PossuiTransacoesPorCategoriaAsync(id))
                {
                    categoriasComTransacoes.Add(cat.Nome);
                }
            }

            if (categoriasComTransacoes.Count > 0)
            {
                var nomes = string.Join(", ", categoriasComTransacoes);
                return BadRequest($"As seguintes categorias possuem transações vinculadas e não podem ser excluídas: {nomes}");
            }

            await _repository.ExcluirMuitasAsync(ids);
            return NoContent();
        }
    }
}
