using System.Globalization;
using System.Text;
using GerenciadorFinanceiro.Application.UseCases;
using GerenciadorFinanceiro.Infrastructure.Data;
using GerenciadorFinanceiro.Infrastructure.Readers;
using GerenciadorFinanceiro.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

namespace GerenciadorFinanceiro.Tests
{
    public class CsvExtratoReaderTests
    {
        [Fact]
        public async Task LerArquivo_ComColunasValidas_DeveRetornarTransacoes()
        {
            // Arrange
            var csv = new StringBuilder();
            csv.AppendLine("Data de Compra;Nome no Cartão;Final do Cartão;Categoria;Descrição;Parcela;Valor (em US$);Cotação (em R$);Valor (em R$)");
            csv.AppendLine("12/12/2025;ANA PAULA SIQUEIRA;8262;Foto / Fotocópia;JIM.COM*49865135 THA;03/mar;0;0;1142,72");
            csv.AppendLine("14/02/2026;ANA PAULA SIQUEIRA;8262;Vestuário / Roupas;TRG COMERCIO VAREJISTA;;0;0;150,00");

            var bytes = Encoding.Default.GetBytes(csv.ToString());
            using var stream = new MemoryStream(bytes);

            var reader = new CsvExtratoReader();

            // Act
            var result = await reader.LerArquivoAsync(stream);
            var list = result.ToList();

            // Assert
            Assert.Equal(2, list.Count);

            var culture = new CultureInfo("pt-BR");

            var primeira = list[0];
            Assert.Equal(new DateTime(2025, 12, 12), primeira.data.Date);
            Assert.Equal("JIM.COM*49865135 THA", primeira.descricao);
            var expectedPrimeiraValor = decimal.Parse("1142,72", culture);
            Assert.Equal(expectedPrimeiraValor, primeira.valor);
            Assert.Equal("Foto / Fotocópia", primeira.categoria);
            Assert.Equal("ANA PAULA SIQUEIRA", primeira.nomeCartao);
            Assert.Equal("8262", primeira.finalCartao);
            Assert.Equal("03/mar", primeira.parcela);
            Assert.Equal(0m, primeira.cotacao);

            var segunda = list[1];
            Assert.Equal(new DateTime(2026, 2, 14), segunda.data.Date);
            Assert.Equal("TRG COMERCIO VAREJISTA", segunda.descricao);
            var expectedSegundaValor = decimal.Parse("150,00", culture);
            Assert.Equal(expectedSegundaValor, segunda.valor);
            Assert.Equal("Vestuário / Roupas", segunda.categoria);
        }

        [Fact]
        public async Task LerArquivo_DeveGarantirQueDataSejaUtc()
        {
            // Arrange
            var csv = new StringBuilder();
            csv.AppendLine("Data de Compra;Valor (em R$)");
            csv.AppendLine("15/03/2026;100,00");

            var bytes = Encoding.UTF8.GetBytes(csv.ToString());
            using var stream = new MemoryStream(bytes);
            var reader = new CsvExtratoReader();

            // Act
            var result = await reader.LerArquivoAsync(stream);
            var transacao = result.First();

            // Assert
            Assert.Equal(DateTimeKind.Utc, transacao.data.Kind);
            Assert.Equal(15, transacao.data.Day);
            Assert.Equal(3, transacao.data.Month);
            Assert.Equal(2026, transacao.data.Year);
        }

        [Fact]
        public async Task LerArquivo_DeveSuportarDiferentesFormatosDecimais()
        {
            // Arrange
            var csv = new StringBuilder();
            csv.AppendLine("Data de Compra;Valor (em R$)");
            csv.AppendLine("01/01/2026;1142.72");   // Ponto como decimal (da imagem)
            csv.AppendLine("02/01/2026;1142,72");   // Vírgula como decimal
            csv.AppendLine("03/01/2026;1.142,72");  // Padrão BR (ponto milhar, vírgula decimal)
            csv.AppendLine("04/01/2026;1,142.72");  // Padrão US (vírgula milhar, ponto decimal)

            var bytes = Encoding.UTF8.GetBytes(csv.ToString());
            using var stream = new MemoryStream(bytes);
            var reader = new CsvExtratoReader();

            // Act
            var result = (await reader.LerArquivoAsync(stream)).ToList();

            // Assert
            Assert.Equal(4, result.Count);
            foreach (var transacao in result)
            {
                Assert.Equal(1142.72m, transacao.valor);
            }
        }

