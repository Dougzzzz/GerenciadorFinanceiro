import { Component, OnInit, signal, computed } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FinanceiroService } from '../../core/services/financeiro.service';
import { Transacao } from '../../core/models/financeiro.model';
import { DashboardSummaryComponent } from './dashboard-summary.component';
import { DashboardRecentTransactionsComponent } from './dashboard-recent-transactions.component';
import { DashboardSpendingChartComponent } from './dashboard-spending-chart.component';

@Component({
  selector: 'app-dashboard',
  standalone: true,
  imports: [
    CommonModule, 
    DashboardSummaryComponent, 
    DashboardRecentTransactionsComponent, 
    DashboardSpendingChartComponent
  ],
  template: `
    <div class="dashboard">
      <header class="dashboard-header">
        <h1>Dashboard</h1>
        <p>Bem-vindo ao seu controle financeiro</p>
      </header>

      <app-dashboard-summary
        [totalReceitas]="totalReceitas()"
        [totalDespesas]="totalDespesas()"
        [saldo]="saldo()">
      </app-dashboard-summary>

      <div class="dashboard-grid">
        <app-dashboard-recent-transactions
          [transacoes]="transacoesRecentes()">
        </app-dashboard-recent-transactions>

        <app-dashboard-spending-chart
          [gastos]="gastosPorCategoria()">
        </app-dashboard-spending-chart>
      </div>
    </div>
  `,
  styles: [`
    .dashboard-header {
      margin-bottom: var(--spacing-xl);
    }

    .dashboard-grid {
      display: grid;
      grid-template-columns: 1.5fr 1fr;
      gap: var(--spacing-lg);
    }

    @media (max-width: 1024px) {
      .dashboard-grid { 
        grid-template-columns: 1fr; 
      }
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

  gastosPorCategoria = computed(() => {
    const despesas = this.transacoes().filter(t => t.valor < 0);
    const total = Math.abs(despesas.reduce((acc, t) => acc + t.valor, 0));
    
    if (total === 0) return [];

    const grupos: Record<string, number> = {};
    despesas.forEach(t => {
      const cat = t.categoriaNavigation?.nome || t.categoria || 'Sem Categoria';
      grupos[cat] = (grupos[cat] || 0) + Math.abs(t.valor);
    });

    return Object.entries(grupos)
      .map(([categoria, valor]) => ({
        categoria,
        valor,
        percentual: (valor / total) * 100
      }))
      .sort((a, b) => b.valor - a.valor);
  });

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
