/**
 * Interface base para ordenação (comum a todos os filtros do sistema)
 */
export interface FiltroBase {
  ordenarPor?: string;
  direcao?: 'Asc' | 'Desc';
}

/**
 * Interface específica para filtrar transações, estendendo a base de ordenação
 */
export interface FiltroTransacao extends FiltroBase {
  dataInicial?: Date | string;
  dataFinal?: Date | string;
  tipo?: number;
  categoriaId?: string;
}