        [Fact]
        public async Task LerArquivo_ComValorReaisAusente_DeveIgnorarLinhaMesmoComValorDolar()
        {
            // Arrange: linha possui valor em US$ mas coluna de Valor (em R$) vazia
            var csv = new StringBuilder();
            csv.AppendLine("Data de Compra;Nome no Cartão;Final do Cartão;Categoria;Descrição;Parcela;Valor (em US$);Cotação (em R$);Valor (em R$)");
            csv.AppendLine("19/02/2026;ANA PAULA SIQUEIRA;8262;Foto / Fotocópia;JIM.COM*49865135 THA;03/mar;2127.44;0;");

            var bytes = Encoding.Default.GetBytes(csv.ToString());
            using var stream = new MemoryStream(bytes);

            var reader = new CsvExtratoReader();

            // Act
            var result = await reader.LerArquivoAsync(stream);
            var list = result.ToList();

            // Assert: linha deve ser pulada porque Valor (em R$) está ausente
            Assert.Empty(list);
        }

        [Fact]
        public async Task LerArquivo_DeveLerCotacaoEImportarParaTransacao()
        {
            // Arrange
            var csv = new StringBuilder();
            csv.AppendLine("Data de Compra;Nome no Cartão;Final do Cartão;Categoria;Descrição;Parcela;Valor (em US$);Cotação (em R$);Valor (em R$)");
            csv.AppendLine("20/02/2026;ANA;0000;Restaurante;COMIDA;1/1;0;5,25;52,50");

            var bytes = Encoding.Default.GetBytes(csv.ToString());
            using var stream = new MemoryStream(bytes);
            var reader = new CsvExtratoReader();
            var list = (await reader.LerArquivoAsync(stream)).ToList();

            Assert.Single(list);
            var t = list[0];
            Assert.Equal(52.50m, t.valor);
            Assert.Equal(5.25m, t.cotacao);
        }

        [Fact]
        public async Task ImportarExtratoUseCase_DevePersistirTransacoesNoRepositorio()
        {
            // Arrange: cria contexto em memória
            var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: $"DbTeste_{Guid.NewGuid()}")
            .Options;

            using var contexto = new AppDbContext(options);
            var repository = new TransacaoRepository(contexto);
            var categoriaRepository = new CategoriaRepository(contexto);
            var cartaoRepository = new CartaoCreditoRepository(contexto);
            var reader = new CsvExtratoReader();
            var useCase = new ImportarExtratoUseCase(repository, categoriaRepository, cartaoRepository, reader);

            var csv = new StringBuilder();
            csv.AppendLine("Data de Compra;Nome no Cartão;Final do Cartão;Categoria;Descrição;Parcela;Valor (em US$);Cotação (em R$);Valor (em R$)");
            csv.AppendLine("12/12/2025;CartaoInexistente;1111;CategoriaInexistente;PAGAMENTO;1/1;0;0;200,00");

            var bytes = Encoding.Default.GetBytes(csv.ToString());
            using var stream = new MemoryStream(bytes);

            // Act
            await useCase.ExecutarAsync(stream, Guid.NewGuid(), null, null);

            // Assert
            var todas = await repository.ObterTodasAsync();
            Assert.Single(todas);

            var todasCategorias = await categoriaRepository.ObterTodasAsync();
            Assert.Contains(todasCategorias, c => c.Nome == "CategoriaInexistente");

            var todosCartoes = await cartaoRepository.ObterTodosAsync();
            Assert.Contains(todosCartoes, c => c.Nome == "CartaoInexistente");

            var salva = todas.First();
            Assert.Equal(200.00m, salva.Valor);
            Assert.Equal("CategoriaInexistente", salva.Categoria);
            Assert.Equal("CartaoInexistente", salva.NomeCartao);
        }
    }
}
