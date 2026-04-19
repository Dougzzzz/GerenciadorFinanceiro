import { Component, Input, Output, EventEmitter, OnChanges, SimpleChanges } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Categoria } from '../../core/models/financeiro.model';
import { CardComponent } from '../../shared/components/card/card.component';

@Component({
  selector: 'app-categorias-list',
  standalone: true,
  imports: [CommonModule, CardComponent, FormsModule],
  template: `
    <app-card title="Categorias Cadastradas">
      <div class="list-controls">
        <div class="search-box">
          <input type="text" [(ngModel)]="filtro" (input)="filtrar()" placeholder="Pesquisar categoria...">
        </div>
        
        <div class="selection-info" *ngIf="categorias && categorias.length > 0">
          <label class="checkbox-container">
            <input type="checkbox" 
                   [checked]="isTodasSelecionadas()" 
                   [indeterminate]="isParcialmenteSelecionadas()"
                   (change)="toggleTodas($event)">
            <span class="checkmark"></span>
            <small>Selecionar Todas ({{ categoriasFiltradas.length }})</small>
          </label>
        </div>
      </div>

      <ul class="list" *ngIf="categoriasFiltradas.length > 0; else emptyState">
        <li *ngFor="let c of categoriasFiltradas" class="item" [class.selected]="selecionadas.has(c.id)">
          <div class="item-info">
            <label class="checkbox-container">
              <input type="checkbox" [checked]="selecionadas.has(c.id)" (change)="onToggleSelecionada.emit(c.id)">
              <span class="checkmark"></span>
            </label>
            <span class="nome">{{ c.nome }}</span>
            <span class="badge" [class.receita]="c.tipo === 0">
              {{ c.tipo === 0 ? 'Receita' : 'Despesa' }}
            </span>
          </div>
          <div class="item-actions">
            <button (click)="onIniciarEdicao.emit(c)" class="btn-icon" title="Editar">✏️</button>
            <button (click)="onExcluirUma.emit(c)" class="btn-icon danger" title="Excluir">🗑️</button>
          </div>
        </li>
      </ul>

      <ng-template #emptyState>
        <div class="empty-state">
          <p>{{ filtro ? 'Nenhuma categoria encontrada para "' + filtro + '"' : 'Nenhuma categoria cadastrada.' }}</p>
        </div>
      </ng-template>
    </app-card>
  `,
  styles: [`
    .list-controls { display: flex; flex-direction: column; gap: 12px; margin-bottom: 16px; padding-bottom: 12px; border-bottom: 1px solid #f1f5f9; }
    .search-box input { width: 100%; padding: 8px 12px; border: 1px solid #e2e8f0; border-radius: var(--border-radius); font-size: 0.9rem; }
    .selection-info { display: flex; align-items: center; }
    
    .list { display: flex; flex-direction: column; gap: 4px; }
    .item { display: flex; justify-content: space-between; align-items: center; padding: 10px; border-radius: 6px; border-bottom: 1px solid #f8fafc; transition: background 0.2s; }
    .item:hover { background: #f8fafc; }
    .item.selected { background: #f1f5f9; }
    
    .item-info { display: flex; align-items: center; gap: 12px; }
    .nome { font-weight: 500; color: var(--color-text-primary); }
    .badge { font-size: 0.7rem; font-weight: 600; padding: 2px 8px; border-radius: 10px; text-transform: uppercase; }
    .badge.receita { background: #dcfce7; color: #15803d; }
    :not(.receita).badge { background: #fee2e2; color: #b91c1c; }
    
    .item-actions { display: flex; gap: 4px; }
    .btn-icon { background: none; border: none; cursor: pointer; font-size: 1.1rem; padding: 6px; border-radius: 4px; transition: all 0.2s; color: #64748b; }
    .btn-icon:hover { background: #fff; color: var(--color-primary); box-shadow: var(--shadow-sm); }
    .btn-icon.danger:hover { color: #ef4444; }

    .checkbox-container { display: flex; align-items: center; cursor: pointer; position: relative; padding-left: 24px; }
    .checkbox-container input { position: absolute; opacity: 0; cursor: pointer; height: 0; width: 0; }
    .checkmark { position: absolute; top: 50%; left: 0; transform: translateY(-50%); height: 18px; width: 18px; background-color: #fff; border: 2px solid #cbd5e1; border-radius: 4px; }
    .checkbox-container:hover input ~ .checkmark { border-color: var(--color-primary); }
    .checkbox-container input:checked ~ .checkmark { background-color: var(--color-primary); border-color: var(--color-primary); }
    .checkmark:after { content: ""; position: absolute; display: none; left: 5px; top: 1px; width: 5px; height: 10px; border: solid white; border-width: 0 2px 2px 0; transform: rotate(45deg); }
    .checkbox-container input:checked ~ .checkmark:after { display: block; }

    .empty-state { padding: 32px; text-align: center; color: var(--color-text-secondary); }
  `]
})
export class CategoriasListComponent implements OnChanges {
  @Input() categorias: Categoria[] = [];
  @Input() selecionadas = new Set<string>();
  
  @Output() onIniciarEdicao = new EventEmitter<Categoria>();
  @Output() onExcluirUma = new EventEmitter<Categoria>();
  @Output() onToggleSelecionada = new EventEmitter<string>();
  @Output() onToggleTodas = new EventEmitter<string[]>();

  filtro = '';
  categoriasFiltradas: Categoria[] = [];

  ngOnChanges(changes: SimpleChanges) {
    if (changes['categorias'] || changes['filtro']) {
      this.filtrar();
    }
  }

  filtrar() {
    const lista = this.categorias || [];
    if (!this.filtro) {
      this.categoriasFiltradas = lista;
    } else {
      const f = this.filtro.toLowerCase();
      this.categoriasFiltradas = lista.filter((c: Categoria) => c.nome.toLowerCase().includes(f));
    }
  }

  isTodasSelecionadas(): boolean {
    if (this.categoriasFiltradas.length === 0) return false;
    return this.categoriasFiltradas.every(c => this.selecionadas.has(c.id));
  }

  isParcialmenteSelecionadas(): boolean {
    const count = this.categoriasFiltradas.filter(c => this.selecionadas.has(c.id)).length;
    return count > 0 && count < this.categoriasFiltradas.length;
  }

  toggleTodas(event: any) {
    const checked = event.target.checked;
    const ids = checked ? this.categoriasFiltradas.map(c => c.id) : [];
    this.onToggleTodas.emit(ids);
  }
}
