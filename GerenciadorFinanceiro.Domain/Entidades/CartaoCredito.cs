namespace GerenciadorFinanceiro.Domain.Entidades
{
    public class CartaoCredito
    {
        public Guid Id { get; private set; }
        public string Nome { get; private set; } = null!;
        public decimal Limite { get; private set; }
        public int DiaFechamento { get; private set; }
        public int DiaVencimento { get; private set; }
        public ProvedorExtrato Provedor { get; private set; } = ProvedorExtrato.Generico;

        public CartaoCredito(string nome, decimal limite, int diaFechamento, int diaVencimento, ProvedorExtrato provedor = ProvedorExtrato.Generico)
        {
            Id = Guid.NewGuid();
            Nome = nome;
            Limite = limite;
            DiaFechamento = diaFechamento;
            DiaVencimento = diaVencimento;
            Provedor = provedor;
        }

        public void Atualizar(string nome, decimal limite, int diaFechamento, int diaVencimento, ProvedorExtrato provedor)
        {
            Nome = nome;
            Limite = limite;
            DiaFechamento = diaFechamento;
            DiaVencimento = diaVencimento;
            Provedor = provedor;
        }

        protected CartaoCredito() { }
    }
}
