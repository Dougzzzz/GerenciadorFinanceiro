import { Component, EventEmitter, Input, Output } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MetaGasto, Categoria } from '../../core/models/financeiro.model';
import { CardComponent } from '../../shared/components/card/card.component';

@Component({
  selector: 'app-metas-gastos-list',
  standalone: true,
  imports: [CommonModule, CardComponent],
  template: `
    <app-card title="Metas Cadastradas">
      <div class="list-header" *ngIf="metas.length > 0">
        <label class="checkbox-label">
          <input type="checkbox" (change)="allSelectedToggled.emit($event)">
          <small>Selecionar Todas</small>
        </label>
      </div>
      
      <ul class="list">
        <li *ngFor="let meta of metas" class="item">
          <div class="item-info">
            <input type="checkbox" [checked]="selecionadas.has(meta.id)" (change)="selectionToggled.emit(meta.id)">
            <div class="meta-details">
              <strong>{{ getCategoriaNome(meta.categoriaId) }}</strong>
              <div class="meta-meta">
                <span class="badge" [class.badge-recorrente]="meta.ehRecorrente">
                  {{ meta.ehRecorrente ? 'Recorrente' : 'Específica (' + meta.mes + '/' + meta.ano + ')' }}
                </span>
                <span class="limit-value">{{ meta.valorLimite | currency:'BRL' }}</span>
              </div>
            </div>
          </div>
          <div class="item-actions">
            <button (click)="editClicked.emit(meta)" class="btn-icon" title="Editar">✏️</button>
            <button (click)="deleteClicked.emit(meta)" class="btn-icon danger" title="Excluir">🗑️</button>
          </div>
        </li>
      </ul>

      <div *ngIf="metas.length === 0" class="empty-state">
        Nenhuma meta cadastrada.
      </div>
    </app-card>
  `,
  styles: [`
    .list { display: flex; flex-direction: column; gap: 8px; }
    .list-header { display: flex; align-items: center; gap: 8px; margin-bottom: 12px; padding-bottom: 8px; border-bottom: 1px solid #f1f5f9; }
    .checkbox-label { display: flex; align-items: center; gap: 8px; cursor: pointer; }
    .item { display: flex; justify-content: space-between; align-items: center; padding: 12px 8px; border-bottom: 1px solid #f1f5f9; }
    .item-info { display: flex; align-items: center; gap: var(--spacing-md); }
    .meta-details { display: flex; flex-direction: column; gap: 4px; }
    .meta-meta { display: flex; align-items: center; gap: 12px; }
    
    .badge { font-size: 0.7rem; padding: 2px 8px; border-radius: 12px; background: #fee2e2; color: #ef4444; }
    .badge-recorrente { background: #dcfce7; color: #10b981; }
    .limit-value { font-weight: 500; color: var(--color-text-secondary); font-size: 0.9rem; }

    .item-actions { display: flex; gap: 8px; }
    .btn-icon { background: none; border: none; cursor: pointer; font-size: 1.1rem; padding: 4px; border-radius: 4px; transition: background 0.2s; }
    .btn-icon:hover { background: #f1f5f9; }
    .btn-icon.danger:hover { background: #fee2e2; }
    .empty-state { text-align: center; padding: 24px; color: #94a3b8; }
  `]
})
export class MetasGastosListComponent {
  @Input() metas: MetaGasto[] = [];
  @Input() categorias: Categoria[] = [];
  @Input() selecionadas = new Set<string>();
  
  @Output() editClicked = new EventEmitter<MetaGasto>();
  @Output() deleteClicked = new EventEmitter<MetaGasto>();
  @Output() selectionToggled = new EventEmitter<string>();
  @Output() allSelectedToggled = new EventEmitter<Event>();

  getCategoriaNome(id: string): string {
    return this.categorias.find(c => c.id === id)?.nome || 'Desconhecida';
  }
}
