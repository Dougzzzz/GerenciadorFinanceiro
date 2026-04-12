import { Component, Output, EventEmitter } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { CardComponent } from '../../shared/components/card/card.component';
import { ButtonComponent } from '../../shared/components/button/button.component';

@Component({
  selector: 'app-cartoes-form',
  standalone: true,
  imports: [CommonModule, CardComponent, ButtonComponent, FormsModule],
  template: `
    <app-card title="Novo Cartão">
      <form (submit)="salvar()" class="form">
        <div class="form-group">
          <label>Nome do Cartão</label>
          <input type="text" [(ngModel)]="novo.nome" name="nome" required>
        </div>
        <div class="form-group">
          <label>Limite (R$)</label>
          <input type="number" step="0.01" [(ngModel)]="novo.limite" name="limite">
        </div>
        <div class="form-row">
          <div class="form-group">
            <label>Dia Fechamento</label>
            <input type="number" [(ngModel)]="novo.diaFechamento" name="diaFechamento">
          </div>
          <div class="form-group">
            <label>Dia Vencimento</label>
            <input type="number" [(ngModel)]="novo.diaVencimento" name="diaVencimento">
          </div>
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
        <app-button type="submit" [disabled]="!novo.nome">Salvar</app-button>
      </form>
    </app-card>
  `,
  styles: [`
    .form { display: flex; flex-direction: column; gap: var(--spacing-md); }
    .form-row { display: grid; grid-template-columns: 1fr 1fr; gap: var(--spacing-md); }
    .form-group { display: flex; flex-direction: column; gap: 4px; }
    input, select { padding: 8px; border: 1px solid #e2e8f0; border-radius: var(--border-radius); }
  `]
})
export class CartoesFormComponent {
  @Output() onSalvar = new EventEmitter<any>();

  novo = { nome: '', limite: 0, diaFechamento: 1, diaVencimento: 10, provedor: 0 };

  salvar() {
    this.onSalvar.emit({ ...this.novo });
    this.novo = { nome: '', limite: 0, diaFechamento: 1, diaVencimento: 10, provedor: 0 };
  }
}
