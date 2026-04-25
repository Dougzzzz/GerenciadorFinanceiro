using GerenciadorFinanceiro.Api.Controllers;
using GerenciadorFinanceiro.Application.DTOs;
using GerenciadorFinanceiro.Application.Interfaces;
using GerenciadorFinanceiro.Application.UseCases;
using GerenciadorFinanceiro.Application.UseCases.Importacao;
using GerenciadorFinanceiro.Domain.Entidades;
using GerenciadorFinanceiro.Domain.Filtros;
using GerenciadorFinanceiro.Domain.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NSubstitute;

namespace GerenciadorFinanceiro.Tests.Api
{
    public class TransacoesControllerTests
    {
        private readonly ITransacaoRepository _repository;
        private readonly ImportarExtratoUseCase _useCase;
        private readonly GerarPreviewImportacaoUseCase _previewUseCase;
        private readonly ConfirmarImportacaoUseCase _confirmarUseCase;
        private readonly TransacoesController _controller;

        public TransacoesControllerTests()
        {
            _repository = Substitute.For<ITransacaoRepository>();
            var categoriaRepository = Substitute.For<ICategoriaRepository>();
            var cartaoRepository = Substitute.For<ICartaoCreditoRepository>();
            var contaRepository = Substitute.For<IContaBancariaRepository>();
            var readerFactory = Substitute.For<IExtratoReaderFactory>();
            var similaridadeService = Substitute.For<ICategoriaSimilaridadeService>();
            var unitOfWork = Substitute.For<IUnitOfWork>();

            _useCase = Substitute.For<ImportarExtratoUseCase>(_repository, categoriaRepository, cartaoRepository, contaRepository, readerFactory);
            _previewUseCase = Substitute.For<GerarPreviewImportacaoUseCase>(readerFactory, categoriaRepository, cartaoRepository, contaRepository, similaridadeService);
            _confirmarUseCase = Substitute.For<ConfirmarImportacaoUseCase>(categoriaRepository, _repository, unitOfWork);

            _controller = new TransacoesController(_repository, _useCase, _previewUseCase, _confirmarUseCase);
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
        public async Task Post_ComContaBancariaUndefined_DeveSalvarComContaNula()
        {
            // Arrange
            var categoriaId = Guid.NewGuid();
            var dto = new SaveTransacaoDto
            {
                Id = Guid.NewGuid(),
                Data = DateTime.Now,
                Descricao = "Compra",
                Valor = -50,
                CategoriaId = categoriaId,
                ContaBancariaId = null,
                CartaoCreditoId = null,
            };

            // Act
            var result = await _controller.Post(dto);

            // Assert
            Assert.IsType<CreatedAtActionResult>(result.Result);
            await _repository.Received(1).AdicionarAsync(Arg.Is<Transacao>(t =>
                t.CategoriaId == categoriaId &&
                t.ContaBancariaId == null &&
                t.CartaoCreditoId == null));
        }

        [Fact]
        public async Task Put_ComIdDiferenteDoCorpo_DeveRetornarBadRequest()
        {
            // Arrange
            var idUrl = Guid.NewGuid();
            var dto = new SaveTransacaoDto
            {
                Id = Guid.NewGuid(),
                Data = DateTime.Now,
                Descricao = "Compra",
                Valor = -50,
                CategoriaId = Guid.NewGuid(),
            };

            // Act
            var result = await _controller.Put(idUrl, dto);

            // Assert
            var badRequest = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("ID da URL não coincide com ID do corpo.", badRequest.Value);
            await _repository.DidNotReceiveWithAnyArgs().AtualizarAsync(default!);
        }

        [Fact]
        public async Task Importar_ComArquivoVazio_DeveRetornarBadRequest()
        {
            // Arrange
            var arquivo = Substitute.For<IFormFile>();
            arquivo.Length.Returns(0);

            // Act
            var result = await _controller.Importar(arquivo, null, Guid.NewGuid(), null);

            // Assert
            var badRequest = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Arquivo inválido.", badRequest.Value);
        }

        [Fact]
        public async Task Importar_QuandoUseCaseLancarArgumentException_DeveRetornarBadRequestComMensagem()
        {
            // Arrange
            var arquivo = Substitute.For<IFormFile>();
            arquivo.Length.Returns(10);
            arquivo.OpenReadStream().Returns(new MemoryStream([1, 2, 3]));

            // Act
            var result = await _controller.Importar(arquivo, null, null, null);

            // Assert
            var badRequest = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("É necessário informar uma Conta Bancária ou um Cartão de Crédito para a importação.", badRequest.Value);
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

            var dto = new SaveTransacaoDto
            {
                Id = id,
                Data = DateTime.Now,
                Descricao = "Update",
                Valor = 150,
                CategoriaId = Guid.NewGuid(),
            };

            _repository.ObterPorIdAsync(id).Returns(existing);

            // Act
            var result = await _controller.Put(id, dto);

            // Assert
            Assert.IsType<NoContentResult>(result);
            await _repository.Received(1).AtualizarAsync(existing);
            Assert.Equal("Update", existing.Descricao); // Fix property name if needed
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
            var dto = new SaveTransacaoDto
            {
                Id = id,
                Data = DateTime.Now,
                Descricao = "Update",
                Valor = 150,
                CategoriaId = Guid.NewGuid(),
                ContaBancariaId = null,
            };

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
