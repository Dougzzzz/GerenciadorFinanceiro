import { Component, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, ReactiveFormsModule } from '@angular/forms';
import { FinanceiroService } from '../../core/services/financeiro.service';
import { Transacao, Categoria, ContaBancaria, CartaoCredito } from '../../core/models/financeiro.model';
import { FiltroTransacao } from '../../core/models/filtros.model';
import { ButtonComponent } from '../../shared/components/button/button.component';
import { TransacoesFormComponent } from './transacoes-form.component';
import { TransacoesImportComponent } from './transacoes-import.component';
import { TransacoesListComponent } from './transacoes-list.component';

@Component({
  selector: 'app-transacoes',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, ButtonComponent, TransacoesFormComponent, TransacoesImportComponent, TransacoesListComponent],
  template: `
    <div class="transacoes">
      <header class="page-header">
        <h1>Transações</h1>
        <div class="actions">
          <app-button *ngIf="selecionadas().size > 0" variant="ghost" (onClick)="excluirSelecionadas()">
            Excluir ({{ selecionadas().size }})
          </app-button>
          <app-button variant="outline" (onClick)="mostrarNovo.set(!mostrarNovo())">
            {{ mostrarNovo() ? 'Fechar' : 'Nova Transação' }}
          </app-button>
          <app-button (onClick)="mostrarImportacao.set(!mostrarImportacao())">
            {{ mostrarImportacao() ? 'Fechar' : 'Importar Extrato' }}
          </app-button>
        </div>
      </header>

      <!-- Filtros -->
      <section class="filters-section">
        <form [formGroup]="filterForm" (ngSubmit)="aplicarFiltro()" class="filter-form">
          <div class="filter-group">
            <label>Início</label>
            <input type="date" formControlName="dataInicial">
          </div>
          <div class="filter-group">
            <label>Fim</label>
            <input type="date" formControlName="dataFinal">
          </div>
          <div class="filter-group">
            <label>Tipo</label>
            <select formControlName="tipo">
              <option [value]="null">Todos</option>
              <option [value]="0">Receita</option>
              <option [value]="1">Despesa</option>
            </select>
          </div>
          <div class="filter-group">
            <label>Categoria</label>
            <select formControlName="categoriaId">
              <option [value]="null">Todas</option>
              <option *ngFor="let cat of categorias()" [value]="cat.id">{{ cat.nome }}</option>
            </select>
          </div>
          <div class="filter-actions">
            <app-button type="submit" size="sm">Filtrar</app-button>
            <app-button type="button" variant="ghost" size="sm" (onClick)="limparFiltros()">Limpar</app-button>
          </div>
        </form>
      </section>

      <app-transacoes-form *ngIf="mostrarNovo()" 
        [categorias]="categorias()" [contas]="contas()" [cartoes]="cartoes()"
        (onSave)="salvarNovaTransacao($event)">
      </app-transacoes-form>

      <app-transacoes-import *ngIf="mostrarImportacao()"
        [categorias]="categorias()" [contas]="contas()" [cartoes]="cartoes()"
        (onImport)="importar($event)" (onCancel)="mostrarImportacao.set(false)">
      </app-transacoes-import>

      <app-transacoes-list
        [transacoes]="transacoes()" [categorias]="categorias()" [contas]="contas()" [cartoes]="cartoes()"
        [selecionadas]="selecionadas()" [editandoId]="editandoId()" [tempEdit]="tempEdit()"
        [ordenarPor]="filterForm.get('ordenarPor')?.value" [direcao]="filterForm.get('direcao')?.value"
        (onSelect)="toggleSelecionada($event)" (onSelectAll)="toggleTodas($event)"
        (onEdit)="iniciarEdicao($event)" (onSaveEdit)="salvarEdicao()" (onCancelEdit)="cancelarEdicao()"
        (onSort)="aoOrdenar($event)">
      </app-transacoes-list>
    </div>
  `,
  styles: [`
    .page-header { display: flex; justify-content: space-between; align-items: center; margin-bottom: var(--spacing-xl); }
    .actions { display: flex; gap: var(--spacing-md); }
    .filters-section { background: var(--bg-secondary); padding: var(--spacing-md); border-radius: 8px; margin-bottom: var(--spacing-lg); }
    .filter-form { display: flex; flex-wrap: wrap; gap: var(--spacing-md); align-items: flex-end; }
    .filter-group { display: flex; flex-direction: column; gap: 4px; }
    .filter-group label { font-size: 12px; font-weight: bold; color: var(--text-secondary); }
    .filter-group input, .filter-group select { padding: 8px; border: 1px solid var(--border-color); border-radius: 4px; background: var(--bg-primary); color: var(--text-primary); }
    .filter-actions { display: flex; gap: var(--spacing-sm); }
  `]
})
export class TransacoesComponent implements OnInit {
  transacoes = signal<Transacao[]>([]);
  categorias = signal<Categoria[]>([]);
  contas = signal<ContaBancaria[]>([]);
  cartoes = signal<CartaoCredito[]>([]);
  selecionadas = signal<Set<string>>(new Set());
  mostrarImportacao = signal(false);
  mostrarNovo = signal(false);
  editandoId = signal<string | null>(null);
  tempEdit = signal<any>(null);

