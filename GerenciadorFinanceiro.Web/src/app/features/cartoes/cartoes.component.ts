import { Component, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FinanceiroService } from '../../core/services/financeiro.service';
import { CartaoCredito } from '../../core/models/financeiro.model';
import { CardComponent } from '../../shared/components/card/card.component';
import { ButtonComponent } from '../../shared/components/button/button.component';
import { FormsModule } from '@angular/forms';

@Component({
  selector: 'app-cartoes',
  standalone: true,
  imports: [CommonModule, CardComponent, ButtonComponent, FormsModule],
  template: `
    <div class="cartoes">
      <h1>Cartões de Crédito</h1>
      
      <div class="grid">
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
            <app-button type="submit" [disabled]="!novo.nome">Salvar</app-button>
          </form>
        </app-card>

        <app-card title="Cartões Cadastrados">
          <ul class="list">
            <li *ngFor="let c of cartoes()" class="item">
              <div class="info">
                <strong>{{ c.nome }}</strong>
                <small>Vencimento: dia {{ c.diaVencimento }}</small>
              </div>
              <span class="value">{{ c.limite | currency:'BRL' }}</span>
            </li>
          </ul>
        </app-card>
      </div>
    </div>
  `,
  styles: [`
    .grid { display: grid; grid-template-columns: 1fr 1.5fr; gap: var(--spacing-lg); margin-top: var(--spacing-xl); }
    .form { display: flex; flex-direction: column; gap: var(--spacing-md); }
    .form-row { display: grid; grid-template-columns: 1fr 1fr; gap: var(--spacing-md); }
    .form-group { display: flex; flex-direction: column; gap: 4px; }
    input { padding: 8px; border: 1px solid #e2e8f0; border-radius: var(--border-radius); }
    .list { display: flex; flex-direction: column; gap: 8px; }
    .item { display: flex; justify-content: space-between; align-items: center; padding: 12px; border-bottom: 1px solid #f1f5f9; }
    .info { display: flex; flex-direction: column; }
    .info small { color: var(--color-text-secondary); }
    .value { font-weight: 600; }
  `]
})
export class CartoesComponent implements OnInit {
  cartoes = signal<CartaoCredito[]>([]);
  novo = { nome: '', limite: 0, diaFechamento: 1, diaVencimento: 10 };

  constructor(private service: FinanceiroService) {}

  ngOnInit() { this.carregar(); }

  carregar() { this.service.getCartoes().subscribe(data => this.cartoes.set(data)); }

  salvar() {
    this.service.criarCartao(this.novo.nome, this.novo.limite, this.novo.diaFechamento, this.novo.diaVencimento).subscribe(() => {
      this.novo = { nome: '', limite: 0, diaFechamento: 1, diaVencimento: 10 };
      this.carregar();
    });
  }
}
