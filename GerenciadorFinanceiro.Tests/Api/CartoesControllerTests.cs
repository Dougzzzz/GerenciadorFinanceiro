using GerenciadorFinanceiro.Api.Controllers;
using GerenciadorFinanceiro.Application.DTOs;
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
            var value = Assert.IsAssignableFrom<IEnumerable<CartaoCredito>>(okResult.Value);
            Assert.Single(value);
        }

        [Fact]
        public async Task Post_Deve_Retornar_CreatedAtAction_Com_Cartao_Criado()
        {
            // Arrange
            var dto = new SaveCartaoDto("Master", 2000, 5, 15);

            // Act
            var result = await _controller.Post(dto);

            // Assert
            var createdResult = Assert.IsType<CreatedAtActionResult>(result.Result);
            var value = Assert.IsType<CartaoCredito>(createdResult.Value);
            Assert.Equal("Master", value.Nome);
            Assert.Equal(2000, value.Limite);
            await _repoMock.Received(1).AdicionarAsync(Arg.Any<CartaoCredito>());
        }

        [Fact]
        public async Task Put_Deve_Atualizar_E_Retornar_NoContent()
        {
            // Arrange
            var id = Guid.NewGuid();
            var cartao = new CartaoCredito("Visa", 1000, 1, 10);
            var dto = new SaveCartaoDto("Visa Gold", 5000, 2, 12);
            _repoMock.ObterPorIdAsync(id).Returns(cartao);

            // Act
            var result = await _controller.Put(id, dto);

            // Assert
            Assert.IsType<NoContentResult>(result);
            Assert.Equal("Visa Gold", cartao.Nome);
            Assert.Equal(5000, cartao.Limite);
            await _repoMock.Received(1).AtualizarAsync(cartao);
        }

        [Fact]
        public async Task Delete_Deve_Remover_E_Retornar_NoContent()
        {
            // Arrange
            var id = Guid.NewGuid();
            var cartao = new CartaoCredito("Visa", 1000, 1, 10);
            _repoMock.ObterPorIdAsync(id).Returns(cartao);

            // Act
            var result = await _controller.Delete(id);

            // Assert
            Assert.IsType<NoContentResult>(result);
            await _repoMock.Received(1).RemoverAsync(id);
        }

        [Fact]
        public async Task GetById_Deve_Retornar_NotFound_Se_Nao_Existir()
        {
            // Arrange
            _repoMock.ObterPorIdAsync(Arg.Any<Guid>()).Returns((CartaoCredito?)null);

            // Act
            var result = await _controller.GetById(Guid.NewGuid());

            // Assert
            Assert.IsType<NotFoundResult>(result.Result);
        }
    }
}
