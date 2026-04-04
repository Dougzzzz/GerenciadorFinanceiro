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
        public async Task<ActionResult<Categoria>> Post(string nome, TipoTransacao tipo)
        {
            var categoria = new Categoria(nome, tipo);
            await _repository.AdicionarAsync(categoria);
            return Ok(categoria);
        }
    }
}
