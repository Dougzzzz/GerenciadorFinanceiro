import { Component, Output, EventEmitter } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { CardComponent } from '../../shared/components/card/card.component';
import { ButtonComponent } from '../../shared/components/button/button.component';

@Component({
  selector: 'app-contas-form',
  standalone: true,
  imports: [CommonModule, CardComponent, ButtonComponent, FormsModule],
  template: `
    <app-card title="Nova Conta">
      <form (submit)="salvar()" class="form">
        <div class="form-group">
          <label>Nome do Banco</label>
          <input type="text" [(ngModel)]="novo.nomeBanco" name="nomeBanco" required>
        </div>
        <div class="form-group">
          <label>Saldo Inicial (R$)</label>
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
        <app-button type="submit" [disabled]="!novo.nomeBanco">Salvar</app-button>
      </form>
    </app-card>
  `,
  styles: [`
    .form { display: flex; flex-direction: column; gap: var(--spacing-md); }
    .form-group { display: flex; flex-direction: column; gap: 4px; }
    input, select { padding: 8px; border: 1px solid #e2e8f0; border-radius: var(--border-radius); }
  `]
})
export class ContasFormComponent {
  @Output() onSalvar = new EventEmitter<{ nomeBanco: string, saldoInicial: number, provedor: number }>();

  novo = { nomeBanco: '', saldoInicial: 0, provedor: 0 };

  salvar() {
    this.onSalvar.emit({ ...this.novo });
    this.novo = { nomeBanco: '', saldoInicial: 0, provedor: 0 };
  }
}
