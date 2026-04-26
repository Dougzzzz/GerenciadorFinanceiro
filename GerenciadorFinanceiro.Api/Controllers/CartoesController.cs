using GerenciadorFinanceiro.Application.DTOs;
using GerenciadorFinanceiro.Domain.Entidades;
using GerenciadorFinanceiro.Domain.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace GerenciadorFinanceiro.Api.Controllers
{
    /// <summary>
    /// Controller para gestão de Cartões de Crédito.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class CartoesController : ControllerBase
    {
        private readonly ICartaoCreditoRepository _repository;

        /// <summary>
        /// Initializes a new instance of the <see cref="CartoesController"/> class.
        /// </summary>
        /// <param name="repository">Repositório de cartões de crédito.</param>
        public CartoesController(ICartaoCreditoRepository repository)
        {
            _repository = repository;
        }

        /// <summary>
        /// Obtém a lista de todos os cartões cadastrados.
        /// </summary>
        /// <returns>Uma lista de cartões de crédito.</returns>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CartaoCredito>>> Get()
        {
            var cartoes = await _repository.ObterTodosAsync();
            return Ok(cartoes);
        }

        /// <summary>
        /// Obtém os detalhes de um cartão específico pelo seu ID.
        /// </summary>
        /// <param name="id">Identificador único do cartão (Guid).</param>
        /// <returns>O cartão correspondente ao ID informado.</returns>
        [HttpGet("{id:guid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<CartaoCredito>> GetById(Guid id)
        {
            var cartao = await _repository.ObterPorIdAsync(id) ?? throw new KeyNotFoundException($"Cartão com ID {id} não encontrado.");
            return Ok(cartao);
        }

        /// <summary>
        /// Cadastra um novo cartão de crédito.
        /// </summary>
        /// <param name="dados">Dados para criação do cartão.</param>
        /// <returns>O cartão recém-criado.</returns>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        public async Task<ActionResult<CartaoCredito>> Post([FromBody] SaveCartaoDto dados)
        {
            var cartao = new CartaoCredito(
                dados.Nome,
                dados.Limite,
                dados.DiaFechamento,
                dados.DiaVencimento,
                (ProvedorExtrato)dados.Provedor);

            await _repository.AdicionarAsync(cartao);
            return CreatedAtAction(nameof(GetById), new { id = cartao.Id }, cartao);
        }

        /// <summary>
        /// Atualiza os dados de um cartão existente.
        /// </summary>
        /// <param name="id">ID do cartão a ser editado.</param>
        /// <param name="dados">Novos dados do cartão.</param>
        /// <returns>NoContent em caso de sucesso.</returns>
        [HttpPut("{id:guid}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Put(Guid id, [FromBody] SaveCartaoDto dados)
        {
            var cartao = await _repository.ObterPorIdAsync(id) ?? throw new KeyNotFoundException($"Cartão com ID {id} não encontrado.");
            cartao.Atualizar(
                dados.Nome,
                dados.Limite,
                dados.DiaFechamento,
                dados.DiaVencimento,
                (ProvedorExtrato)dados.Provedor);

            await _repository.AtualizarAsync(cartao);

            return NoContent();
        }

        /// <summary>
        /// Remove um cartão de crédito do sistema.
        /// </summary>
        /// <param name="id">ID do cartão a ser removido.</param>
        /// <returns>NoContent em caso de sucesso.</returns>
        [HttpDelete("{id:guid}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(Guid id)
        {
            _ = await _repository.ObterPorIdAsync(id) ?? throw new KeyNotFoundException($"Cartão com ID {id} não encontrado.");
            await _repository.RemoverAsync(id);
            return NoContent();
        }
    }
}
