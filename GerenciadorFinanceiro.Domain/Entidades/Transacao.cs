namespace GerenciadorFinanceiro.Domain.Entidades
{
    public class Transacao
    {
        public Guid Id { get; private set; }
        public DateTime Data { get; private set; }
        public string Descricao { get; private set; } = null!;
        public decimal Valor { get; private set; }
        public TipoTransacao Tipo { get; private set; }

        // Relacionamentos (Referências para outras entidades)
        public Guid CategoriaId { get; private set; }
        public Guid? ContaBancariaId { get; private set; } // Nullable (?) pois pode ser do cartão
        public Guid? CartaoCreditoId { get; private set; } // Nullable (?) pois pode ser da conta

        // Novos campos vindos do CSV
        public string Categoria { get; private set; } = null!;
        public string NomeCartao { get; private set; } = null!;
        public string FinalCartao { get; private set; } = null!;
        public string Parcela { get; private set; } = null!;
        public decimal Cotacao { get; private set; }

        // Construtor principal (compatível com implementações existentes)
        public Transacao(DateTime data, string descricao, decimal valor, Guid categoriaId, Guid? contaBancariaId, Guid? cartaoCreditoId)
        {
            Id = Guid.NewGuid();
            Data = data;
            Descricao = descricao;
            Valor = valor;
            Tipo = valor < 0 ? TipoTransacao.Despesa : TipoTransacao.Receita;

            CategoriaId = categoriaId;
            ContaBancariaId = contaBancariaId;
            CartaoCreditoId = cartaoCreditoId;

            // Campos opcionais mantidos vazios por compatibilidade
            Categoria = string.Empty;
            NomeCartao = string.Empty;
            FinalCartao = string.Empty;
            Parcela = string.Empty;
            Cotacao = 0m;
        }

        // Sobrecarga que permite popular os novos campos do DTO
        public Transacao(DateTime data, string descricao, decimal valor, Guid categoriaId, Guid? contaBancariaId, Guid? cartaoCreditoId,
            string categoria, string nomeCartao, string finalCartao, string parcela, decimal cotacao)
            : this(data, descricao, valor, categoriaId, contaBancariaId, cartaoCreditoId)
        {
            Categoria = categoria ?? string.Empty;
            NomeCartao = nomeCartao ?? string.Empty;
            FinalCartao = finalCartao ?? string.Empty;
            Parcela = parcela ?? string.Empty;
            Cotacao = cotacao;
        }

        protected Transacao() { }
    }
}
