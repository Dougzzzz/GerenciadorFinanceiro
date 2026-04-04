using GerenciadorFinanceiro.Api.Controllers;
using GerenciadorFinanceiro.Application.Interfaces;
using GerenciadorFinanceiro.Application.UseCases;
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
            _useCase = Substitute.For<ImportarExtratoUseCase>(_repository, Substitute.For<IExtratoReader>());
            _controller = new TransacoesController(_repository, _useCase);
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
    }
}
