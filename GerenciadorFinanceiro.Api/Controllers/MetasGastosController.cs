using GerenciadorFinanceiro.Application.DTOs;
using GerenciadorFinanceiro.Application.UseCases;
using GerenciadorFinanceiro.Domain.Entidades;
using GerenciadorFinanceiro.Domain.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace GerenciadorFinanceiro.Api.Controllers
{
    /// <summary>
    /// Controller para gestão das Metas de Gastos por categoria.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class MetasGastosController : ControllerBase
    {
        private readonly IMetaGastoRepository _repository;
        private readonly IValidarMetaGastoUseCase _validarUseCase;

        /// <summary>
        /// Initializes a new instance of the <see cref="MetasGastosController"/> class.
        /// </summary>
        /// <param name="repository">O repositório de metas.</param>
        /// <param name="validarUseCase">O caso de uso de validação de metas.</param>
        public MetasGastosController(IMetaGastoRepository repository, IValidarMetaGastoUseCase validarUseCase)
        {
            _repository = repository;
            _validarUseCase = validarUseCase;
        }

        /// <summary>
        /// Lista todas as metas cadastradas.
        /// </summary>
        /// <returns>Uma lista de metas.</returns>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<MetaGasto>>> Get()
        {
            var metas = await _repository.ObterTodasAsync();
            return Ok(metas);
        }

        /// <summary>
        /// Cadastra uma nova meta de gasto.
        /// </summary>
        /// <param name="dados">Dados da meta.</param>
        /// <returns>A meta criada.</returns>
        [HttpPost]
        public async Task<ActionResult<MetaGasto>> Post([FromBody] SaveMetaGastoDto dados)
        {
            try
            {
                var meta = new MetaGasto(dados.CategoriaId, dados.ValorLimite, dados.Mes, dados.Ano);
                await _repository.AdicionarAsync(meta);
                return CreatedAtAction(nameof(Get), new { id = meta.Id }, meta);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Atualiza o valor de uma meta existente.
        /// </summary>
        /// <param name="id">ID da meta.</param>
        /// <param name="novoValor">O novo valor limite.</param>
        /// <returns>Status de sucesso.</returns>
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(Guid id, [FromBody] decimal novoValor)
        {
            var meta = await _repository.ObterPorIdAsync(id) ?? throw new KeyNotFoundException($"Meta com ID {id} não encontrada.");
            try
            {
                meta.AtualizarValor(novoValor);
                await _repository.AtualizarAsync(meta);
                return NoContent();
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Remove uma meta de gasto.
        /// </summary>
        /// <param name="id">ID da meta.</param>
        /// <returns>Status de sucesso.</returns>
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            await _repository.ExcluirAsync(id);
            return NoContent();
        }

        /// <summary>
        /// Obtém o resumo de todas as metas de gastos para um mês e ano específicos.
        /// </summary>
        /// <param name="mes">Mês de referência.</param>
        /// <param name="ano">Ano de referência.</param>
        /// <returns>Uma lista de resumos por categoria.</returns>
        [HttpGet("resumo")]
        public async Task<ActionResult<IEnumerable<MetaResumoDto>>> GetResumo([FromQuery] int mes, [FromQuery] int ano)
        {
            var resumo = await _validarUseCase.ExecutarResumoMensalAsync(mes, ano);
            return Ok(resumo);
        }

        /// <summary>
        /// Verifica o status atual de uma meta (quanto já foi gasto no mês).
        /// </summary>
        /// <param name="categoriaId">ID da categoria.</param>
        /// <param name="mes">Mês de referência.</param>
        /// <param name="ano">Ano de referência.</param>
        /// <returns>Indicadores de consumo da meta.</returns>
        [HttpGet("validar/{categoriaId}")]
        public async Task<ActionResult<ResultadoValidacaoMeta>> Validar(Guid categoriaId, [FromQuery] int mes, [FromQuery] int ano)
        {
            // Valida sem adicionar novo gasto (valor 0)
            var resultado = await _validarUseCase.ExecutarAsync(categoriaId, mes, ano, 0);
            return Ok(resultado);
        }
    }
}
