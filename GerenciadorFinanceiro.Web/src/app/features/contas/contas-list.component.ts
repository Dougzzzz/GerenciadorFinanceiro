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
          <span>{{ c.nomeBanco }}</span>
          <span class="value" [class.negative]="c.saldoAtual < 0">{{ c.saldoAtual | currency:'BRL' }}</span>
        </li>
      </ul>
    </app-card>
  `,
  styles: [`
    .list { display: flex; flex-direction: column; gap: 8px; }
    .item { display: flex; justify-content: space-between; padding: 8px; border-bottom: 1px solid #f1f5f9; }
    .value { font-weight: 600; color: var(--color-income); }
    .value.negative { color: var(--color-expense); }
  `]
})
export class ContasListComponent {
  @Input() contas: ContaBancaria[] = [];
}
