import { Component, Input } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ContaBancaria } from '../../core/models/financeiro.model';
import { CardComponent } from '../../shared/components/card/card.component';

@Component({
  selector: 'app-contas-list',
  standalone: true,
  imports: [CommonModule, CardComponent],
  template: `
    <app-card title="Contas Cadastradas">
      <ul class="list">
        <li *ngFor="let c of contas" class="item">
          <div class="info">
            <span>{{ c.nomeBanco }}</span>
            <small class="badge" *ngIf="c.provedor > 0">
              {{ c.provedor === 1 ? 'C6 Bank' : c.provedor === 2 ? 'Nubank' : 'Inter' }}
            </small>
          </div>
          <span class="value" [class.negative]="c.saldoAtual < 0">{{ c.saldoAtual | currency:'BRL' }}</span>
        </li>
      </ul>
    </app-card>
  `,
  styles: [`
    .list { display: flex; flex-direction: column; gap: 8px; }
    .item { display: flex; justify-content: space-between; align-items: center; padding: 8px; border-bottom: 1px solid #f1f5f9; }
    .info { display: flex; flex-direction: column; gap: 2px; }
    .badge { font-size: 10px; color: #64748b; background: #f1f5f9; padding: 2px 6px; border-radius: 4px; width: fit-content; }
    .value { font-weight: 600; color: var(--color-income); }
    .value.negative { color: var(--color-expense); }
  `]
})
export class ContasListComponent {
  @Input() contas: ContaBancaria[] = [];
}
