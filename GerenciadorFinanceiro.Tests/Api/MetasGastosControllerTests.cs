using GerenciadorFinanceiro.Api.Controllers;
using GerenciadorFinanceiro.Application.DTOs;
using GerenciadorFinanceiro.Application.UseCases;
using GerenciadorFinanceiro.Domain.Entidades;
using GerenciadorFinanceiro.Domain.Interfaces;
using Microsoft.AspNetCore.Mvc;
using NSubstitute;

namespace GerenciadorFinanceiro.Tests.Api
{
    public class MetasGastosControllerTests
    {
        private readonly IMetaGastoRepository _repoMock;
        private readonly IValidarMetaGastoUseCase _useCaseMock;
        private readonly MetasGastosController _controller;

        public MetasGastosControllerTests()
        {
            _repoMock = Substitute.For<IMetaGastoRepository>();
            _useCaseMock = Substitute.For<IValidarMetaGastoUseCase>();
            _controller = new MetasGastosController(_repoMock, _useCaseMock);
        }

        [Fact]
        public async Task Get_Deve_Retornar_Ok_Com_Lista()
        {
            _repoMock.ObterTodasAsync().Returns([]);
            var result = await _controller.Get();
            Assert.IsType<OkObjectResult>(result.Result);
        }

        [Fact]
        public async Task Post_Deve_Retornar_CreatedAtAction_Quando_Valido()
        {
            var dto = new SaveMetaGastoDto { CategoriaId = Guid.NewGuid(), ValorLimite = 100, Mes = null, Ano = null };
            var result = await _controller.Post(dto);
            Assert.IsType<CreatedAtActionResult>(result.Result);
        }

        [Fact]
        public async Task Put_Deve_Retornar_NotFound_Quando_Inexistente()
        {
            _repoMock.ObterPorIdAsync(Arg.Any<Guid>()).Returns((MetaGasto?)null);

            // Act & Assert
            await Assert.ThrowsAsync<KeyNotFoundException>(() => _controller.Put(Guid.NewGuid(), 100));
        }

        [Fact]
        public async Task GetResumo_Deve_Retornar_Ok()
        {
            _useCaseMock.ExecutarResumoMensalAsync(Arg.Any<int>(), Arg.Any<int>())
                .Returns([]);
            var result = await _controller.GetResumo(4, 2026);
            Assert.IsType<OkObjectResult>(result.Result);
        }
    }
}
