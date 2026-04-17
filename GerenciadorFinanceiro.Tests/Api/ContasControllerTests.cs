using GerenciadorFinanceiro.Api.Controllers;
using GerenciadorFinanceiro.Domain.Entidades;
using GerenciadorFinanceiro.Domain.Interfaces;
using Microsoft.AspNetCore.Mvc;
using NSubstitute;
using Xunit;

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
            var contas = new List<ContaBancaria> { new ContaBancaria("Itaú") };
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
    }
}
