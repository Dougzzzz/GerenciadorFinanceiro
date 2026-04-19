using System.Security.Cryptography;
using System.Text;

namespace GerenciadorFinanceiro.Domain.Entidades
{
    public class Transacao
    {
        public Guid Id { get; private set; }
        public DateTime Data { get; private set; }
        public string Descricao { get; private set; }
        public decimal Valor { get; private set; }
        public TipoTransacao Tipo { get; private set; }

        /// <summary>
        /// Gets hash único gerado para evitar duplicação de importações.
        /// Gerado a partir de: Data + Descricao + Valor + Conta/Cartão.
        /// </summary>
        public string ChaveExclusiva { get; private set; }

        // Relacionamentos
        public Guid CategoriaId { get; private set; }
        public Guid? ContaBancariaId { get; private set; }
        public Guid? CartaoCreditoId { get; private set; }

        // Propriedades de Navegação
        public virtual Categoria? CategoriaNavigation { get; private set; }
        public virtual ContaBancaria? ContaBancariaNavigation { get; private set; }
        public virtual CartaoCredito? CartaoCreditoNavigation { get; private set; }

        public string Categoria { get; private set; } = null!;
        public string NomeCartao { get; private set; } = null!;
        public string FinalCartao { get; private set; } = null!;
        public string Parcela { get; private set; } = null!;
        public decimal Cotacao { get; private set; }

        public Transacao(DateTime data, string descricao, decimal valor, Guid categoriaId, Guid? contaBancariaId, Guid? cartaoCreditoId)
        {
            Id = Guid.NewGuid();

            Data = data.Kind == DateTimeKind.Unspecified
                ? DateTime.SpecifyKind(data, DateTimeKind.Utc)
                : data.ToUniversalTime();

            Descricao = descricao;
            Valor = valor;
            Tipo = valor < 0 ? TipoTransacao.Despesa : TipoTransacao.Receita;

            CategoriaId = categoriaId;
            ContaBancariaId = contaBancariaId;
            CartaoCreditoId = cartaoCreditoId;

            Categoria = string.Empty;
            NomeCartao = string.Empty;
            FinalCartao = string.Empty;
            Parcela = string.Empty;
            Cotacao = 0m;

            ChaveExclusiva = GerarHash();
        }

        public Transacao(
            DateTime data,
            string descricao,
            decimal valor,
            Guid categoriaId,
            Guid? contaBancariaId,
            Guid? cartaoCreditoId,
            string categoria,
            string nomeCartao,
            string finalCartao,
            string parcela,
            decimal cotacao)
            : this(data, descricao, valor, categoriaId, contaBancariaId, cartaoCreditoId)
        {
            Categoria = categoria ?? string.Empty;
            NomeCartao = nomeCartao ?? string.Empty;
            FinalCartao = finalCartao ?? string.Empty;
            Parcela = parcela ?? string.Empty;
            Cotacao = cotacao;

            // Recalcula o hash com os campos de snapshot se disponíveis
            ChaveExclusiva = GerarHash();
        }

        protected Transacao() { }

        public void Atualizar(
            DateTime data,
            string descricao,
            decimal valor,
            Guid categoriaId,
            Guid? contaBancariaId,
            Guid? cartaoCreditoId,
            string categoria,
            string nomeCartao,
            string finalCartao,
            string parcela,
            decimal cotacao)
        {
            Data = data.Kind == DateTimeKind.Unspecified
                ? DateTime.SpecifyKind(data, DateTimeKind.Utc)
                : data.ToUniversalTime();

            Descricao = descricao;
            Valor = valor;
            Tipo = valor < 0 ? TipoTransacao.Despesa : TipoTransacao.Receita;
            CategoriaId = categoriaId;
            ContaBancariaId = contaBancariaId;
            CartaoCreditoId = cartaoCreditoId;
            Categoria = categoria ?? string.Empty;
            NomeCartao = nomeCartao ?? string.Empty;
            FinalCartao = finalCartao ?? string.Empty;
            Parcela = parcela ?? string.Empty;
            Cotacao = cotacao;

            ChaveExclusiva = GerarHash();
        }

        private string GerarHash()
        {
            // Criamos uma string composta pelos dados fundamentais da transação
            // O uso de InvariantCulture e formatos fixos garante que o hash seja o mesmo em qualquer ambiente
            var raw = $"{Data:yyyyMMdd}-{Descricao.Trim().ToUpper()}-{Valor:F2}-{ContaBancariaId}-{CartaoCreditoId}-{Parcela}";
            var inputBytes = Encoding.UTF8.GetBytes(raw);
            var hashBytes = SHA256.HashData(inputBytes);
            return Convert.ToHexString(hashBytes);
        }
    }
}
