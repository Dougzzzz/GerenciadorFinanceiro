export enum TipoTransacao {
  Receita = 0,
  Despesa = 1
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
}

export interface ContaBancaria {
  id: string;
  nomeBanco: string;
  saldoAtual: number;
}

export interface CartaoCredito {
  id: string;
  nome: string;
  limite: number;
  diaFechamento: number;
  diaVencimento: number;
}

export interface Categoria {
  id: string;
  nome: string;
  tipo: TipoTransacao;
}
