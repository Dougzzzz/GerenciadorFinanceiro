using GerenciadorFinanceiro.Domain.Entidades;

namespace GerenciadorFinanceiro.Tests.Domain
{
    public class MetaGastoTests
    {
        [Fact]
        public void CriarMetaGasto_Recorrente_DeveInstanciarSemMesEAno()
        {
            // Arrange
            var categoriaId = Guid.NewGuid();
            var valorLimite = 1000.00m;

            // Act
            var meta = new MetaGasto(categoriaId, valorLimite);

            // Assert
            Assert.True(meta.EhRecorrente);
            Assert.Null(meta.Mes);
            Assert.Null(meta.Ano);
            Assert.Equal(valorLimite, meta.ValorLimite);
        }

        [Fact]
        public void CriarMetaGasto_Especifica_DeveInstanciarComMesEAno()
        {
            // Arrange
            var categoriaId = Guid.NewGuid();
            var valorLimite = 500.00m;
            var mes = 12;
            var ano = 2026;

            // Act
            var meta = new MetaGasto(categoriaId, valorLimite, mes, ano);

            // Assert
            Assert.False(meta.EhRecorrente);
            Assert.Equal(mes, meta.Mes);
            Assert.Equal(ano, meta.Ano);
        }

        [Fact]
        public void CriarMetaGasto_ComValorNegativo_DeveLancarExcecao()
        {
            // Arrange
            var categoriaId = Guid.NewGuid();
            var valorLimite = -50.00m;

            // Act & Assert
            var ex = Assert.Throws<ArgumentException>(() => new MetaGasto(categoriaId, valorLimite));
            Assert.Contains("O valor limite não pode ser negativo", ex.Message);
        }
    }
}
