import { Component, Input, Output, EventEmitter } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { CardComponent } from '../../shared/components/card/card.component';
import { ButtonComponent } from '../../shared/components/button/button.component';
import { TipoTransacao } from '../../core/models/financeiro.model';

export interface SaveCategoriaData {
  nome: string;
  tipo: TipoTransacao;
}

@Component({
  selector: 'app-categorias-form',
  standalone: true,
  imports: [CommonModule, CardComponent, ButtonComponent, FormsModule],
  template: `
    <app-card [title]="editando ? 'Editar Categoria' : 'Nova Categoria'">
      <form (submit)="saved.emit(novo)" class="form">
        <div class="form-group">
          <label for="nome">Nome</label>
          <input type="text" id="nome" [(ngModel)]="novo.nome" name="nome" required>
        </div>
        <div class="form-group">
          <label for="tipo">Tipo</label>
          <select id="tipo" [(ngModel)]="novo.tipo" name="tipo">
            <option [value]="0">Receita</option>
            <option [value]="1">Despesa</option>
          </select>
        </div>
        <div class="form-actions">
          <app-button *ngIf="editando" variant="outline" (clicked)="cleared.emit()">
            Cancelar
          </app-button>
          <app-button type="submit" [disabled]="!novo.nome">
            {{ editando ? 'Atualizar' : 'Salvar' }}
          </app-button>
        </div>
      </form>
    </app-card>
  `,
  styles: [`
    .form { display: flex; flex-direction: column; gap: var(--spacing-md); }
    .form-group { display: flex; flex-direction: column; gap: 4px; }
    input[type="text"], select { padding: 8px; border: 1px solid #e2e8f0; border-radius: var(--border-radius); }
    .form-actions { display: flex; justify-content: flex-end; gap: var(--spacing-md); }
  `]
})
export class CategoriasFormComponent {
  @Input() editando = false;
  @Input() novo: SaveCategoriaData = { nome: '', tipo: TipoTransacao.Despesa };
  @Output() saved = new EventEmitter<SaveCategoriaData>();
  @Output() cleared = new EventEmitter<void>();
}
