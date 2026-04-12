import { Component, Input } from '@angular/core';
import { CommonModule } from '@angular/common';
import { CardComponent } from '../../shared/components/card/card.component';
import { MetaResumo } from '../../core/models/financeiro.model';

@Component({
  selector: 'app-dashboard-spending-chart',
  standalone: true,
  imports: [CommonModule, CardComponent],
  template: `
    <app-card title="Metas vs Gastos por Categoria">
      <div class="chart-container">
        <div *ngFor="let item of metasResumo" class="category-group">
          <div class="category-header">
            <span class="category-name">{{ item.categoria }}</span>
            <span class="usage-percent" [class.exceeded]="item.gastoAtual > item.meta">
              {{ item.percentual | percent }}
            </span>
          </div>
          
          <div class="bars-container">
            <!-- Barra de Meta -->
            <div class="bar-wrapper">
              <div class="bar meta-bar" [style.width.%]="100" title="Meta: {{ item.meta | currency:'BRL' }}">
                <span class="bar-label">Meta: {{ item.meta | currency:'BRL' }}</span>
              </div>
            </div>
            
            <!-- Barra de Gasto Atual -->
            <div class="bar-wrapper">
              <div class="bar actual-bar" 
                   [class.exceeded]="item.gastoAtual > item.meta"
                   [style.width.%]="calculateWidth(item)" 
                   title="Gasto: {{ item.gastoAtual | currency:'BRL' }}">
                <span class="bar-label">Gasto: {{ item.gastoAtual | currency:'BRL' }}</span>
              </div>
            </div>
          </div>
        </div>
        
        <div *ngIf="metasResumo.length === 0" class="empty-state">
          Nenhuma meta de gasto definida para este mês.
        </div>
      </div>
    </app-card>
  `,
  styles: [`
    .chart-container {
      display: flex;
      flex-direction: column;
      gap: var(--spacing-lg);
    }

    .category-group {
      display: flex;
      flex-direction: column;
      gap: 8px;
    }

    .category-header {
      display: flex;
      justify-content: space-between;
      align-items: center;
      font-size: 0.875rem;
      font-weight: 500;
    }

    .usage-percent {
      color: var(--color-text-secondary);
      font-size: 0.75rem;
    }

    .usage-percent.exceeded {
      color: var(--color-danger, #dc3545);
      font-weight: bold;
    }

    .bars-container {
      display: flex;
      flex-direction: column;
      gap: 4px;
      background: rgba(0,0,0,0.03);
      padding: 8px;
      border-radius: 4px;
    }

    .bar-wrapper {
      height: 20px;
      width: 100%;
      background: rgba(0,0,0,0.05);
      border-radius: 2px;
      overflow: hidden;
      position: relative;
    }

    .bar {
      height: 100%;
      display: flex;
      align-items: center;
      padding-left: 8px;
      transition: width 0.3s ease;
      white-space: nowrap;
      min-width: 2px;
    }

    .meta-bar {
      background-color: var(--color-primary-light, #a5d8ff);
      color: var(--color-primary-dark, #1864ab);
    }

    .actual-bar {
      background-color: var(--color-expense, #ffa8a8);
      color: var(--color-expense-dark, #c92a2a);
    }

    .actual-bar.exceeded {
      background-color: var(--color-danger, #dc3545);
      color: white;
    }

    .bar-label {
      font-size: 0.7rem;
      font-weight: 600;
      z-index: 1;
    }

    .empty-state {
      text-align: center;
      color: var(--color-text-secondary);
      padding: var(--spacing-xl) !important;
    }
  `]
})
export class DashboardSpendingChartComponent {
  @Input() metasResumo: MetaResumo[] = [];

  calculateWidth(item: MetaResumo): number {
    if (item.meta === 0) return 0;
    const percent = (item.gastoAtual / item.meta) * 100;
    return Math.min(percent, 100); // Limita a 100% para visualização na barra, mas a cor indica excesso
  }
}
