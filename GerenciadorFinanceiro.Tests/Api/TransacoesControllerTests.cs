using GerenciadorFinanceiro.Api.Controllers;
using GerenciadorFinanceiro.Application.DTOs;
using GerenciadorFinanceiro.Application.Interfaces;
using GerenciadorFinanceiro.Application.UseCases;
using GerenciadorFinanceiro.Domain.Entidades;
using GerenciadorFinanceiro.Domain.Filtros;
using GerenciadorFinanceiro.Domain.Interfaces;
using Microsoft.AspNetCore.Mvc;
using NSubstitute;

namespace GerenciadorFinanceiro.Tests.Api
{
    public class TransacoesControllerTests
    {
        private readonly ITransacaoRepository _repository;
        private readonly ImportarExtratoUseCase _useCase;
        private readonly TransacoesController _controller;

        public TransacoesControllerTests()
        {
            _repository = Substitute.For<ITransacaoRepository>();
            var categoriaRepository = Substitute.For<ICategoriaRepository>();
            var cartaoRepository = Substitute.For<ICartaoCreditoRepository>();
            _useCase = Substitute.For<ImportarExtratoUseCase>(_repository, categoriaRepository, cartaoRepository, Substitute.For<IExtratoReader>());
            _controller = new TransacoesController(_repository, _useCase);
        }

        [Fact]
        public async Task Get_ComFiltros_DeveChamarRepositorioComFiltroERetornarOk()
        {
            // Arrange
            var filtro = new FiltroTransacao
            {
                DataInicial = DateTime.Now.AddDays(-7),
                Tipo = TipoTransacao.Despesa,
                OrdenarPor = "Valor",
                Direcao = "Asc",
            };

            var transacoesMock = new List<Transacao>
            {
                new(DateTime.Now, "Teste", -100, Guid.NewGuid(), null, null),
            };

            _repository.ObterTodasAsync(filtro).Returns(transacoesMock);

            // Act
            var result = await _controller.Get(filtro);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var model = Assert.IsAssignableFrom<IEnumerable<Transacao>>(okResult.Value);
            Assert.Single(model);
            await _repository.Received(1).ObterTodasAsync(filtro);
        }

        [Fact]
        public async Task Get_SemFiltros_DeveChamarRepositorioComFiltroVazioERetornarOk()
        {
            // Arrange
            var filtroVazio = new FiltroTransacao();
            _repository.ObterTodasAsync(Arg.Any<FiltroTransacao>()).Returns([]);

            // Act
            var result = await _controller.Get(filtroVazio);

            // Assert
            Assert.IsType<OkObjectResult>(result.Result);
            await _repository.Received(1).ObterTodasAsync(filtroVazio);
        }

        [Fact]
        public async Task ExcluirMuitas_ComIdsValidos_DeveRetornarNoContent()
        {
            // Arrange
            List<Guid> ids = [Guid.NewGuid(), Guid.NewGuid()];

            // Act
            var result = await _controller.ExcluirMuitas(ids);

            // Assert
            Assert.IsType<NoContentResult>(result);
            await _repository.Received(1).ExcluirMuitasAsync(ids);
        }

        [Fact]
        public async Task ExcluirMuitas_SemIds_DeveRetornarBadRequest()
        {
            // Act
            var result = await _controller.ExcluirMuitas([]);

            // Assert
            Assert.IsType<BadRequestObjectResult>(result);
            await _repository.DidNotReceiveWithAnyArgs().ExcluirMuitasAsync(default!);
        }

        [Fact]
        public async Task Put_ComDadosValidos_DeveRetornarNoContent()
        {
            // Arrange
            var id = Guid.NewGuid();
            var existing = new Transacao(DateTime.Now, "Original", 100, Guid.NewGuid(), null, null);
            typeof(Transacao).GetProperty("Id")?.SetValue(existing, id);

            var dto = new SaveTransacaoDto(id, DateTime.Now, "Update", 150, Guid.NewGuid(), null, null);

            _repository.ObterPorIdAsync(id).Returns(existing);

            // Act
            var result = await _controller.Put(id, dto);

            // Assert
            Assert.IsType<NoContentResult>(result);
            await _repository.Received(1).AtualizarAsync(existing);
            Assert.Equal("Update", existing.Descricao);
            Assert.Equal(150, existing.Valor);
        }

        [Fact]
        public async Task Put_ComIdUndefined_DeveTratarComoNullERetornarNoContent()
        {
            // Arrange
            var id = Guid.NewGuid();
            var existing = new Transacao(DateTime.Now, "Original", 100, Guid.NewGuid(), Guid.NewGuid(), null);
            typeof(Transacao).GetProperty("Id")?.SetValue(existing, id);

            // Simulando o que o frontend envia: "undefined" como string
            var dto = new SaveTransacaoDto(id, DateTime.Now, "Update", 150, Guid.NewGuid(), "undefined", null);

            _repository.ObterPorIdAsync(id).Returns(existing);

            // Act
            var result = await _controller.Put(id, dto);

            // Assert
            Assert.IsType<NoContentResult>(result);
            await _repository.Received(1).AtualizarAsync(existing);
            Assert.Null(existing.ContaBancariaId); // Deve ter sido limpo para null
        }
    }
}
