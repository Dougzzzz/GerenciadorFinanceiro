using GerenciadorFinanceiro.Domain.Entidades;

namespace GerenciadorFinanceiro.Domain.Filtros
{
    /// <summary>
    /// Classe de filtros específica para a entidade Transacao.
    /// </summary>
    public class FiltroTransacao : FiltroBase
    {
        public DateTime? DataInicial { get; set; }
        public DateTime? DataFinal { get; set; }
        public TipoTransacao? Tipo { get; set; }
        public Guid? CategoriaId { get; set; }

        /// <summary>
        /// Aplica os filtros e ordenações configurados a uma consulta IQueryable.
        /// </summary>
        /// <param name="query">A consulta original do banco de dados (ou mock).</param>
        /// <returns>A consulta modificada com os filtros aplicados.</returns>
        public IQueryable<Transacao> Aplicar(IQueryable<Transacao> query)
        {
            // 1. Aplicar Filtros (Where)
            if (DataInicial.HasValue)
            {
                query = query.Where(t => t.Data >= DataInicial.Value);
            }

            if (DataFinal.HasValue)
            {
                query = query.Where(t => t.Data <= DataFinal.Value);
            }

            if (Tipo.HasValue)
            {
                query = query.Where(t => t.Tipo == Tipo.Value);
            }

            if (CategoriaId.HasValue)
            {
                query = query.Where(t => t.CategoriaId == CategoriaId.Value);
            }

            // 2. Aplicar Ordenação (OrderBy)
            bool ascendente = Direcao?.Equals("Asc", StringComparison.OrdinalIgnoreCase) ?? true;

            if (string.IsNullOrWhiteSpace(OrdenarPor))
            {
                // Ordenação padrão: Data mais recente primeiro
                query = query.OrderByDescending(t => t.Data);
            }
            else
            {
                // Ordenação dinâmica baseada na propriedade informada
                query = OrdenarPor.ToLower() switch
                {
                    "valor" => ascendente ? query.OrderBy(t => t.Valor) : query.OrderByDescending(t => t.Valor),
                    "descricao" => ascendente ? query.OrderBy(t => t.Descricao) : query.OrderByDescending(t => t.Descricao),
                    "data" => ascendente ? query.OrderBy(t => t.Data) : query.OrderByDescending(t => t.Data),
                    _ => query.OrderByDescending(t => t.Data) // Fallback para data
                };
            }

            return query;
        }
    }
}
