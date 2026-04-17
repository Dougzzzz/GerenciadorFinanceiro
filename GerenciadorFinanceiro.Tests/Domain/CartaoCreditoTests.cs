using GerenciadorFinanceiro.Domain.Entidades;
using Xunit;

namespace GerenciadorFinanceiro.Tests.Domain
{
    public class CartaoCreditoTests
    {
        [Fact]
        public void Deve_Criar_Cartao_Com_Dados_Corretos()
        {
            // Arrange
            var nome = "Nubank";
            var limite = 5000m;
            var fechamento = 10;
            var vencimento = 20;

            // Act
            var cartao = new CartaoCredito(nome, limite, fechamento, vencimento);

            // Assert
            Assert.NotEqual(Guid.Empty, cartao.Id);
            Assert.Equal(nome, cartao.Nome);
            Assert.Equal(limite, cartao.Limite);
            Assert.Equal(fechamento, cartao.DiaFechamento);
            Assert.Equal(vencimento, cartao.DiaVencimento);
            Assert.Equal(ProvedorExtrato.Generico, cartao.Provedor);
        }
    }
}
