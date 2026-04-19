export interface CategoriaSugestao {
  categoriaId: string;
  nomeCategoria: string;
  similaridade: number;
  similaridadeFormatada: string;
}

export interface TransacaoPreview {
  idTemporario: string;
  descricao: string;
  valor: number;
  data: string;
  categoriaOriginalCsv?: string;
  categoriasSugeridas: CategoriaSugestao[];
  categoriaEscolhidaId?: string;
  novaCategoriaPersonalizada?: string;
}

export interface ImportacaoPreviewResultado {
  transacoes: TransacaoPreview[];
  linhasComErro: number;
  totalComSugestao: number;
  totalSemSugestao: number;
}

export interface ResultadoImportacao {
  sucesso: boolean;
  totalImportado: number;
  totalIgnorado: number;
  mensagemErro?: string;
}
