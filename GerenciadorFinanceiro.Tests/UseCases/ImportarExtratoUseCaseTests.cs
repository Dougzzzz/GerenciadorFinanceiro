using System.Text;
using GerenciadorFinanceiro.Application.DTOs;
using GerenciadorFinanceiro.Application.Interfaces;
using GerenciadorFinanceiro.Application.UseCases;
using GerenciadorFinanceiro.Domain.Entidades;
using GerenciadorFinanceiro.Domain.Interfaces;
using NSubstitute;

namespace GerenciadorFinanceiro.Tests.UseCases
{
    public class ImportarExtratoUseCaseTests
    {
        private readonly ITransacaoRepository _repository;
        private readonly ICategoriaRepository _categoriaRepository;
        private readonly ICartaoCreditoRepository _cartaoRepository;
        private readonly IExtratoReader _reader;
        private readonly ImportarExtratoUseCase _useCase;

        public ImportarExtratoUseCaseTests()
        {
            _repository = Substitute.For<ITransacaoRepository>();
            _categoriaRepository = Substitute.For<ICategoriaRepository>();
            _cartaoRepository = Substitute.For<ICartaoCreditoRepository>();
            _reader = Substitute.For<IExtratoReader>();
            _useCase = new ImportarExtratoUseCase(_repository, _categoriaRepository, _cartaoRepository, _reader);
        }

        [Fact]
        public async Task ExecutarAsync_SemContaOuCartao_DeveLancarArgumentException()
        {
            // Arrange
            var stream = new MemoryStream();

            // Act & Assert
            var ex = await Assert.ThrowsAsync<ArgumentException>(() =>
                _useCase.ExecutarAsync(stream, null, null, null));

            Assert.Equal("É necessário informar uma Conta Bancária ou um Cartão de Crédito para a importação.", ex.Message);
        }

        [Fact]
        public async Task ExecutarAsync_ComDadosValidos_DeveUsarCategoriaOutrosSeNenhumaInformada()
        {
            // Arrange
            var stream = new MemoryStream(Encoding.UTF8.GetBytes("data,descricao,valor\n2024-01-01,Teste,100"));
            var contaId = Guid.NewGuid();
            var dtos = new List<TransacaoDto> { new TransacaoDto(DateTime.Now, "Teste", 100) };

            _reader.LerArquivoAsync(Arg.Any<Stream>()).Returns(dtos);

            // Mock da categoria "Outros"
            var categoriaOutros = new Categoria("Outros", TipoTransacao.Despesa);
            _categoriaRepository.ObterPorNomeAsync("Outros", TipoTransacao.Despesa).Returns(categoriaOutros);

            // Act
            await _useCase.ExecutarAsync(stream, null, contaId, null);

            // Assert
            await _repository.Received(1).AdicionarAsync(Arg.Is<Transacao>(t => t.CategoriaId == categoriaOutros.Id));
        }

        [Fact]
        public async Task ExecutarAsync_DeveCriarCategoriaOutrosSeNaoExistir()
        {
            // Arrange
            var stream = new MemoryStream();
            var contaId = Guid.NewGuid();
            var dtos = new List<TransacaoDto> { new TransacaoDto(DateTime.Now, "Teste", 100) };

            _reader.LerArquivoAsync(Arg.Any<Stream>()).Returns(dtos);

            // Mock da categoria "Outros" como não existente inicialmente
            _categoriaRepository.ObterPorNomeAsync("Outros", TipoTransacao.Despesa).Returns((Categoria?)null);

            // Act
            await _useCase.ExecutarAsync(stream, null, contaId, null);

            // Assert
            await _categoriaRepository.Received(1).AdicionarAsync(Arg.Is<Categoria>(c => c.Nome == "Outros"));
            await _repository.Received(1).AdicionarAsync(Arg.Any<Transacao>());
        }
    }
}
