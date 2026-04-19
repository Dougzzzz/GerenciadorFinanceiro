using GerenciadorFinanceiro.Application.DTOs.Importacao;
using GerenciadorFinanceiro.Application.UseCases.Importacao;
using GerenciadorFinanceiro.Domain.Entidades;
using GerenciadorFinanceiro.Domain.Interfaces;
using NSubstitute;

namespace GerenciadorFinanceiro.Tests.Application
{
    public class ConfirmarImportacaoUseCaseTests
    {
        private readonly ICategoriaRepository _categoriaRepo;
        private readonly ITransacaoRepository _transacaoRepo;
        private readonly IUnitOfWork _uow;
        private readonly ConfirmarImportacaoUseCase _useCase;

        public ConfirmarImportacaoUseCaseTests()
        {
            _categoriaRepo = Substitute.For<ICategoriaRepository>();
            _transacaoRepo = Substitute.For<ITransacaoRepository>();
            _uow = Substitute.For<IUnitOfWork>();
            _useCase = new ConfirmarImportacaoUseCase(_categoriaRepo, _transacaoRepo, _uow);
        }

        [Fact]
        public async Task ExecutarAsync_ComGastoNegativo_DeveCriarCategoriaComoDespesa()
        {
            // Arrange: Simulando uma compra do C6 que já foi invertida para negativa pelo leitor
            var transacoes = new List<TransacaoPreviewDto>
            {
                new()
                {
                    Descricao = "MERCADO EXTRA",
                    Valor = -150.00m,
                    Data = DateTime.Now,
                    CategoriaOriginalCsv = "Mercado",
                    CategoriaEscolhidaId = null // Forçar criação automática
                },
            };

            _categoriaRepo.ObterPorNomeAsync(Arg.Any<string>(), Arg.Any<TipoTransacao>())
                .Returns((Categoria?)null);

            // Act
            var result = await _useCase.ExecutarAsync(transacoes, Guid.NewGuid(), null);

            // Assert: Verificar se a categoria criada foi do tipo DESPESA (1)
            await _categoriaRepo.Received(1).AdicionarAsync(Arg.Is<Categoria>(c =>
                c.Nome == "Mercado" && c.Tipo == TipoTransacao.Despesa));

            Assert.True(result.Sucesso);
        }

        [Fact]
        public async Task ExecutarAsync_ComPagamentoPositivo_DeveCriarCategoriaComoReceita()
        {
            // Arrange: Simulando um pagamento/rendimento que é positivo no sistema
            var transacoes = new List<TransacaoPreviewDto>
            {
                new()
                {
                    Descricao = "PIX RECEBIDO",
                    Valor = 500.00m,
                    Data = DateTime.Now,
                    CategoriaOriginalCsv = "Transferência",
                    CategoriaEscolhidaId = null
                },
            };

            _categoriaRepo.ObterPorNomeAsync(Arg.Any<string>(), Arg.Any<TipoTransacao>())
                .Returns((Categoria?)null);

            // Act
            var result = await _useCase.ExecutarAsync(transacoes, Guid.NewGuid(), null);

            // Assert: Verificar se a categoria criada foi do tipo RECEITA (0)
            await _categoriaRepo.Received(1).AdicionarAsync(Arg.Is<Categoria>(c =>
                c.Nome == "Transferência" && c.Tipo == TipoTransacao.Receita));

            Assert.True(result.Sucesso);
        }

        [Fact]
        public async Task ExecutarAsync_QuandoCategoriaJaExiste_NaoDeveCriarNova()
        {
            // Arrange
            var categoriaExistente = new Categoria("Saúde", TipoTransacao.Despesa);
            var transacoes = new List<TransacaoPreviewDto>
            {
                new() { Descricao = "FARMACIA", Valor = -50, Data = DateTime.Now, CategoriaOriginalCsv = "Saúde" },
            };

            _categoriaRepo.ObterPorNomeAsync("Saúde", TipoTransacao.Despesa).Returns(categoriaExistente);

            // Act
            await _useCase.ExecutarAsync(transacoes, Guid.NewGuid(), null);

            // Assert
            await _categoriaRepo.DidNotReceive().AdicionarAsync(Arg.Any<Categoria>());
            await _transacaoRepo.Received(1).AdicionarAsync(Arg.Is<Transacao>(t => t.CategoriaId == categoriaExistente.Id));
        }
    }
}
