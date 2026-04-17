using GerenciadorFinanceiro.Api.Controllers;
using GerenciadorFinanceiro.Domain.Entidades;
using GerenciadorFinanceiro.Domain.Interfaces;
using Microsoft.AspNetCore.Mvc;
using NSubstitute;

namespace GerenciadorFinanceiro.Tests.Api
{
    public class ContasControllerTests
    {
        private readonly IContaBancariaRepository _repoMock;
        private readonly ContasController _controller;

        public ContasControllerTests()
        {
            _repoMock = Substitute.For<IContaBancariaRepository>();
            _controller = new ContasController(_repoMock);
        }

        [Fact]
        public async Task Get_Deve_Retornar_Ok_Com_Lista()
        {
            // Arrange
            var contas = new List<ContaBancaria> { new("Itaú") };
            _repoMock.ObterTodasAsync().Returns(contas);

            // Act
            var result = await _controller.Get();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var value = Assert.IsType<List<ContaBancaria>>(okResult.Value);
            Assert.Single(value);
        }

        [Fact]
        public async Task Post_Deve_Retornar_Ok_Com_Conta_Criada()
        {
            // Act
            var result = await _controller.Post("Nubank", 500);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var value = Assert.IsType<ContaBancaria>(okResult.Value);
            Assert.Equal("Nubank", value.NomeBanco);
            Assert.Equal(500, value.SaldoAtual);
            await _repoMock.Received(1).AdicionarAsync(Arg.Any<ContaBancaria>());
        }

        [Fact]
        public async Task Put_Deve_Retornar_NoContent_Quando_Sucesso()
        {
            // Arrange
            var id = Guid.NewGuid();
            var conta = new ContaBancaria("Teste");
            _repoMock.ObterPorIdAsync(id).Returns(conta);

            // Act
            var result = await _controller.Put(id, "Novo Nome", 1000, ProvedorExtrato.C6Bank);

            // Assert
            Assert.IsType<NoContentResult>(result);
            await _repoMock.Received(1).AtualizarAsync(Arg.Any<ContaBancaria>());
        }

        [Fact]
        public async Task Put_Deve_Retornar_NotFound_Quando_Inexistente()
        {
            // Arrange
            _repoMock.ObterPorIdAsync(Arg.Any<Guid>()).Returns((ContaBancaria?)null);

            // Act
            var result = await _controller.Put(Guid.NewGuid(), "Nome", 0, ProvedorExtrato.Generico);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task Delete_Deve_Retornar_NoContent_Quando_Sucesso()
        {
            // Act
            var result = await _controller.Delete(Guid.NewGuid());

            // Assert
            Assert.IsType<NoContentResult>(result);
            await _repoMock.Received(1).ExcluirAsync(Arg.Any<Guid>());
        }
    }
}
