export enum TipoTransacao {
  Receita = 0,
  Despesa = 1
}

export enum ProvedorExtrato {
  Generico = 0,
  C6Bank = 1,
  Nubank = 2,
  Inter = 3
}

export interface Transacao {
  id: string;
  data: string;
  descricao: string;
  valor: number;
  tipo: TipoTransacao;
  categoriaId: string;
  contaBancariaId?: string;
  cartaoCreditoId?: string;
  categoria: string;
  nomeCartao: string;
  finalCartao: string;
  parcela: string;
  cotacao: number;
  categoriaNavigation?: Categoria;
  contaBancariaNavigation?: ContaBancaria;
  cartaoCreditoNavigation?: CartaoCredito;
}

export interface ContaBancaria {
  id: string;
  nomeBanco: string;
  saldoAtual: number;
  provedor: ProvedorExtrato;
}

export interface CartaoCredito {
  id: string;
  nome: string;
  limite: number;
  diaFechamento: number;
  diaVencimento: number;
  provedor: ProvedorExtrato;
}

export interface Categoria {
  id: string;
  nome: string;
  tipo: TipoTransacao;
}

export interface MetaGasto {
  id: string;
  categoriaId: string;
  valorLimite: number;
  mes?: number;
  ano?: number;
  ehRecorrente: boolean;
}

export interface ResultadoValidacaoMeta {
  excedeu: boolean;
  valorLimite: number;
  totalGasto: number;
  percentualUso: number;
}

export interface MetaResumo {
  categoria: string;
  meta: number;
  gastoAtual: number;
  percentual: number;
}
