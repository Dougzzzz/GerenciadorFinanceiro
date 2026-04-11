import { Component, Input, Output, EventEmitter } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { CardComponent } from '../../shared/components/card/card.component';
import { Transacao, Categoria, ContaBancaria, CartaoCredito } from '../../core/models/financeiro.model';

@Component({
  selector: 'app-transacoes-list',
  standalone: true,
  imports: [CommonModule, CardComponent, FormsModule],
  template: `
    <app-card title="Histórico de Lançamentos">
      <table class="table">
        <thead>
          <tr>
            <th width="40"><input type="checkbox" (change)="onSelectAll.emit($event)"></th>
            <th class="sortable" (click)="toggleSort('Data')">
              Data {{ getSortIcon('Data') }}
            </th>
            <th class="sortable" (click)="toggleSort('Descricao')">
              Descrição {{ getSortIcon('Descricao') }}
            </th>
            <th>Categoria</th>
            <th>Cartão/Conta</th>
            <th class="text-right sortable" (click)="toggleSort('Valor')">
              Valor {{ getSortIcon('Valor') }}
            </th>
            <th width="80">Ações</th>
          </tr>
        </thead>
        <tbody>
          <tr *ngFor="let t of transacoes">
            <td><input type="checkbox" [checked]="selecionadas.has(t.id)" (change)="onSelect.emit(t.id)"></td>
            <td>
              <input *ngIf="editandoId === t.id" type="date" [(ngModel)]="tempEdit.data" class="edit-input">
              <span *ngIf="editandoId !== t.id">{{ t.data | date:'dd/MM/yyyy' }}</span>
            </td>
            <td>
              <input *ngIf="editandoId === t.id" type="text" [(ngModel)]="tempEdit.descricao" class="edit-input">
              <div *ngIf="editandoId !== t.id" class="desc-cell">
                <strong>{{ t.descricao }}</strong>
                <small *ngIf="t.parcela">{{ t.parcela }}</small>
              </div>
            </td>
            <td>
              <select *ngIf="editandoId === t.id" [(ngModel)]="tempEdit.categoriaId" class="edit-input">
                <option *ngFor="let c of categorias" [value]="c.id">{{ c.nome }}</option>
              </select>
              <span *ngIf="editandoId !== t.id" class="badge">{{ t.categoriaNavigation?.nome || t.categoria }}</span>
            </td>
            <td>
              <div *ngIf="editandoId === t.id" class="edit-group">
                <select [(ngModel)]="tempEdit.contaBancariaId" class="edit-input">
                  <option [value]="undefined">Nenhuma</option>
                  <option *ngFor="let c of contas" [value]="c.id">{{ c.nomeBanco }}</option>
                </select>
                <select [(ngModel)]="tempEdit.cartaoCreditoId" class="edit-input">
                  <option [value]="undefined">Nenhum</option>
                  <option *ngFor="let c of cartoes" [value]="c.id">{{ c.nome }}</option>
                </select>
              </div>
              <span *ngIf="editandoId !== t.id">{{ t.cartaoCreditoNavigation?.nome || t.contaBancariaNavigation?.nomeBanco || t.nomeCartao || 'Nenhum' }}</span>
            </td>
            <td class="text-right">
              <input *ngIf="editandoId === t.id" type="number" step="0.01" [(ngModel)]="tempEdit.valor" class="edit-input text-right">
              <span *ngIf="editandoId !== t.id" [class.income]="t.valor > 0" [class.expense]="t.valor < 0">{{ t.valor | currency:'BRL' }}</span>
            </td>
            <td>
              <div class="row-actions">
                <button *ngIf="editandoId !== t.id" (click)="onEdit.emit(t)" class="btn-icon">✏️</button>
                <button *ngIf="editandoId === t.id" (click)="onSaveEdit.emit()" class="btn-icon success">✅</button>
                <button *ngIf="editandoId === t.id" (click)="onCancelEdit.emit()" class="btn-icon danger">❌</button>
              </div>
            </td>
          </tr>
        </tbody>
      </table>
    </app-card>
  `,
  styles: [`
    .table { width: 100%; border-collapse: collapse; }
    .table th { text-align: left; padding: var(--spacing-md); border-bottom: 2px solid #f1f5f9; }
    .table td { padding: var(--spacing-md); border-bottom: 1px solid #f1f5f9; vertical-align: middle; }
    .sortable { cursor: pointer; user-select: none; transition: background 0.2s; }
    .sortable:hover { background: #f8fafc; }
    .text-right { text-align: right; }
    .income { color: var(--color-income); font-weight: 600; }
    .expense { color: var(--color-expense); font-weight: 600; }
    .desc-cell { display: flex; flex-direction: column; }
    .desc-cell small { color: var(--color-text-secondary); font-size: 0.75rem; }
    .badge { background: #f1f5f9; padding: 2px 8px; border-radius: 12px; font-size: 0.75rem; }
    .btn-icon { background: none; border: none; cursor: pointer; font-size: 1.1rem; padding: 4px; border-radius: 4px; transition: background 0.2s; }
    .btn-icon:hover { background: #f1f5f9; }
    .btn-icon.success:hover { background: #dcfce7; }
    .btn-icon.danger:hover { background: #fee2e2; }
    .edit-input { width: 100%; padding: 4px; border-radius: 4px; border: 1px solid #e2e8f0; }
    .edit-group { display: flex; flex-direction: column; gap: 4px; }
    .row-actions { display: flex; gap: 8px; }
  `]
})
export class TransacoesListComponent {
  @Input() transacoes: Transacao[] = [];
  @Input() categorias: Categoria[] = [];
  @Input() contas: ContaBancaria[] = [];
  @Input() cartoes: CartaoCredito[] = [];
  @Input() selecionadas = new Set<string>();
  @Input() editandoId: string | null = null;
  @Input() tempEdit: any = null;
  
  // Informação de ordenação atual (recebida do pai)
  @Input() ordenarPor: string = 'Data';
  @Input() direcao: 'Asc' | 'Desc' = 'Desc';

  @Output() onSelect = new EventEmitter<string>();
  @Output() onSelectAll = new EventEmitter<any>();
  @Output() onEdit = new EventEmitter<Transacao>();
  @Output() onSaveEdit = new EventEmitter<void>();
  @Output() onCancelEdit = new EventEmitter<void>();
  
  // Novo evento de ordenação
  @Output() onSort = new EventEmitter<{coluna: string, direcao: 'Asc' | 'Desc'}>();

  toggleSort(coluna: string) {
    let novaDirecao: 'Asc' | 'Desc' = 'Asc';
    
    // Se clicou na mesma coluna que já estava ordenada, inverte a direção
    if (this.ordenarPor === coluna) {
      novaDirecao = this.direcao === 'Asc' ? 'Desc' : 'Asc';
    }
    
    this.onSort.emit({ coluna, direcao: novaDirecao });
  }

  getSortIcon(coluna: string): string {
    if (this.ordenarPor !== coluna) return '↕️';
    return this.direcao === 'Asc' ? '🔼' : '🔽';
  }
}
