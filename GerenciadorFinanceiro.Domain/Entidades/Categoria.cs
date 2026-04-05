namespace GerenciadorFinanceiro.Domain.Entidades
{
    public class Categoria
    {
        public Guid Id { get; private set; }
        public string Nome { get; private set; } = null!;
        public TipoTransacao Tipo { get; private set; }

        public Categoria(string nome, TipoTransacao tipo)
        {
            Id = Guid.NewGuid();
            Nome = nome;
            Tipo = tipo;
        }

        protected Categoria() { }

        public void Atualizar(string nome, TipoTransacao tipo)
        {
            Nome = nome;
            Tipo = tipo;
        }
    }

    public enum TipoTransacao
    {
        Receita,
        Despesa,
    }
}
