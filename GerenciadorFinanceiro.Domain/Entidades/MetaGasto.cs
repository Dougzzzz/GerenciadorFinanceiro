namespace GerenciadorFinanceiro.Domain.Entidades
{
    /// <summary>
    /// Representa o limite de gastos de uma categoria. Pode ser uma meta específica para um mês/ano ou uma meta recorrente.
    /// </summary>
    public class MetaGasto
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MetaGasto"/> class.
        /// Construtor para metas recorrentes ou específicas.
        /// </summary>
        /// <param name="categoriaId">ID da categoria associada.</param>
        /// <param name="valorLimite">Valor máximo permitido (deve ser positivo).</param>
        /// <param name="mes">Opcional: Mês da meta (1-12).</param>
        /// <param name="ano">Opcional: Ano da meta.</param>
        public MetaGasto(Guid categoriaId, decimal valorLimite, int? mes = null, int? ano = null)
        {
            if (valorLimite < 0)
            {
                throw new ArgumentException("O valor limite não pode ser negativo.", nameof(valorLimite));
            }

            if (mes is < 1 or > 12)
            {
                throw new ArgumentException("O mês deve estar entre 1 e 12.", nameof(mes));
            }

            // Se o mês for informado, o ano também precisa ser informado para uma meta específica.
            if (mes.HasValue && !ano.HasValue)
            {
                throw new ArgumentException("Para metas específicas, o ano deve ser informado junto com o mês.");
            }

            Id = Guid.NewGuid();
            CategoriaId = categoriaId;
            ValorLimite = valorLimite;
            Mes = mes;
            Ano = ano;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MetaGasto"/> class.
        /// Construtor para o Entity Framework.
        /// </summary>
        protected MetaGasto() { }

        /// <summary>
        /// Gets the unique identifier of the meta.
        /// </summary>
        public Guid Id { get; private set; }

        /// <summary>
        /// Gets the category identifier.
        /// </summary>
        public Guid CategoriaId { get; private set; }

        /// <summary>
        /// Gets the limit value.
        /// </summary>
        public decimal ValorLimite { get; private set; }

        /// <summary>
        /// Gets the month of the meta.
        /// </summary>
        public int? Mes { get; private set; }

        /// <summary>
        /// Gets the year of the meta.
        /// </summary>
        public int? Ano { get; private set; }

        /// <summary>
        /// Gets a value indicating whether a meta é recorrente (não possui mês e ano definidos).
        /// </summary>
        public bool EhRecorrente => !Mes.HasValue && !Ano.HasValue;

        /// <summary>
        /// Permite atualizar o valor da meta de gasto.
        /// </summary>
        /// <param name="novoValor">O novo valor limite (deve ser positivo).</param>
        public void AtualizarValor(decimal novoValor)
        {
            if (novoValor < 0)
            {
                throw new ArgumentException("O novo valor limite não pode ser negativo.", nameof(novoValor));
            }

            ValorLimite = novoValor;
        }
    }
}