  filterForm: FormGroup;

  constructor(private financeiroService: FinanceiroService, private fb: FormBuilder) {
    this.filterForm = this.fb.group({
      dataInicial: [null],
      dataFinal: [null],
      tipo: [null],
      categoriaId: [null],
      ordenarPor: ['Data'],
      direcao: ['Desc']
    });
  }

  ngOnInit(): void {
    this.carregarDados();
    this.financeiroService.getCategorias().subscribe(data => this.categorias.set(data));
    this.financeiroService.getContas().subscribe(data => this.contas.set(data));
    this.financeiroService.getCartoes().subscribe(data => this.cartoes.set(data));
  }

  carregarDados(filtro?: FiltroTransacao): void {
    this.financeiroService.getTransacoes(filtro).subscribe(data => this.transacoes.set(data));
  }

  aplicarFiltro(): void {
    const filtro = this.filterForm.value as FiltroTransacao;
    this.carregarDados(filtro);
  }

  aoOrdenar(event: {coluna: string, direcao: 'Asc' | 'Desc'}): void {
    this.filterForm.patchValue({
      ordenarPor: event.coluna,
      direcao: event.direcao
    });
    this.aplicarFiltro();
  }

  limparFiltros(): void {
    this.filterForm.reset({
      dataInicial: null,
      dataFinal: null,
      tipo: null,
      categoriaId: null,
      ordenarPor: 'Data',
      direcao: 'Desc'
    });
    this.carregarDados();
  }

  salvarNovaTransacao(dados: any): void {
    this.financeiroService.criarTransacao(dados).subscribe({
      next: () => { this.mostrarNovo.set(false); this.carregarDados(this.filterForm.value); alert('Transação salva!'); },
      error: (err) => alert('Erro ao salvar: ' + err.message)
    });
  }

  importar(event: {file: File, config: any}): void {
    this.financeiroService.importarExtrato(event.file, event.config.categoriaId, event.config.contaId, event.config.cartaoId).subscribe({
      next: () => { this.mostrarImportacao.set(false); this.carregarDados(this.filterForm.value); alert('Importado!'); },
      error: (err) => alert('Erro: ' + err.message)
    });
  }

  iniciarEdicao(t: Transacao) {
    this.editandoId.set(t.id);
    // Garantir que a data seja formatada como yyyy-MM-dd para o input type="date"
    const data = new Date(t.data);
    const dataFormatada = data.toISOString().split('T')[0];
    this.tempEdit.set({ ...t, data: dataFormatada });
  }

  cancelarEdicao() { this.editandoId.set(null); this.tempEdit.set(null); }

  salvarEdicao() {
    this.financeiroService.atualizarTransacao(this.tempEdit()).subscribe({
      next: () => { this.cancelarEdicao(); this.carregarDados(this.filterForm.value); alert('Atualizada!'); },
      error: (err) => alert('Erro: ' + err.message)
    });
  }

  toggleSelecionada(id: string) {
    const s = new Set(this.selecionadas());
    if (s.has(id)) s.delete(id); else s.add(id);
    this.selecionadas.set(s);
  }

  toggleTodas(event: any) {
    this.selecionadas.set(event.target.checked ? new Set(this.transacoes().map(t => t.id)) : new Set());
  }

  excluirSelecionadas() {
    if (!confirm('Excluir selecionadas?')) return;
    this.financeiroService.excluirTransacoes(Array.from(this.selecionadas())).subscribe({
      next: () => { this.selecionadas.set(new Set()); this.carregarDados(this.filterForm.value); },
      error: (err) => alert('Erro: ' + err.message)
    });
  }
}
