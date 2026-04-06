import { Component, Input } from '@angular/core';
import { CommonModule } from '@angular/common';
import { CartaoCredito } from '../../core/models/financeiro.model';
import { CardComponent } from '../../shared/components/card/card.component';

@Component({
  selector: 'app-cartoes-list',
  standalone: true,
  imports: [CommonModule, CardComponent],
  template: `
    <app-card title="Cartões Cadastrados">
      <ul class="list">
        <li *ngFor="let c of cartoes" class="item">
          <div class="info">
            <strong>{{ c.nome }}</strong>
            <small>Vencimento: dia {{ c.diaVencimento }}</small>
          </div>
          <span class="value">{{ c.limite | currency:'BRL' }}</span>
        </li>
      </ul>
    </app-card>
  `,
  styles: [`
    .list { display: flex; flex-direction: column; gap: 8px; }
    .item { display: flex; justify-content: space-between; align-items: center; padding: 12px; border-bottom: 1px solid #f1f5f9; }
    .info { display: flex; flex-direction: column; }
    .info small { color: var(--color-text-secondary); }
    .value { font-weight: 600; }
  `]
})
export class CartoesListComponent {
  @Input() cartoes: CartaoCredito[] = [];
}
