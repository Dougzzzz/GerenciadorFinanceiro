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
        <app-cartoes-form 
          [cartao]="cartaoParaEditar()" 
          (saved)="salvar($event)"
          (canceled)="cancelarEdicao()">
        </app-cartoes-form>
        <app-cartoes-list 
          [cartoes]="cartoes()" 
          (edit)="editar($event)" 
          (delete)="excluir($event)">
        </app-cartoes-list>
      </div>
    </div>
  `,
  styles: [`
    .grid { display: grid; grid-template-columns: 1fr 1.5fr; gap: var(--spacing-lg); margin-top: var(--spacing-xl); }
  `]
})
export class CartoesComponent implements OnInit {
  cartoes = signal<CartaoCredito[]>([]);
  cartaoParaEditar = signal<CartaoCredito | null>(null);

  constructor(private service: FinanceiroService) {}

  ngOnInit() { this.carregar(); }

  carregar() { 
    this.service.getCartoes().subscribe(data => this.cartoes.set(data)); 
  }

  editar(cartao: CartaoCredito) {
    this.cartaoParaEditar.set(cartao);
  }

  cancelarEdicao() {
    this.cartaoParaEditar.set(null);
  }

  excluir(id: string) {
    if (confirm('Deseja realmente excluir este cartão?')) {
      this.service.excluirCartao(id).subscribe(() => this.carregar());
    }
  }

  salvar(dados: Partial<CartaoCredito>) {
    const editando = this.cartaoParaEditar();
    
    if (editando) {
      this.service.atualizarCartao(editando.id, dados).subscribe(() => {
        this.cartaoParaEditar.set(null);
        this.carregar();
      });
    } else {
      this.service.criarCartao(dados).subscribe(() => {
        this.carregar();
      });
    }
  }
}
