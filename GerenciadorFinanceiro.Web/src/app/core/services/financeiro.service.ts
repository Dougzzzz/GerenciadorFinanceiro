import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Transacao, Categoria, ContaBancaria, CartaoCredito, TipoTransacao } from '../models/financeiro.model';

@Injectable({
  providedIn: 'root'
})
export class FinanceiroService {
  private apiUrl = '/api';

  constructor(private http: HttpClient) {}

  // Transações
  getTransacoes(): Observable<Transacao[]> {
    return this.http.get<Transacao[]>(`${this.apiUrl}/transacoes`);
  }

  criarTransacao(transacao: Partial<Transacao>): Observable<Transacao> {
    // Garante que a data está em UTC
    if (transacao.data) {
      transacao.data = new Date(transacao.data).toISOString();
    }
    return this.http.post<Transacao>(`${this.apiUrl}/transacoes`, transacao);
  }

  importarExtrato(arquivo: File, categoriaId: string, contaId?: string, cartaoId?: string): Observable<any> {
    const formData = new FormData();
    formData.append('arquivo', arquivo);
    
    let url = `${this.apiUrl}/transacoes/importar?categoriaPadraoId=${categoriaId}`;
    if (contaId) url += `&contaId=${contaId}`;
    if (cartaoId) url += `&cartaoId=${cartaoId}`;

    return this.http.post(url, formData, { responseType: 'text' });
  }

  // Categorias
  getCategorias(): Observable<Categoria[]> {
    return this.http.get<Categoria[]>(`${this.apiUrl}/categorias`);
  }

  criarCategoria(nome: string, tipo: TipoTransacao): Observable<Categoria> {
    return this.http.post<Categoria>(`${this.apiUrl}/categorias?nome=${nome}&tipo=${tipo}`, {});
  }

  // Contas
  getContas(): Observable<ContaBancaria[]> {
    return this.http.get<ContaBancaria[]>(`${this.apiUrl}/contas`);
  }

  criarConta(nomeBanco: string, saldoInicial: number): Observable<ContaBancaria> {
    return this.http.post<ContaBancaria>(`${this.apiUrl}/contas?nomeBanco=${nomeBanco}&saldoInicial=${saldoInicial}`, {});
  }

  // Cartões
  getCartoes(): Observable<CartaoCredito[]> {
    return this.http.get<CartaoCredito[]>(`${this.apiUrl}/cartoes`);
  }

  criarCartao(nome: string, limite: number, diaFechamento: number, diaVencimento: number): Observable<CartaoCredito> {
    return this.http.post<CartaoCredito>(`${this.apiUrl}/cartoes?nome=${nome}&limite=${limite}&diaFechamento=${diaFechamento}&diaVencimento=${diaVencimento}`, {});
  }
}
