using GerenciadorFinanceiro.Domain.Entidades;
using GerenciadorFinanceiro.Infrastructure.Readers;

namespace GerenciadorFinanceiro.Tests.Infrastructure
{
    public class ExtratoReaderFactoryTests
    {
        private readonly ExtratoReaderFactory _factory;

        public ExtratoReaderFactoryTests()
        {
            _factory = new ExtratoReaderFactory();
        }

        [Fact]
        public void ObterReader_C6Bank_DeveRetornarC6CsvExtratoReader()
        {
            // Act
            var reader = _factory.ObterReader(ProvedorExtrato.C6Bank);

            // Assert
            Assert.IsType<C6CsvExtratoReader>(reader);
        }

        [Theory]
        [InlineData(ProvedorExtrato.Generico)]
        [InlineData(ProvedorExtrato.Nubank)]
        [InlineData(ProvedorExtrato.Inter)]
        public void ObterReader_OutrosProvedores_DeveRetornarCsvExtratoReader(ProvedorExtrato provedor)
        {
            // Act
            var reader = _factory.ObterReader(provedor);

            // Assert
            Assert.IsType<CsvExtratoReader>(reader);
        }

        [Fact]
        public void ObterReader_ProvedorInvalido_DeveRetornarCsvExtratoReader()
        {
            // Act
            var reader = _factory.ObterReader((ProvedorExtrato)999);

            // Assert
            Assert.IsType<CsvExtratoReader>(reader);
        }
    }
}
