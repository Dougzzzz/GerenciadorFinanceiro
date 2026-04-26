import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Transacao, Categoria, ContaBancaria, CartaoCredito, TipoTransacao, MetaResumo, ResumoMensal, MetaGasto, ResultadoValidacaoMeta } from '../models/financeiro.model';
import { FiltroTransacao } from '../models/filtros.model';
import { ImportacaoPreviewResultado, ResultadoImportacao, TransacaoPreview } from '../models/importacao.model';

@Injectable({
  providedIn: 'root'
})
export class FinanceiroService {
  private apiUrl = '/api';

  constructor(private http: HttpClient) { }

  // Dashboard & Resumos
  getResumoMensal(mes: number, ano: number): Observable<ResumoMensal> {
    return this.http.get<ResumoMensal>(`${this.apiUrl}/dashboard/resumo?mes=${mes}&ano=${ano}`);
  }

  getResumoMetas(mes: number, ano: number): Observable<MetaResumo[]> {
    return this.http.get<MetaResumo[]>(`${this.apiUrl}/MetasGastos/resumo?mes=${mes}&ano=${ano}`);
  }

  // Metas de Gastos
  getMetas(): Observable<MetaGasto[]> {
    return this.http.get<MetaGasto[]>(`${this.apiUrl}/MetasGastos`);
  }

  criarMeta(meta: Partial<MetaGasto>): Observable<MetaGasto> {
    return this.http.post<MetaGasto>(`${this.apiUrl}/MetasGastos`, meta);
  }

  atualizarMeta(id: string, novoValor: number): Observable<unknown> {
    return this.http.put(`${this.apiUrl}/MetasGastos/${id}`, novoValor);
  }

  excluirMeta(id: string): Observable<unknown> {
    return this.http.delete(`${this.apiUrl}/MetasGastos/${id}`);
  }

  validarMeta(categoriaId: string, mes: number, ano: number): Observable<ResultadoValidacaoMeta> {
    return this.http.get<ResultadoValidacaoMeta>(`${this.apiUrl}/MetasGastos/validar/${categoriaId}?mes=${mes}&ano=${ano}`);
  }

  // Transações

  /**
   * Obtém as transações aplicando filtros e ordenação opcionais.
   * @param filtro Objeto contendo os critérios de filtro (data, tipo, categoria, etc.)
   */
  getTransacoes(filtro?: FiltroTransacao): Observable<Transacao[]> {
    let params = new HttpParams();

    if (filtro) {
      const filtroObj = filtro as unknown as Record<string, string | number | Date | null | undefined>;
      Object.keys(filtroObj).forEach(key => {
        const value = filtroObj[key];
        
        if (value !== null && value !== undefined && value !== '') {
          if (value instanceof Date) {
            params = params.set(key, value.toISOString().split('T')[0]);
          } else if (typeof value === 'string' && /^\d{4}-\d{2}-\d{2}$/.test(value)) {
            params = params.set(key, value);
          } else {
            params = params.set(key, value.toString());
          }
        }
      });
    }

    return this.http.get<Transacao[]>(`${this.apiUrl}/transacoes`, { params });
  }

  criarTransacao(transacao: Partial<Transacao>): Observable<Transacao> {
    if (transacao.data) {
      transacao.data = new Date(transacao.data).toISOString();
    }
    return this.http.post<Transacao>(`${this.apiUrl}/transacoes`, transacao);
  }

  atualizarTransacao(transacao: Transacao): Observable<unknown> {
    const body = { ...transacao, data: new Date(transacao.data).toISOString() };
    return this.http.put(`${this.apiUrl}/transacoes/${transacao.id}`, body);
  }

  excluirTransacoes(ids: string[]): Observable<unknown> {
    return this.http.delete(`${this.apiUrl}/transacoes/excluir-muitas`, { body: ids });
  }

  getPreviewImportacao(arquivo: File, contaId?: string, cartaoId?: string): Observable<ImportacaoPreviewResultado> {
    const formData = new FormData();
    formData.append('arquivo', arquivo);
    
    let url = `${this.apiUrl}/transacoes/importar/preview?`;
    if (contaId) url += `contaId=${contaId}&`;
    if (cartaoId) url += `cartaoId=${cartaoId}`;

    return this.http.post<ImportacaoPreviewResultado>(url, formData);
  }

  confirmarImportacao(transacoes: TransacaoPreview[], contaId?: string, cartaoId?: string): Observable<ResultadoImportacao> {
    let url = `${this.apiUrl}/transacoes/importar/confirmar?`;
    if (contaId) url += `contaId=${contaId}&`;
    if (cartaoId) url += `cartaoId=${cartaoId}`;

    return this.http.post<ResultadoImportacao>(url, transacoes);
  }

  importarExtrato(arquivo: File, categoriaId?: string, contaId?: string, cartaoId?: string): Observable<string> {
    const formData = new FormData();
    formData.append('arquivo', arquivo);
    
    let url = `${this.apiUrl}/transacoes/importar?`;
    if (categoriaId) url += `categoriaPadraoId=${categoriaId}&`;
    if (contaId) url += `contaId=${contaId}&`;
    if (cartaoId) url += `&cartaoId=${cartaoId}`;

    return this.http.post(url, formData, { responseType: 'text' });
  }

  // Categorias
  getCategorias(): Observable<Categoria[]> {
    return this.http.get<Categoria[]>(`${this.apiUrl}/categorias`);
  }

  criarCategoria(nome: string, tipo: TipoTransacao): Observable<Categoria> {
    return this.http.post<Categoria>(`${this.apiUrl}/categorias`, { nome, tipo });
  }

  atualizarCategoria(categoria: Categoria): Observable<unknown> {
    return this.http.put(`${this.apiUrl}/categorias/${categoria.id}`, categoria);
  }

  excluirCategorias(ids: string[]): Observable<unknown> {
    return this.http.delete(`${this.apiUrl}/categorias/excluir-muitas`, { body: ids });
  }

  // Contas
  getContas(): Observable<ContaBancaria[]> {
    return this.http.get<ContaBancaria[]>(`${this.apiUrl}/contas`);
  }

  adicionarConta(nomeBanco: string, saldoInicial: number, provedor: number): Observable<ContaBancaria> {
    return this.http.post<ContaBancaria>(`${this.apiUrl}/contas`, { nomeBanco, saldo: saldoInicial, provedor });
  }

  atualizarConta(id: string, nomeBanco: string, saldoAtual: number, provedor: number): Observable<unknown> {
    return this.http.put(`${this.apiUrl}/contas/${id}`, { nomeBanco, saldo: saldoAtual, provedor });
  }

  excluirConta(id: string): Observable<unknown> {
    return this.http.delete(`${this.apiUrl}/contas/${id}`);
  }

  // Cartões
  getCartoes(): Observable<CartaoCredito[]> {
    return this.http.get<CartaoCredito[]>(`${this.apiUrl}/cartoes`);
  }

  getCartaoById(id: string): Observable<CartaoCredito> {
    return this.http.get<CartaoCredito>(`${this.apiUrl}/cartoes/${id}`);
  }

  criarCartao(cartao: Partial<CartaoCredito>): Observable<CartaoCredito> {
    return this.http.post<CartaoCredito>(`${this.apiUrl}/cartoes`, cartao);
  }

  atualizarCartao(id: string, cartao: Partial<CartaoCredito>): Observable<unknown> {
    return this.http.put(`${this.apiUrl}/cartoes/${id}`, cartao);
  }

  excluirCartao(id: string): Observable<unknown> {
    return this.http.delete(`${this.apiUrl}/cartoes/${id}`);
  }
}
