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
        private readonly ICartaoCreditoRepository _repository;
        private readonly CartoesController _controller;

        public CartoesControllerTests()
        {
            _repository = Substitute.For<ICartaoCreditoRepository>();
            _controller = new CartoesController(_repository);
        }

        [Fact]
        public async Task Get_Deve_Retornar_Ok_Com_Lista_De_Cartoes()
        {
            // Arrange
            var cartoes = new List<CartaoCredito> { new("Nubank", 1000, 1, 10) };
            _repository.ObterTodosAsync().Returns(cartoes);

            // Act
            var result = await _controller.Get();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var model = Assert.IsAssignableFrom<IEnumerable<CartaoCredito>>(okResult.Value);
            Assert.Single(model);
        }

        [Fact]
        public async Task GetById_Deve_Retornar_Ok_Se_Existir()
        {
            // Arrange
            var id = Guid.NewGuid();
            var cartao = new CartaoCredito("Nubank", 1000, 1, 10);
            _repository.ObterPorIdAsync(id).Returns(cartao);

            // Act
            var result = await _controller.GetById(id);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            Assert.Equal(cartao, okResult.Value);
        }

        [Fact]
        public async Task GetById_Deve_Retornar_NotFound_Se_Nao_Existir()
        {
            // Arrange
            var id = Guid.NewGuid();
            _repository.ObterPorIdAsync(id).Returns((CartaoCredito?)null);

            // Act & Assert
            await Assert.ThrowsAsync<KeyNotFoundException>(() => _controller.GetById(id));
        }

        [Fact]
        public async Task Post_Deve_Retornar_CreatedAtAction()
        {
            // Arrange
            var dto = new SaveCartaoDto { Nome = "Nubank", Limite = 1000, DiaFechamento = 1, DiaVencimento = 10, Provedor = 1 };

            // Act
            var result = await _controller.Post(dto);

            // Assert
            var createdResult = Assert.IsType<CreatedAtActionResult>(result.Result);
            Assert.Equal(nameof(CartoesController.GetById), createdResult.ActionName);
        }

        [Fact]
        public async Task Put_Deve_Retornar_NoContent_Se_Sucesso()
        {
            // Arrange
            var id = Guid.NewGuid();
            var dto = new SaveCartaoDto { Nome = "Nubank Alt", Limite = 2000, DiaFechamento = 1, DiaVencimento = 10, Provedor = 1 };
            var cartao = new CartaoCredito("Nubank", 1000, 1, 10);
            _repository.ObterPorIdAsync(id).Returns(cartao);

            // Act
            var result = await _controller.Put(id, dto);

            // Assert
            Assert.IsType<NoContentResult>(result);
            await _repository.Received(1).AtualizarAsync(Arg.Is<CartaoCredito>(c => c.Nome == "Nubank Alt"));
        }

        [Fact]
        public async Task Delete_Deve_Retornar_NoContent_Se_Sucesso()
        {
            // Arrange
            var id = Guid.NewGuid();
            var cartao = new CartaoCredito("Nubank", 1000, 1, 10);
            _repository.ObterPorIdAsync(id).Returns(cartao);

            // Act
            var result = await _controller.Delete(id);

            // Assert
            Assert.IsType<NoContentResult>(result);
            await _repository.Received(1).RemoverAsync(id);
        }
    }
}
