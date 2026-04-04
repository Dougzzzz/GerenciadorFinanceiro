import { Component, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FinanceiroService } from '../../core/services/financeiro.service';
import { Categoria, TipoTransacao } from '../../core/models/financeiro.model';
import { CardComponent } from '../../shared/components/card/card.component';
import { ButtonComponent } from '../../shared/components/button/button.component';
import { FormsModule } from '@angular/forms';

@Component({
  selector: 'app-categorias',
  standalone: true,
  imports: [CommonModule, CardComponent, ButtonComponent, FormsModule],
  template: `
    <div class="categorias">
      <h1>Categorias</h1>
      
      <div class="grid">
        <app-card title="Nova Categoria">
          <form (submit)="salvar()" class="form">
            <div class="form-group">
              <label>Nome</label>
              <input type="text" [(ngModel)]="novo.nome" name="nome" required>
            </div>
            <div class="form-group">
              <label>Tipo</label>
              <select [(ngModel)]="novo.tipo" name="tipo">
                <option [value]="0">Receita</option>
                <option [value]="1">Despesa</option>
              </select>
            </div>
            <app-button type="submit" [disabled]="!novo.nome">Salvar</app-button>
          </form>
        </app-card>

        <app-card title="Categorias Cadastradas">
          <ul class="list">
            <li *ngFor="let c of categorias()" class="item">
              <span>{{ c.nome }}</span>
              <span class="badge" [class.receita]="c.tipo === 0">{{ c.tipo === 0 ? 'Receita' : 'Despesa' }}</span>
            </li>
          </ul>
        </app-card>
      </div>
    </div>
  `,
  styles: [`
    .grid { display: grid; grid-template-columns: 1fr 1.5fr; gap: var(--spacing-lg); margin-top: var(--spacing-xl); }
    .form { display: flex; flex-direction: column; gap: var(--spacing-md); }
    .form-group { display: flex; flex-direction: column; gap: 4px; }
    input, select { padding: 8px; border: 1px solid #e2e8f0; border-radius: var(--border-radius); }
    .list { display: flex; flex-direction: column; gap: 8px; }
    .item { display: flex; justify-content: space-between; padding: 8px; border-bottom: 1px solid #f1f5f9; }
    .badge { font-size: 0.75rem; padding: 2px 8px; border-radius: 12px; background: #fee2e2; color: #ef4444; }
    .badge.receita { background: #dcfce7; color: #10b981; }
  `]
})
export class CategoriasComponent implements OnInit {
  categorias = signal<Categoria[]>([]);
  novo = { nome: '', tipo: TipoTransacao.Despesa };

  constructor(private service: FinanceiroService) {}

  ngOnInit() { this.carregar(); }

  carregar() { this.service.getCategorias().subscribe(data => this.categorias.set(data)); }

  salvar() {
    this.service.criarCategoria(this.novo.nome, this.novo.tipo).subscribe(() => {
      this.novo = { nome: '', tipo: TipoTransacao.Despesa };
      this.carregar();
    });
  }
}
