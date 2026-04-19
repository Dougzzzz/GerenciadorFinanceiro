import { Component, Input, Output, EventEmitter } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { CardComponent } from '../../shared/components/card/card.component';
import { ButtonComponent } from '../../shared/components/button/button.component';
import { Categoria, ContaBancaria, CartaoCredito } from '../../core/models/financeiro.model';

export interface SaveTransacaoData {
  data: string;
  descricao: string;
  valor: number;
  categoriaId: string;
  contaBancariaId?: string;
  cartaoCreditoId?: string;
}

@Component({
  selector: 'app-transacoes-form',
  standalone: true,
  imports: [CommonModule, CardComponent, ButtonComponent, FormsModule],
  template: `
    <app-card title="Nova Transação">
      <form (submit)="salvar()" class="form-grid">
        <div class="form-group">
          <label for="data">Data</label>
          <input type="date" id="data" [(ngModel)]="dados.data" name="data" required>
        </div>
        <div class="form-group">
          <label for="descricao">Descrição</label>
          <input type="text" id="descricao" [(ngModel)]="dados.descricao" name="descricao" placeholder="Ex: Almoço" required>
        </div>
        <div class="form-group">
          <label for="valor">Valor (R$)</label>
          <input type="number" id="valor" step="0.01" [(ngModel)]="dados.valor" name="valor" placeholder="Negativo para despesas" required>
        </div>
        <div class="form-group">
          <label for="categoriaId">Categoria</label>
          <select id="categoriaId" [(ngModel)]="dados.categoriaId" name="categoriaId" required>
            <option *ngFor="let c of categorias" [value]="c.id">{{ c.nome }}</option>
          </select>
        </div>
        <div class="form-group">
          <label for="contaBancariaId">Conta (Opcional)</label>
          <select id="contaBancariaId" [(ngModel)]="dados.contaBancariaId" name="contaBancariaId">
            <option [value]="undefined">Nenhuma</option>
            <option *ngFor="let c of contas" [value]="c.id">{{ c.nomeBanco }}</option>
          </select>
        </div>
        <div class="form-group">
          <label for="cartaoCreditoId">Cartão (Opcional)</label>
          <select id="cartaoCreditoId" [(ngModel)]="dados.cartaoCreditoId" name="cartaoCreditoId">
            <option [value]="undefined">Nenhum</option>
            <option *ngFor="let c of cartoes" [value]="c.id">{{ c.nome }}</option>
          </select>
        </div>
        <div class="form-actions-full">
          <app-button type="submit" [disabled]="!dados.descricao || !dados.valor || !dados.categoriaId">
            Salvar Transação
          </app-button>
        </div>
      </form>
    </app-card>
  `,
  styles: [`
    .form-grid { display: grid; grid-template-columns: repeat(auto-fit, minmax(200px, 1fr)); gap: var(--spacing-md); }
    .form-group { display: flex; flex-direction: column; gap: var(--spacing-xs); }
    .form-group label { font-size: 0.875rem; font-weight: 500; color: var(--color-text-secondary); }
    select, input { padding: var(--spacing-sm); border-radius: var(--border-radius); border: 1px solid #e2e8f0; background: white; font-size: 0.875rem; }
    .form-actions-full { grid-column: 1 / -1; display: flex; justify-content: flex-end; margin-top: var(--spacing-md); }
  `]
})
export class TransacoesFormComponent {
  @Input() categorias: Categoria[] = [];
  @Input() contas: ContaBancaria[] = [];
  @Input() cartoes: CartaoCredito[] = [];
  @Output() saved = new EventEmitter<SaveTransacaoData>();

  // Helper para pegar a data atual no fuso horário local formatada para o input (yyyy-MM-dd)
  private getDataAtualLocal(): string {
    const d = new Date();
    // Ajusta o offset do fuso horário manualmente para pegar a data "civil" local
    d.setMinutes(d.getMinutes() - d.getTimezoneOffset());
    return d.toISOString().split('T')[0];
  }

  dados: SaveTransacaoData = {
    data: this.getDataAtualLocal(),
    descricao: '',
    valor: 0,
    categoriaId: '',
    contaBancariaId: undefined,
    cartaoCreditoId: undefined
  };

  salvar() {
    this.saved.emit(this.dados);
    this.dados = {
      data: this.getDataAtualLocal(),
      descricao: '',
      valor: 0,
      categoriaId: '',
      contaBancariaId: undefined,
      cartaoCreditoId: undefined
    };
  }
}
