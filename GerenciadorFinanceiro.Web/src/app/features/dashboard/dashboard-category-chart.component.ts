import { Component, Input, OnChanges, SimpleChanges, ViewChild } from '@angular/core';
import { CommonModule } from '@angular/common';
import { BaseChartDirective } from 'ng2-charts';
import { ChartConfiguration, ChartData, ChartType } from 'chart.js';
import { ResumoCategoria } from '../../core/models/financeiro.model';
import { CardComponent } from '../../shared/components/card/card.component';

@Component({
  selector: 'app-dashboard-category-chart',
  standalone: true,
  imports: [CommonModule, BaseChartDirective, CardComponent],
  template: `
    <app-card title="Distribuição de Gastos">
      <div class="chart-wrapper">
        <canvas baseChart
          [data]="pieChartData"
          [options]="pieChartOptions"
          [type]="pieChartType">
        </canvas>
      </div>
      
      <div *ngIf="!gastos || gastos.length === 0" class="empty-state">
        Sem dados de despesas para o período.
      </div>
    </app-card>
  `,
  styles: [`
    .chart-wrapper {
      display: block;
      height: 300px;
      position: relative;
    }
    .empty-state {
      text-align: center;
      color: var(--color-text-secondary);
      padding: var(--spacing-xl);
    }
  `]
})
export class DashboardCategoryChartComponent implements OnChanges {
  @Input() gastos: ResumoCategoria[] = [];

  @ViewChild(BaseChartDirective) chart: BaseChartDirective | undefined;

  // Configuração do Gráfico de Pizza
  public pieChartOptions: ChartConfiguration['options'] = {
    responsive: true,
    maintainAspectRatio: false,
    plugins: {
      legend: {
        display: true,
        position: 'right',
      }
    }
  };

  public pieChartData: ChartData<'pie', number[], string | string[]> = {
    labels: [],
    datasets: [{
      data: [],
      backgroundColor: [
        '#3b82f6', '#10b981', '#f59e0b', '#ef4444', '#8b5cf6', 
        '#ec4899', '#06b6d4', '#f97316', '#71717a', '#a855f7'
      ]
    }]
  };

  public pieChartType: ChartType = 'pie';

  ngOnChanges(changes: SimpleChanges): void {
    if (changes['gastos'] && this.gastos) {
      this.updateChart();
    }
  }

  private updateChart(): void {
    this.pieChartData.labels = this.gastos.map(g => g.categoria);
    this.pieChartData.datasets[0].data = this.gastos.map(g => g.valor);
    
    // Forçar atualização do gráfico
    this.chart?.update();
  }
}
