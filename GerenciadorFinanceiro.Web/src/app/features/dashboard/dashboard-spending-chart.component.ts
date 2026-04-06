import { Component, Input } from '@angular/core';
import { CommonModule } from '@angular/common';
import { CardComponent } from '../../shared/components/card/card.component';
import { ProgressBarComponent } from '../../shared/components/progress-bar/progress-bar.component';

@Component({
  selector: 'app-dashboard-spending-chart',
  standalone: true,
  imports: [CommonModule, CardComponent, ProgressBarComponent],
  template: `
    <app-card title="Gastos por Categoria">
      <div class="chart-container">
        <div *ngFor="let item of gastos" class="chart-item">
          <div class="chart-info">
            <span class="category-name">{{ item.categoria }}</span>
            <span class="category-value">{{ item.valor | currency:'BRL' }}</span>
          </div>
          <app-progress-bar [value]="item.percentual" color="var(--color-expense)"></app-progress-bar>
        </div>
        <div *ngIf="gastos.length === 0" class="empty-state">
          Sem dados de despesas para exibir.
        </div>
      </div>
    </app-card>
  `,
  styles: [`
    .chart-container {
      display: flex;
      flex-direction: column;
      gap: var(--spacing-md);
    }

    .chart-item {
      display: flex;
      flex-direction: column;
      gap: 4px;
    }

    .chart-info {
      display: flex;
      justify-content: space-between;
      font-size: 0.875rem;
    }

    .category-name { font-weight: 500; }
    .category-value { color: var(--color-text-secondary); }

    .empty-state {
      text-align: center;
      color: var(--color-text-secondary);
      padding: var(--spacing-xl) !important;
    }
  `]
})
export class DashboardSpendingChartComponent {
  @Input() gastos: { categoria: string; valor: number; percentual: number }[] = [];
}
