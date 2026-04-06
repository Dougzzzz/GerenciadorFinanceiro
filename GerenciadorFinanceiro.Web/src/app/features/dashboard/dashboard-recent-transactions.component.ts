import { Component, Input } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Transacao } from '../../core/models/financeiro.model';
import { CardComponent } from '../../shared/components/card/card.component';

@Component({
  selector: 'app-dashboard-recent-transactions',
  standalone: true,
  imports: [CommonModule, CardComponent],
  template: `
    <app-card title="Últimas Transações">
      <table class="table">
        <thead>
          <tr>
            <th>Data</th>
            <th>Descrição</th>
            <th>Categoria</th>
            <th class="text-right">Valor</th>
          </tr>
        </thead>
        <tbody>
          <tr *ngFor="let t of transacoes">
            <td>{{ t.data | date:'dd/MM/yyyy' }}</td>
            <td>{{ t.descricao }}</td>
            <td><span class="badge">{{ t.categoriaNavigation?.nome || t.categoria || 'Sem Categoria' }}</span></td>
            <td class="text-right" [class.income]="t.valor > 0" [class.expense]="t.valor < 0">
              {{ t.valor | currency:'BRL' }}
            </td>
          </tr>
          <tr *ngIf="transacoes.length === 0">
            <td colspan="4" class="empty-state">Nenhuma transação encontrada.</td>
          </tr>
        </tbody>
      </table>
    </app-card>
  `,
  styles: [`
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

    .text-right { text-align: right; }
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
export class DashboardRecentTransactionsComponent {
  @Input() transacoes: Transacao[] = [];
}
