using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GerenciadorFinanceiro.Domain.Entidades
{
    public class ContaBancaria
    {
        public Guid Id { get; private set; }
        public string NomeBanco { get; private set; }
        public decimal SaldoAtual { get; private set; }

        public ContaBancaria(string nomeBanco, decimal saldoInicial = 0)
        {
            Id = Guid.NewGuid();
            NomeBanco = nomeBanco;
            SaldoAtual = saldoInicial;
        }

        protected ContaBancaria() { }

        public void AtualizarSaldo(decimal valor)
        {
            SaldoAtual += valor;
        }
    }
}
