import { Component, Input } from '@angular/core';
import { CommonModule } from '@angular/common';
import { CardComponent } from '../../shared/components/card/card.component';

@Component({
  selector: 'app-dashboard-summary',
  standalone: true,
  imports: [CommonModule, CardComponent],
  template: `
    <div class="summary-grid">
      <app-card class="card-income">
        <div class="summary-content">
          <span class="label">Total Receitas</span>
          <span class="value">{{ totalReceitas | currency:'BRL' }}</span>
        </div>
      </app-card>

      <app-card class="card-expense">
        <div class="summary-content">
          <span class="label">Total Despesas</span>
          <span class="value">{{ totalDespesas | currency:'BRL' }}</span>
        </div>
      </app-card>

      <app-card class="card-balance">
        <div class="summary-content">
          <span class="label">Saldo Atual</span>
          <span class="value" [class.negative]="saldo < 0">{{ saldo | currency:'BRL' }}</span>
        </div>
      </app-card>
    </div>
  `,
  styles: [`
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
  `]
})
export class DashboardSummaryComponent {
  @Input() totalReceitas = 0;
  @Input() totalDespesas = 0;
  @Input() saldo = 0;
}
