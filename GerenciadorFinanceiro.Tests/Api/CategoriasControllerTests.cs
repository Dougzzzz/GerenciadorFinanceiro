using GerenciadorFinanceiro.Api.Controllers;
using GerenciadorFinanceiro.Application.DTOs;
using GerenciadorFinanceiro.Domain.Entidades;
using GerenciadorFinanceiro.Domain.Interfaces;
using Microsoft.AspNetCore.Mvc;
using NSubstitute;

namespace GerenciadorFinanceiro.Tests.Api
{
    public class CategoriasControllerTests
    {
        private readonly ICategoriaRepository _repository;
        private readonly CategoriasController _controller;

        public CategoriasControllerTests()
        {
            _repository = Substitute.For<ICategoriaRepository>();
            _controller = new CategoriasController(_repository);
        }

        [Fact]
        public async Task Put_ComDadosValidos_DeveRetornarNoContent()
        {
            // Arrange
            var id = Guid.NewGuid();
            var existing = new Categoria("Original", TipoTransacao.Despesa);
            typeof(Categoria).GetProperty("Id")?.SetValue(existing, id);

            var dto = new SaveCategoriaDto(id, "Update", TipoTransacao.Receita);

            _repository.ObterPorIdAsync(id).Returns(existing);

            // Act
            var result = await _controller.Put(id, dto);

            // Assert
            Assert.IsType<NoContentResult>(result);
            await _repository.Received(1).AtualizarAsync(existing);
            Assert.Equal("Update", existing.Nome);
            Assert.Equal(TipoTransacao.Receita, existing.Tipo);
        }

        [Fact]
        public async Task Put_ComIdInvalido_DeveRetornarBadRequest()
        {
            // Arrange
            var idUrl = Guid.NewGuid();
            var idCorpo = Guid.NewGuid();
            var dto = new SaveCategoriaDto(idCorpo, "Update", TipoTransacao.Despesa);

            // Act
            var result = await _controller.Put(idUrl, dto);

            // Assert
            Assert.IsType<BadRequestObjectResult>(result);
            await _repository.DidNotReceive().AtualizarAsync(Arg.Any<Categoria>());
        }

        [Fact]
        public async Task Put_CategoriaNaoExistente_DeveRetornarNotFound()
        {
            // Arrange
            var id = Guid.NewGuid();
            var dto = new SaveCategoriaDto(id, "Update", TipoTransacao.Despesa);
            _repository.ObterPorIdAsync(id).Returns((Categoria?)null);

            // Act
            var result = await _controller.Put(id, dto);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }
    }
}
