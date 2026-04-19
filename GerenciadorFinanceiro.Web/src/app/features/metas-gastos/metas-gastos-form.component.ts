import { Component, EventEmitter, Input, Output } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { CardComponent } from '../../shared/components/card/card.component';
import { ButtonComponent } from '../../shared/components/button/button.component';
import { Categoria, MetaGasto } from '../../core/models/financeiro.model';

@Component({
  selector: 'app-metas-gastos-form',
  standalone: true,
  imports: [CommonModule, CardComponent, ButtonComponent, FormsModule],
  template: `
    <app-card [title]="editando ? 'Editar Meta' : 'Nova Meta'">
      <form (ngSubmit)="saved.emit(novo)" #metaForm="ngForm" class="form">
        <div class="form-group">
          <label for="categoriaId">Categoria</label>
          <select id="categoriaId" [(ngModel)]="novo.categoriaId" name="categoriaId" required [disabled]="editando">
            <option value="" disabled>Selecione uma categoria</option>
            <option *ngFor="let cat of categorias" [value]="cat.id">{{ cat.nome }}</option>
          </select>
        </div>

        <div class="form-group">
          <label for="valorLimite">Valor Limite</label>
          <input type="number" id="valorLimite" [(ngModel)]="novo.valorLimite" name="valorLimite" required min="0.01" placeholder="0,00">
        </div>

        <div class="row g-2">
          <div class="col-6 form-group">
            <label for="mes">Mês (Opcional)</label>
            <input type="number" id="mes" [(ngModel)]="novo.mes" name="mes" min="1" max="12" placeholder="1-12" [disabled]="editando">
          </div>
          <div class="col-6 form-group">
            <label for="ano">Ano (Opcional)</label>
            <input type="number" id="ano" [(ngModel)]="novo.ano" name="ano" min="2024" placeholder="Ex: 2026" [disabled]="editando">
          </div>
        </div>

        <div class="form-actions">
          <app-button variant="outline" (clicked)="cleared.emit()">
            {{ editando ? 'Cancelar' : 'Limpar' }}
          </app-button>
          <app-button type="submit" [disabled]="!metaForm.form.valid || !novo.categoriaId">
            {{ editando ? 'Atualizar' : 'Salvar' }}
          </app-button>
        </div>
      </form>
    </app-card>
  `,
  styles: [`
    .form { display: flex; flex-direction: column; gap: var(--spacing-md); }
    .form-group { display: flex; flex-direction: column; gap: 4px; }
    input, select { padding: 8px; border: 1px solid #e2e8f0; border-radius: var(--border-radius); }
    input:disabled, select:disabled { background-color: #f1f5f9; color: #94a3b8; cursor: not-allowed; }
    .form-actions { display: flex; justify-content: flex-end; gap: var(--spacing-md); margin-top: var(--spacing-sm); }
  `]
})
export class MetasGastosFormComponent {
  @Input() editando = false;
  @Input() categorias: Categoria[] = [];
  @Input() novo: Partial<MetaGasto> = {
    categoriaId: '',
    valorLimite: 0,
    mes: undefined,
    ano: undefined
  };
  
  @Output() saved = new EventEmitter<Partial<MetaGasto>>();
  @Output() cleared = new EventEmitter<void>();
}
