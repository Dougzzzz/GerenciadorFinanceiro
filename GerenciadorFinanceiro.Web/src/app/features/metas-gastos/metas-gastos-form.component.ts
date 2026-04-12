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
      <form (ngSubmit)="onSalvar.emit(novo)" #metaForm="ngForm" class="form">
        <div class="form-group">
          <label>Categoria</label>
          <select [(ngModel)]="novo.categoriaId" name="categoriaId" required [disabled]="editando">
            <option value="" disabled>Selecione uma categoria</option>
            <option *ngFor="let cat of categorias" [value]="cat.id">{{ cat.nome }}</option>
          </select>
        </div>

        <div class="form-group">
          <label>Valor Limite</label>
          <input type="number" [(ngModel)]="novo.valorLimite" name="valorLimite" required min="0.01" placeholder="0,00">
        </div>

        <div class="row g-2">
          <div class="col-6 form-group">
            <label>Mês (Opcional)</label>
            <input type="number" [(ngModel)]="novo.mes" name="mes" min="1" max="12" placeholder="1-12" [disabled]="editando">
          </div>
          <div class="col-6 form-group">
            <label>Ano (Opcional)</label>
            <input type="number" [(ngModel)]="novo.ano" name="ano" min="2024" placeholder="Ex: 2026" [disabled]="editando">
          </div>
        </div>

        <div class="form-actions">
          <app-button variant="outline" (onClick)="onLimpar.emit()">
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
  @Input() editando: boolean = false;
  @Input() categorias: Categoria[] = [];
  @Input() novo: Partial<MetaGasto> = {
    categoriaId: '',
    valorLimite: 0,
    mes: undefined,
    ano: undefined
  };
  
  @Output() onSalvar = new EventEmitter<Partial<MetaGasto>>();
  @Output() onLimpar = new EventEmitter<void>();
}
