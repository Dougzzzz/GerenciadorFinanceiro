using GerenciadorFinanceiro.Domain.Entidades;

namespace GerenciadorFinanceiro.Tests
{
    public class ContaBancariaTests
    {
        [Fact]
        public void ContaBancaria_DeveSerCriada_ComValoresCorretos()
        {
            string nomeEsperado = "Banco Nubank";
            decimal saldoInicialEsperado = 1500.50m;

            var conta = new ContaBancaria(nomeEsperado, saldoInicialEsperado);

            Assert.Equal(nomeEsperado, conta.NomeBanco);
            Assert.Equal(saldoInicialEsperado, conta.SaldoAtual);
            Assert.NotEqual(Guid.Empty, conta.Id);
        }

        [Fact]
        public void AtualizarSaldo_DeveSomarValor_AoSaldoAtual()
        {
            var conta = new ContaBancaria("Banco Itaú", 100m);
            decimal valorDeposito = 50m;

            conta.AtualizarSaldo(valorDeposito);

            Assert.Equal(150m, conta.SaldoAtual);
        }

        [Fact]
        public void AtualizarSaldo_DeveSubtrairValor_QuandoForNegativo()
        {
            var conta = new ContaBancaria("Banco Itaú", 100m);
            decimal valorSaque = -30m;

            conta.AtualizarSaldo(valorSaque);

            Assert.Equal(70m, conta.SaldoAtual);
        }
        [Fact]
        public void Deve_Permitir_Saldo_Negativo_Ao_Atualizar()
        {
            // Arrange
            var conta = new ContaBancaria("Inter", 100);

            // Act
            conta.AtualizarSaldo(-150);

            // Assert
            Assert.Equal(-50, conta.SaldoAtual);
        }
    }
}
