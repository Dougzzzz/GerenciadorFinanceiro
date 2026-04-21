using System.Text;
using GerenciadorFinanceiro.Application.DTOs;
using GerenciadorFinanceiro.Infrastructure.Readers;
using Xunit;

namespace GerenciadorFinanceiro.Tests.Infrastructure
{
    public class ItauXlsExtratoReaderTests
    {
        [Fact]
        public async Task LerArquivoAsync_DeveIgnorarCabecalhoEFiltrosDeSaldo()
        {
            // Arrange
            var reader = new ItauXlsExtratoReader();
            
            // Simular um conteúdo que represente as colunas do Itaú
            // O ExcelDataReader consegue ler CSVs simples se auto-detectar
            var sb = new StringBuilder();
            for (int i = 1; i <= 10; i++)
            {
                sb.AppendLine($"Lixo linha {i};;;;"); // 10 linhas de cabeçalho
            }
            
            sb.AppendLine("18/02/2026;SALDO ANTERIOR;;;0,66"); // Deve ser ignorada
            sb.AppendLine("06/03/2026;PAGTO SALARIO;;4501,03;4501,69"); // Entrada
            sb.AppendLine("09/03/2026;PIX TRANSF ANA PAU07/03;;-4400,00;"); // Saída
            sb.AppendLine("09/03/2026;SALDO TOTAL DISPONÍVEL DIA;;;0,01"); // Deve ser ignorada

            byte[] byteArray = Encoding.UTF8.GetBytes(sb.ToString());
            using var stream = new MemoryStream(byteArray);

            // Act
            var resultado = (await reader.LerArquivoAsync(stream)).ToList();

            // Assert
            Assert.Equal(2, resultado.Count);
            
            // Validação da Entrada
            Assert.Equal(new DateTime(2026, 3, 6, 0, 0, 0, DateTimeKind.Utc), resultado[0].data);
            Assert.Equal("PAGTO SALARIO", resultado[0].descricao);
            Assert.Equal(4501.03m, resultado[0].valor);

            // Validação da Saída
            Assert.Equal(new DateTime(2026, 3, 9, 0, 0, 0, DateTimeKind.Utc), resultado[1].data);
            Assert.Equal("PIX TRANSF ANA PAU07/03", resultado[1].descricao);
            Assert.Equal(-4400.00m, resultado[1].valor);
        }
    }
}
