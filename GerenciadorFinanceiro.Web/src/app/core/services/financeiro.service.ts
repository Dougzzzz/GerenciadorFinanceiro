import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Transacao, Categoria, ContaBancaria, CartaoCredito } from '../models/financeiro.model';

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

  // Contas
  getContas(): Observable<ContaBancaria[]> {
    return this.http.get<ContaBancaria[]>(`${this.apiUrl}/contas`);
  }

  // Cartões
  getCartoes(): Observable<CartaoCredito[]> {
    return this.http.get<CartaoCredito[]>(`${this.apiUrl}/cartoes`);
  }
}
