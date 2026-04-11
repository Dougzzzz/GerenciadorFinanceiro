import { Component, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FinanceiroService } from '../../core/services/financeiro.service';
import { CartaoCredito } from '../../core/models/financeiro.model';
import { CartoesFormComponent } from './cartoes-form.component';
import { CartoesListComponent } from './cartoes-list.component';

@Component({
  selector: 'app-cartoes',
  standalone: true,
  imports: [CommonModule, CartoesFormComponent, CartoesListComponent],
  template: `
    <div class="cartoes">
      <h1>Cartões de Crédito</h1>
      
      <div class="grid">
        <app-cartoes-form (onSalvar)="salvar($event)"></app-cartoes-form>
        <app-cartoes-list [cartoes]="cartoes()"></app-cartoes-list>
      </div>
    </div>
  `,
  styles: [`
    .grid { display: grid; grid-template-columns: 1fr 1.5fr; gap: var(--spacing-lg); margin-top: var(--spacing-xl); }
  `]
})
export class CartoesComponent implements OnInit {
  cartoes = signal<CartaoCredito[]>([]);

  constructor(private service: FinanceiroService) {}

  ngOnInit() { this.carregar(); }

  carregar() { 
    this.service.getCartoes().subscribe(data => this.cartoes.set(data)); 
  }

  salvar(dados: any) {
    this.service.criarCartao(dados.nome, dados.limite, dados.diaFechamento, dados.diaVencimento, dados.provedor).subscribe(() => {
      this.carregar();
    });
  }
}
