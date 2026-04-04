import { Component, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FinanceiroService } from '../../core/services/financeiro.service';
import { ContaBancaria } from '../../core/models/financeiro.model';
import { CardComponent } from '../../shared/components/card/card.component';
import { ButtonComponent } from '../../shared/components/button/button.component';
import { FormsModule } from '@angular/forms';

@Component({
  selector: 'app-contas',
  standalone: true,
  imports: [CommonModule, CardComponent, ButtonComponent, FormsModule],
  template: `
    <div class="contas">
      <h1>Contas Bancárias</h1>
      
      <div class="grid">
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
            <app-button type="submit" [disabled]="!novo.nomeBanco">Salvar</app-button>
          </form>
        </app-card>

        <app-card title="Contas Cadastradas">
          <ul class="list">
            <li *ngFor="let c of contas()" class="item">
              <span>{{ c.nomeBanco }}</span>
              <span class="value" [class.negative]="c.saldoAtual < 0">{{ c.saldoAtual | currency:'BRL' }}</span>
            </li>
          </ul>
        </app-card>
      </div>
    </div>
  `,
  styles: [`
    .grid { display: grid; grid-template-columns: 1fr 1.5fr; gap: var(--spacing-lg); margin-top: var(--spacing-xl); }
    .form { display: flex; flex-direction: column; gap: var(--spacing-md); }
    .form-group { display: flex; flex-direction: column; gap: 4px; }
    input { padding: 8px; border: 1px solid #e2e8f0; border-radius: var(--border-radius); }
    .list { display: flex; flex-direction: column; gap: 8px; }
    .item { display: flex; justify-content: space-between; padding: 8px; border-bottom: 1px solid #f1f5f9; }
    .value { font-weight: 600; color: var(--color-income); }
    .value.negative { color: var(--color-expense); }
  `]
})
export class ContasComponent implements OnInit {
  contas = signal<ContaBancaria[]>([]);
  novo = { nomeBanco: '', saldoInicial: 0 };

  constructor(private service: FinanceiroService) {}

  ngOnInit() { this.carregar(); }

  carregar() { this.service.getContas().subscribe(data => this.contas.set(data)); }

  salvar() {
    this.service.criarConta(this.novo.nomeBanco, this.novo.saldoInicial).subscribe(() => {
      this.novo = { nomeBanco: '', saldoInicial: 0 };
      this.carregar();
    });
  }
}
