namespace GerenciadorFinanceiro.Domain.Entidades
{
    public class ContaBancaria
    {
        public Guid Id { get; private set; }
        public string NomeBanco { get; private set; } = null!;
        public decimal SaldoAtual { get; private set; }
        public ProvedorExtrato Provedor { get; private set; } = ProvedorExtrato.Generico;

        public ContaBancaria(string nomeBanco, decimal saldoInicial = 0, ProvedorExtrato provedor = ProvedorExtrato.Generico)
        {
            Id = Guid.NewGuid();
            NomeBanco = nomeBanco;
            SaldoAtual = saldoInicial;
            Provedor = provedor;
        }

        protected ContaBancaria() { }

        public void AtualizarSaldo(decimal valor) => SaldoAtual += valor;
    }
}
