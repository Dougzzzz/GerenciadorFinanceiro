import { Component, Output, EventEmitter, Input } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { CardComponent } from '../../shared/components/card/card.component';
import { ButtonComponent } from '../../shared/components/button/button.component';

@Component({
  selector: 'app-contas-form',
  standalone: true,
  imports: [CommonModule, CardComponent, ButtonComponent, FormsModule],
  template: `
    <app-card [title]="editando ? 'Editar Conta' : 'Nova Conta'">
      <form (submit)="salvar()" class="form">
        <div class="form-group">
          <label>Nome do Banco</label>
          <input type="text" [(ngModel)]="novo.nomeBanco" name="nomeBanco" required>
        </div>
        <div class="form-group">
          <label>{{ editando ? 'Saldo Atual' : 'Saldo Inicial' }} (R$)</label>
          <input type="number" step="0.01" [(ngModel)]="novo.saldoInicial" name="saldoInicial">
        </div>
        <div class="form-group">
          <label>Provedor de Extrato</label>
          <select [(ngModel)]="novo.provedor" name="provedor">
            <option [value]="0">Genérico / Padrão</option>
            <option [value]="1">C6 Bank</option>
            <option [value]="2">Nubank</option>
            <option [value]="3">Inter</option>
          </select>
        </div>
        <div class="actions">
            <app-button type="submit" [disabled]="!novo.nomeBanco">
                {{ editando ? 'Atualizar' : 'Salvar' }}
            </app-button>
            <app-button *ngIf="editando" variant="outline" (onClick)="onLimpar.emit()">
                Cancelar
            </app-button>
        </div>
      </form>
    </app-card>
  `,
  styles: [`
    .form { display: flex; flex-direction: column; gap: var(--spacing-md); }
    .form-group { display: flex; flex-direction: column; gap: 4px; }
    input, select { padding: 8px; border: 1px solid #e2e8f0; border-radius: var(--border-radius); }
    .actions { display: flex; gap: 8px; }
  `]
})
export class ContasFormComponent {
  @Input() editando = false;
  @Input() novo = { nomeBanco: '', saldoInicial: 0, provedor: 0 };
  @Output() onSalvar = new EventEmitter<{ nomeBanco: string, saldoInicial: number, provedor: number }>();
  @Output() onLimpar = new EventEmitter<void>();

  salvar() {
    this.onSalvar.emit({ ...this.novo });
  }
}
