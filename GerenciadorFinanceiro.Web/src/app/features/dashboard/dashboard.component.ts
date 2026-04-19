import { Component, OnInit, signal, computed } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FinanceiroService } from '../../core/services/financeiro.service';
import { Transacao, MetaResumo, ResumoMensal } from '../../core/models/financeiro.model';
import { DashboardSummaryComponent } from './dashboard-summary.component';
import { DashboardRecentTransactionsComponent } from './dashboard-recent-transactions.component';
import { DashboardSpendingChartComponent } from './dashboard-spending-chart.component';
import { DashboardCategoryChartComponent } from './dashboard-category-chart.component';
import { provideCharts, withDefaultRegisterables } from 'ng2-charts';

@Component({
  selector: 'app-dashboard',
  standalone: true,
  providers: [provideCharts(withDefaultRegisterables())],
  imports: [
    CommonModule,
    DashboardSummaryComponent,
    DashboardRecentTransactionsComponent,
    DashboardSpendingChartComponent,
    DashboardCategoryChartComponent,
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

        <div class="charts-column">
          <app-dashboard-category-chart
            [gastos]="resumo()?.gastosPorCategoria || []">
          </app-dashboard-category-chart>

          <app-dashboard-spending-chart
            [metasResumo]="metasResumo()">
          </app-dashboard-spending-chart>
        </div>
      </div>
    </div>
  `,
  styles: [`
    .dashboard-header {
      margin-bottom: var(--spacing-xl);
    }

    .dashboard-grid {
      display: grid;
      grid-template-columns: 1fr 1fr;
      gap: var(--spacing-lg);
    }

    .charts-column {
      display: flex;
      flex-direction: column;
      gap: var(--spacing-lg);
    }

    @media (max-width: 1024px) {
      .dashboard-grid { 
        grid-template-columns: 1fr; 
      }
    }
  `],
})
export class DashboardComponent implements OnInit {
  transacoes = signal<Transacao[]>([]);
  metasResumo = signal<MetaResumo[]>([]);
  resumo = signal<ResumoMensal | null>(null);

  // Computeds para manter compatibilidade com testes existentes
  totalReceitas = computed(() => this.resumo()?.totalReceitas || 0);
  totalDespesas = computed(() => this.resumo()?.totalDespesas || 0);
  saldo = computed(() => this.resumo()?.saldo || 0);

  transacoesRecentes = computed(() =>
    [...this.transacoes()]
      .sort((a, b) => new Date(b.data).getTime() - new Date(a.data).getTime())
      .slice(0, 5)
  );

  constructor(private financeiroService: FinanceiroService) { }

  ngOnInit(): void {
    this.carregarDados();
  }

  carregarDados(): void {
    this.financeiroService.getTransacoes().subscribe(data => {
      this.transacoes.set(data);
    });

    const hoje = new Date();
    const mes = hoje.getMonth() + 1;
    const ano = hoje.getFullYear();

    this.financeiroService.getResumoMensal(mes, ano).subscribe(data => {
      this.resumo.set(data);
    });

    this.financeiroService.getResumoMetas(mes, ano).subscribe(data => {
      this.metasResumo.set(data);
    });
  }
}
