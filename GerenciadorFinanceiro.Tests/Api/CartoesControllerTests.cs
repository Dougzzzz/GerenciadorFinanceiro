using GerenciadorFinanceiro.Api.Controllers;
using GerenciadorFinanceiro.Domain.Entidades;
using GerenciadorFinanceiro.Domain.Interfaces;
using Microsoft.AspNetCore.Mvc;
using NSubstitute;

namespace GerenciadorFinanceiro.Tests.Api
{
    public class CartoesControllerTests
    {
        private readonly ICartaoCreditoRepository _repoMock;
        private readonly CartoesController _controller;

        public CartoesControllerTests()
        {
            _repoMock = Substitute.For<ICartaoCreditoRepository>();
            _controller = new CartoesController(_repoMock);
        }

        [Fact]
        public async Task Get_Deve_Retornar_Ok_Com_Lista()
        {
            // Arrange
            var cartoes = new List<CartaoCredito> { new("Visa", 1000, 1, 10) };
            _repoMock.ObterTodosAsync().Returns(cartoes);

            // Act
            var result = await _controller.Get();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var value = Assert.IsType<List<CartaoCredito>>(okResult.Value);
            Assert.Single(value);
        }

        [Fact]
        public async Task Post_Deve_Retornar_Ok_Com_Cartao_Criado()
        {
            // Act
            var result = await _controller.Post("Master", 2000, 5, 15);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var value = Assert.IsType<CartaoCredito>(okResult.Value);
            Assert.Equal("Master", value.Nome);
            Assert.Equal(2000, value.Limite);
            await _repoMock.Received(1).AdicionarAsync(Arg.Any<CartaoCredito>());
        }
    }
}
