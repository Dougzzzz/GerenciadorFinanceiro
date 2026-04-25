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
        private readonly ITransacaoRepository _transacaoRepository;
        private readonly CategoriasController _controller;

        public CategoriasControllerTests()
        {
            _repository = Substitute.For<ICategoriaRepository>();
            _transacaoRepository = Substitute.For<ITransacaoRepository>();
            _controller = new CategoriasController(_repository, _transacaoRepository);
        }

        [Fact]
        public async Task ExcluirMuitas_ComCategoriasSemTransacoes_DeveRetornarNoContent()
        {
            // Arrange
            List<Guid> ids = [Guid.NewGuid()];
            _transacaoRepository.PossuiTransacoesPorCategoriaAsync(ids[0]).Returns(false);

            // Act
            var result = await _controller.ExcluirMuitas(ids);

            // Assert
            Assert.IsType<NoContentResult>(result);
            await _repository.Received(1).ExcluirMuitasAsync(ids);
        }

        [Fact]
        public async Task ExcluirMuitas_ComCategoriaVinculada_DeveRetornarBadRequest()
        {
            // Arrange
            var id = Guid.NewGuid();
            var ids = new List<Guid> { id };
            var cat = new Categoria("Teste", TipoTransacao.Despesa);

            _repository.ObterPorIdAsync(id).Returns(cat);
            _transacaoRepository.PossuiTransacoesPorCategoriaAsync(id).Returns(true);

            // Act
            var result = await _controller.ExcluirMuitas(ids);

            // Assert
            var badRequest = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Contains("Teste", badRequest.Value?.ToString() ?? string.Empty);
            await _repository.DidNotReceive().ExcluirMuitasAsync(Arg.Any<IEnumerable<Guid>>());
        }

        [Fact]
        public async Task Put_ComDadosValidos_DeveRetornarNoContent()
        {
            // Arrange
            var id = Guid.NewGuid();
            var existing = new Categoria("Original", TipoTransacao.Despesa);
            typeof(Categoria).GetProperty("Id")?.SetValue(existing, id);

            var dto = new SaveCategoriaDto { Id = id, Nome = "Update", Tipo = (int)TipoTransacao.Receita };

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
            var dto = new SaveCategoriaDto { Id = idCorpo, Nome = "Update", Tipo = (int)TipoTransacao.Despesa };

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
            var dto = new SaveCategoriaDto { Id = id, Nome = "Update", Tipo = (int)TipoTransacao.Despesa };
            _repository.ObterPorIdAsync(id).Returns((Categoria?)null);

            // Act & Assert
            await Assert.ThrowsAsync<KeyNotFoundException>(() => _controller.Put(id, dto));
        }
    }
}
