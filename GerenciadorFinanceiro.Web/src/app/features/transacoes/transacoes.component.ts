import { Component, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FinanceiroService } from '../../core/services/financeiro.service';
import { Transacao, Categoria, ContaBancaria, CartaoCredito } from '../../core/models/financeiro.model';
import { ButtonComponent } from '../../shared/components/button/button.component';
import { TransacoesFormComponent } from './transacoes-form.component';
import { TransacoesImportComponent } from './transacoes-import.component';
import { TransacoesListComponent } from './transacoes-list.component';

@Component({
  selector: 'app-transacoes',
  standalone: true,
  imports: [CommonModule, ButtonComponent, TransacoesFormComponent, TransacoesImportComponent, TransacoesListComponent],
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
        (onSelect)="toggleSelecionada($event)" (onSelectAll)="toggleTodas($event)"
        (onEdit)="iniciarEdicao($event)" (onSaveEdit)="salvarEdicao()" (onCancelEdit)="cancelarEdicao()">
      </app-transacoes-list>
    </div>
  `,
  styles: [`
    .page-header { display: flex; justify-content: space-between; align-items: center; margin-bottom: var(--spacing-xl); }
    .actions { display: flex; gap: var(--spacing-md); }
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

  constructor(private financeiroService: FinanceiroService) {}

  ngOnInit(): void { this.carregarDados(); }

  carregarDados(): void {
    this.financeiroService.getTransacoes().subscribe(data => this.transacoes.set(data));
    this.financeiroService.getCategorias().subscribe(data => this.categorias.set(data));
    this.financeiroService.getContas().subscribe(data => this.contas.set(data));
    this.financeiroService.getCartoes().subscribe(data => this.cartoes.set(data));
  }

  salvarNovaTransacao(dados: any): void {
    this.financeiroService.criarTransacao(dados).subscribe({
      next: () => { this.mostrarNovo.set(false); this.carregarDados(); alert('Transação salva!'); },
      error: (err) => alert('Erro ao salvar: ' + err.message)
    });
  }

  importar(event: {file: File, config: any}): void {
    this.financeiroService.importarExtrato(event.file, event.config.categoriaId, event.config.contaId, event.config.cartaoId).subscribe({
      next: () => { this.mostrarImportacao.set(false); this.carregarDados(); alert('Importado!'); },
      error: (err) => alert('Erro: ' + err.message)
    });
  }

  iniciarEdicao(t: Transacao) {
    this.editandoId.set(t.id);
    this.tempEdit.set({ ...t, data: new Date(t.data).toISOString().split('T')[0] });
  }

  cancelarEdicao() { this.editandoId.set(null); this.tempEdit.set(null); }

  salvarEdicao() {
    this.financeiroService.atualizarTransacao(this.tempEdit()).subscribe({
      next: () => { this.cancelarEdicao(); this.carregarDados(); alert('Atualizada!'); },
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
      next: () => { this.selecionadas.set(new Set()); this.carregarDados(); },
      error: (err) => alert('Erro: ' + err.message)
    });
  }
}
