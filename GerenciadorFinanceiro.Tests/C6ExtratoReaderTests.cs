using System.Text;
using GerenciadorFinanceiro.Infrastructure.Readers;

namespace GerenciadorFinanceiro.Tests
{
    public class C6ExtratoReaderTests
    {
        private readonly C6CsvExtratoReader _reader;

        public C6ExtratoReaderTests()
        {
            _reader = new C6CsvExtratoReader();
        }

        [Fact]
        public async Task LerArquivoC6_DeveInverterSinalDeGasto_PositivoParaNegativo()
        {
            // Cenário 1: Compra de 84.90 no CSV deve virar -84.90
            // Arrange
            var csv = new StringBuilder();
            csv.AppendLine("Data;Nome;Cartao;Categoria;Descricao;Parcela;Dolar;Cotacao;Valor (em R$)");
            csv.AppendLine("10/02/2026;DOUGLAS;5474;Alimentação;RESTAURANTE;Única;0;0;84,90");

            using var stream = new MemoryStream(Encoding.UTF8.GetBytes(csv.ToString()));

            // Act
            var result = await _reader.LerArquivoAsync(stream);
            var transacao = result.First();

            // Assert
            Assert.Equal(-84.90m, transacao.Valor);
        }

        [Fact]
        public async Task LerArquivoC6_DeveInverterSinalDePagamento_NegativoParaPositivo()
        {
            // Cenário 2: Pagamento/Estorno de -27.77 no CSV deve virar 27.77
            // Arrange
            var csv = new StringBuilder();
            csv.AppendLine("Data;Nome;Cartao;Categoria;Descricao;Parcela;Dolar;Cotacao;Valor (em R$)");
            csv.AppendLine("10/02/2026;DOUGLAS;5474;Pagamento;PAGAMENTO FATURA;Única;0;0;-27,77");

            using var stream = new MemoryStream(Encoding.UTF8.GetBytes(csv.ToString()));

            // Act
            var result = await _reader.LerArquivoAsync(stream);
            var transacao = result.First();

            // Assert
            Assert.Equal(27.77m, transacao.Valor);
        }

        [Fact]
        public async Task LerArquivoC6_DeveMapearColunasCorretamente()
        {
            // Cenário 3: Validação de indexação e separadores
            // Arrange
            var csv = new StringBuilder();
            csv.AppendLine("Data;Nome;Cartao;Categoria;Descricao;Parcela;Dolar;Cotacao;Valor (em R$)");
            csv.AppendLine("10/02/2026;DOUGLAS BARCELOS;5474;Categoria;MP *ALIEXPRESS;Única;0;0;-27,77");

            using var stream = new MemoryStream(Encoding.UTF8.GetBytes(csv.ToString()));

            // Act
            var result = await _reader.LerArquivoAsync(stream);
            var transacao = result.First();

            // Assert
            Assert.Equal(new DateTime(2026, 02, 10), transacao.Data.Date);
            Assert.Equal("MP *ALIEXPRESS", transacao.Descricao);
            Assert.Equal(27.77m, transacao.Valor);
        }

        [Fact]
        public async Task LerArquivoC6_VazioOuApenasCabecalho_DeveRetornarListaVazia()
        {
            // Cenário 4: Ficheiro sem dados
            // Arrange
            var csv = "Data;Nome;Cartao;Categoria;Descricao;Parcela;Dolar;Cotacao;Valor (em R$)";
            using var stream = new MemoryStream(Encoding.UTF8.GetBytes(csv));

            // Act
            var result = await _reader.LerArquivoAsync(stream);

            // Assert
            Assert.Empty(result);
        }

        [Fact]
        public async Task LerArquivoC6_ValorInvalido_DeveIgnorarLinhaOuLancarExcecao()
        {
            // Cenário 5: Resiliência a dados corrompidos
            // Nota: O CsvExtratoReader atual pula linhas inválidas (continue). 
            // O AC pede uma exceção clara ou tratamento. Vamos validar o comportamento atual primeiro.

            // Arrange
            var csv = new StringBuilder();
            csv.AppendLine("Data;Descricao;Valor (em R$)");
            csv.AppendLine("10/02/2026;ERRO;texto_invalido");

            using var stream = new MemoryStream(Encoding.UTF8.GetBytes(csv.ToString()));

            // Act
            var result = await _reader.LerArquivoAsync(stream);

            // Assert
            Assert.Empty(result); // Atualmente ele ignora, vamos ver se precisamos mudar para Exception
        }
    }
}
