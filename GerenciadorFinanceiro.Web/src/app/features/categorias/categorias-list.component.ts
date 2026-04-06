import { Component, Input, Output, EventEmitter } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Categoria } from '../../core/models/financeiro.model';
import { CardComponent } from '../../shared/components/card/card.component';

@Component({
  selector: 'app-categorias-list',
  standalone: true,
  imports: [CommonModule, CardComponent],
  template: `
    <app-card title="Categorias Cadastradas">
      <div class="list-header" *ngIf="categorias.length > 0">
        <input type="checkbox" (change)="onToggleTodas.emit($event)">
        <small>Selecionar Todas</small>
      </div>
      <ul class="list">
        <li *ngFor="let c of categorias()" class="item">
          <div class="item-info">
            <input type="checkbox" [checked]="selecionadas.has(c.id)" (change)="onToggleSelecionada.emit(c.id)">
            <span>{{ c.nome }}</span>
            <span class="badge" [class.receita]="c.tipo === 0">
              {{ c.tipo === 0 ? 'Receita' : 'Despesa' }}
            </span>
          </div>
          <div class="item-actions">
            <button (click)="onIniciarEdicao.emit(c)" class="btn-icon">✏️</button>
            <button (click)="onExcluirUma.emit(c)" class="btn-icon danger">🗑️</button>
          </div>
        </li>
      </ul>
    </app-card>
  `,
  styles: [`
    .list { display: flex; flex-direction: column; gap: 8px; }
    .list-header { display: flex; align-items: center; gap: 8px; margin-bottom: 12px; padding-bottom: 8px; border-bottom: 1px solid #f1f5f9; }
    .item { display: flex; justify-content: space-between; align-items: center; padding: 8px; border-bottom: 1px solid #f1f5f9; }
    .item-info { display: flex; align-items: center; gap: var(--spacing-md); }
    .badge { font-size: 0.75rem; padding: 2px 8px; border-radius: 12px; background: #fee2e2; color: #ef4444; }
    .badge.receita { background: #dcfce7; color: #10b981; }
    .item-actions { display: flex; gap: 8px; }
    .btn-icon { background: none; border: none; cursor: pointer; font-size: 1.1rem; padding: 4px; border-radius: 4px; transition: background 0.2s; }
    .btn-icon:hover { background: #f1f5f9; }
    .btn-icon.danger:hover { background: #fee2e2; }
  `]
})
export class CategoriasListComponent {
  @Input() categorias: any = []; // Usando any para facilitar o signal no pai ou Categoria[]
  @Input() selecionadas = new Set<string>();
  
  @Output() onIniciarEdicao = new EventEmitter<Categoria>();
  @Output() onExcluirUma = new EventEmitter<Categoria>();
  @Output() onToggleSelecionada = new EventEmitter<string>();
  @Output() onToggleTodas = new EventEmitter<any>();
}
