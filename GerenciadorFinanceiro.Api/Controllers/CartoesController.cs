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

        [HttpPost]
        public async Task<ActionResult<CartaoCredito>> Post(string nome, decimal limite, int diaFechamento, int diaVencimento)
        {
            var cartao = new CartaoCredito(nome, limite, diaFechamento, diaVencimento);
            await _repository.AdicionarAsync(cartao);
            return Ok(cartao);
        }
    }
}
