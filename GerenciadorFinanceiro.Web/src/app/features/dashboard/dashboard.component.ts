import { Component, OnInit, signal, computed } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FinanceiroService } from '../../core/services/financeiro.service';
import { Transacao, TipoTransacao } from '../../core/models/financeiro.model';
import { CardComponent } from '../../shared/components/card/card.component';

@Component({
  selector: 'app-dashboard',
  standalone: true,
  imports: [CommonModule, CardComponent],
  template: `
    <div class="dashboard">
      <header class="dashboard-header">
        <h1>Dashboard</h1>
        <p>Bem-vindo ao seu controle financeiro</p>
      </header>

      <div class="summary-grid">
        <app-card class="card-income">
          <div class="summary-content">
            <span class="label">Total Receitas</span>
            <span class="value">{{ totalReceitas() | currency:'BRL' }}</span>
          </div>
        </app-card>

        <app-card class="card-expense">
          <div class="summary-content">
            <span class="label">Total Despesas</span>
            <span class="value">{{ totalDespesas() | currency:'BRL' }}</span>
          </div>
        </app-card>

        <app-card class="card-balance">
          <div class="summary-content">
            <span class="label">Saldo Atual</span>
            <span class="value" [class.negative]="saldo() < 0">{{ saldo() | currency:'BRL' }}</span>
          </div>
        </app-card>
      </div>

      <div class="recent-transactions">
        <app-card title="Últimas Transações">
          <table class="table">
            <thead>
              <tr>
                <th>Data</th>
                <th>Descrição</th>
                <th>Categoria</th>
                <th>Valor</th>
              </tr>
            </thead>
            <tbody>
              <tr *ngFor="let t of transacoesRecentes()">
                <td>{{ t.data | date:'dd/MM/yyyy' }}</td>
                <td>{{ t.descricao }}</td>
                <td><span class="badge">{{ t.categoria || 'Sem Categoria' }}</span></td>
                <td [class.income]="t.valor > 0" [class.expense]="t.valor < 0">
                  {{ t.valor | currency:'BRL' }}
                </td>
              </tr>
              <tr *ngIf="transacoes().length === 0">
                <td colspan="4" class="empty-state">Nenhuma transação encontrada.</td>
              </tr>
            </tbody>
          </table>
        </app-card>
      </div>
    </div>
  `,
  styles: [`
    .dashboard-header {
      margin-bottom: var(--spacing-xl);
    }

    .summary-grid {
      display: grid;
      grid-template-columns: repeat(auto-fit, minmax(280px, 1fr));
      gap: var(--spacing-lg);
      margin-bottom: var(--spacing-xl);
    }

    .summary-content {
      display: flex;
      flex-direction: column;
    }

    .summary-content .label {
      font-size: 0.875rem;
      color: var(--color-text-secondary);
      margin-bottom: var(--spacing-xs);
    }

    .summary-content .value {
      font-size: 1.875rem;
      font-weight: 700;
    }

    .card-income { border-left: 4px solid var(--color-income); }
    .card-expense { border-left: 4px solid var(--color-expense); }
    .card-balance { border-left: 4px solid var(--color-primary); }

    .value.negative { color: var(--color-expense); }

    .table {
      width: 100%;
      border-collapse: collapse;
    }

    .table th {
      text-align: left;
      padding: var(--spacing-md);
      color: var(--color-text-secondary);
      font-weight: 500;
      border-bottom: 1px solid #e2e8f0;
    }

    .table td {
      padding: var(--spacing-md);
      border-bottom: 1px solid #f1f5f9;
    }

    .income { color: var(--color-income); font-weight: 600; }
    .expense { color: var(--color-expense); font-weight: 600; }

    .badge {
      background-color: #f1f5f9;
      padding: 2px 8px;
      border-radius: 12px;
      font-size: 0.75rem;
    }

    .empty-state {
      text-align: center;
      color: var(--color-text-secondary);
      padding: var(--spacing-xl) !important;
    }
  `]
})
export class DashboardComponent implements OnInit {
  transacoes = signal<Transacao[]>([]);

  totalReceitas = computed(() => 
    this.transacoes()
      .filter(t => t.valor > 0)
      .reduce((acc, t) => acc + t.valor, 0)
  );

  totalDespesas = computed(() => 
    Math.abs(this.transacoes()
      .filter(t => t.valor < 0)
      .reduce((acc, t) => acc + t.valor, 0))
  );

  saldo = computed(() => this.totalReceitas() - this.totalDespesas());

  transacoesRecentes = computed(() => 
    [...this.transacoes()]
      .sort((a, b) => new Date(b.data).getTime() - new Date(a.data).getTime())
      .slice(0, 5)
  );

  constructor(private financeiroService: FinanceiroService) {}

  ngOnInit(): void {
    this.carregarDados();
  }

  carregarDados(): void {
    this.financeiroService.getTransacoes().subscribe(data => {
      this.transacoes.set(data);
    });
  }
}
