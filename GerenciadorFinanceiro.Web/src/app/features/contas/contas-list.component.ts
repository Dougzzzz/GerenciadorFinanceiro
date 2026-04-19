import { Component, Input, Output, EventEmitter } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ContaBancaria } from '../../core/models/financeiro.model';
import { CardComponent } from '../../shared/components/card/card.component';

@Component({
  selector: 'app-contas-list',
  standalone: true,
  imports: [CommonModule, CardComponent],
  template: `
    <app-card title="Contas Cadastradas">
      <ul class="list">
        <li *ngFor="let c of contas" class="item">
          <div class="info">
            <strong>{{ c.nomeBanco }}</strong>
            <small class="badge" *ngIf="c.provedor > 0">
              {{ c.provedor === 1 ? 'C6 Bank' : c.provedor === 2 ? 'Nubank' : 'Inter' }}
            </small>
          </div>
          <div class="actions-wrapper">
              <span class="value" [class.negative]="c.saldoAtual < 0">
                  {{ c.saldoAtual | currency:'BRL' }}
              </span>
              <div class="actions">
                  <button class="btn-icon" (click)="edit.emit(c)" title="Editar">✏️</button>
                  <button class="btn-icon danger" (click)="delete.emit(c.id)" title="Excluir">🗑️</button>
              </div>
          </div>
        </li>
      </ul>
      <p *ngIf="contas.length === 0" class="empty">Nenhuma conta cadastrada.</p>
    </app-card>
  `,
  styles: [`
    .list { display: flex; flex-direction: column; gap: 8px; }
    .item { display: flex; justify-content: space-between; align-items: center; padding: 12px 8px; border-bottom: 1px solid #f1f5f9; }
    .info { display: flex; flex-direction: column; gap: 2px; }
    .badge { font-size: 10px; color: #64748b; background: #f1f5f9; padding: 2px 6px; border-radius: 4px; width: fit-content; }
    .actions-wrapper { display: flex; align-items: center; gap: 16px; }
    .value { font-weight: 600; color: var(--color-income); min-width: 100px; text-align: right; }
    .value.negative { color: var(--color-expense); }
    .actions { display: flex; gap: 4px; }
    .btn-icon { background: none; border: none; cursor: pointer; font-size: 1.1rem; padding: 4px; border-radius: 4px; transition: background 0.2s; }
    .btn-icon:hover { background: #f1f5f9; }
    .btn-icon.danger:hover { background: #fee2e2; }
    .empty { text-align: center; color: var(--color-text-secondary); padding: var(--spacing-md); }
  `]
})
export class ContasListComponent {
  @Input() contas: ContaBancaria[] = [];
  @Output() edit = new EventEmitter<ContaBancaria>();
  @Output() delete = new EventEmitter<string>();
}
