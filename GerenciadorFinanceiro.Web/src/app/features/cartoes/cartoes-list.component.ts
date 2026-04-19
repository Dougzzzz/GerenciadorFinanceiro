import { Component, Input, Output, EventEmitter } from '@angular/core';
import { CommonModule } from '@angular/common';
import { CartaoCredito } from '../../core/models/financeiro.model';
import { CardComponent } from '../../shared/components/card/card.component';

@Component({
  selector: 'app-cartoes-list',
  standalone: true,
  imports: [CommonModule, CardComponent],
  template: `
    <app-card title="Cartões Cadastrados">
      <ul class="list">
        <li *ngFor="let c of cartoes" class="item">
          <div class="info">
            <strong>{{ c.nome }}</strong>
            <div class="meta">
              <small>Vencimento: dia {{ c.diaVencimento }}</small>
              <small class="badge" *ngIf="c.provedor > 0">
                {{ c.provedor === 1 ? 'C6 Bank' : c.provedor === 2 ? 'Nubank' : 'Inter' }}
              </small>
            </div>
          </div>
          <div class="right">
            <span class="value">{{ c.limite | currency:'BRL' }}</span>
            <div class="actions">
              <button class="btn-icon" (click)="edit.emit(c)" title="Editar">✏️</button>
              <button class="btn-icon delete" (click)="delete.emit(c.id)" title="Excluir">🗑️</button>
            </div>
          </div>
        </li>
      </ul>
      <p *ngIf="cartoes.length === 0" class="empty">Nenhum cartão cadastrado.</p>
    </app-card>
  `,
  styles: [`
    .list { display: flex; flex-direction: column; }
    .item { display: flex; justify-content: space-between; align-items: center; padding: 12px; border-bottom: 1px solid #f1f5f9; }
    .info { display: flex; flex-direction: column; }
    .meta { display: flex; align-items: center; gap: 8px; }
    .badge { font-size: 10px; color: #64748b; background: #f1f5f9; padding: 2px 6px; border-radius: 4px; }
    .info small { color: var(--color-text-secondary); }
    .right { display: flex; align-items: center; gap: var(--spacing-md); }
    .value { font-weight: 600; }
    .actions { display: flex; gap: 4px; }
    .btn-icon { background: none; border: none; cursor: pointer; font-size: 1.1rem; padding: 4px; border-radius: 4px; transition: background 0.2s; }
    .btn-icon:hover { background: #f1f5f9; }
    .btn-icon.delete:hover { background: #fee2e2; }
    .empty { padding: var(--spacing-md); text-align: center; color: var(--color-text-secondary); }
  `]
})
export class CartoesListComponent {
  @Input() cartoes: CartaoCredito[] = [];
  @Output() edit = new EventEmitter<CartaoCredito>();
  @Output() delete = new EventEmitter<string>();
}
