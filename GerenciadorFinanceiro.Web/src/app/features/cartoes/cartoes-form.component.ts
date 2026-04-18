import { Component, Output, EventEmitter, Input, OnChanges, SimpleChanges } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { CardComponent } from '../../shared/components/card/card.component';
import { ButtonComponent } from '../../shared/components/button/button.component';
import { CartaoCredito } from '../../core/models/financeiro.model';

@Component({
  selector: 'app-cartoes-form',
  standalone: true,
  imports: [CommonModule, CardComponent, ButtonComponent, FormsModule],
  template: `
    <app-card [title]="editando ? 'Editar Cartão' : 'Novo Cartão'">
      <form (submit)="salvar()" class="form">
        <div class="form-group">
          <label>Nome do Cartão</label>
          <input type="text" [(ngModel)]="dados.nome" name="nome" required>
        </div>
        <div class="form-group">
          <label>Limite (R$)</label>
          <input type="number" step="0.01" [(ngModel)]="dados.limite" name="limite">
        </div>
        <div class="form-row">
          <div class="form-group">
            <label>Dia Fechamento</label>
            <input type="number" [(ngModel)]="dados.diaFechamento" name="diaFechamento">
          </div>
          <div class="form-group">
            <label>Dia Vencimento</label>
            <input type="number" [(ngModel)]="dados.diaVencimento" name="diaVencimento">
          </div>
        </div>
        <div class="form-group">
          <label>Provedor de Extrato</label>
          <select [(ngModel)]="dados.provedor" name="provedor">
            <option [value]="0">Genérico / Padrão</option>
            <option [value]="1">C6 Bank</option>
            <option [value]="2">Nubank</option>
            <option [value]="3">Inter</option>
          </select>
        </div>
        <div class="actions">
          <app-button type="submit" [disabled]="!dados.nome">Salvar</app-button>
          <app-button *ngIf="editando" variant="outline" (click)="cancelar()">Cancelar</app-button>
        </div>
      </form>
    </app-card>
  `,
  styles: [`
    .form { display: flex; flex-direction: column; gap: var(--spacing-md); }
    .form-row { display: grid; grid-template-columns: 1fr 1fr; gap: var(--spacing-md); }
    .form-group { display: flex; flex-direction: column; gap: 4px; }
    input, select { padding: 8px; border: 1px solid #e2e8f0; border-radius: var(--border-radius); }
    .actions { display: flex; gap: var(--spacing-sm); }
  `]
})
export class CartoesFormComponent implements OnChanges {
  @Input() cartao: CartaoCredito | null = null;
  @Output() onSalvar = new EventEmitter<Partial<CartaoCredito>>();
  @Output() onCancelar = new EventEmitter<void>();

  dados: Partial<CartaoCredito> = this.resetDados();
  editando = false;

  ngOnChanges(changes: SimpleChanges) {
    if (changes['cartao'] && this.cartao) {
      this.dados = { ...this.cartao };
      this.editando = true;
    } else if (changes['cartao'] && !this.cartao) {
      this.dados = this.resetDados();
      this.editando = false;
    }
  }

  salvar() {
    this.onSalvar.emit({ ...this.dados });
    this.dados = this.resetDados();
    this.editando = false;
  }

  cancelar() {
    this.onCancelar.emit();
    this.dados = this.resetDados();
    this.editando = false;
  }

  private resetDados(): Partial<CartaoCredito> {
    return { nome: '', limite: 0, diaFechamento: 1, diaVencimento: 10, provedor: 0 };
  }
}
