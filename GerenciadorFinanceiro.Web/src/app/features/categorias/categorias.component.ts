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
      <header class="page-header">
        <h1>Categorias</h1>
        <div class="actions">
          <app-button *ngIf="selecionadas().size > 0" variant="ghost" (onClick)="excluirSelecionadas()">
            Excluir ({{ selecionadas().size }})
          </app-button>
        </div>
      </header>
      
      <div class="grid">
        <app-card [title]="editando() ? 'Editar Categoria' : 'Nova Categoria'">
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
            <div class="form-actions">
              <app-button *ngIf="editando()" variant="outline" (onClick)="limpar()">Cancelar</app-button>
              <app-button type="submit" [disabled]="!novo.nome">
                {{ editando() ? 'Atualizar' : 'Salvar' }}
              </app-button>
            </div>
          </form>
        </app-card>

        <app-card title="Categorias Cadastradas">
          <div class="list-header" *ngIf="categorias().length > 0">
            <input type="checkbox" (change)="toggleTodas($event)">
            <small>Selecionar Todas</small>
          </div>
          <ul class="list">
            <li *ngFor="let c of categorias()" class="item">
              <div class="item-info">
                <input type="checkbox" [checked]="selecionadas().has(c.id)" (change)="toggleSelecionada(c.id)">
                <span>{{ c.nome }}</span>
                <span class="badge" [class.receita]="c.tipo === 0">{{ c.tipo === 0 ? 'Receita' : 'Despesa' }}</span>
              </div>
              <div class="item-actions">
                <button (click)="iniciarEdicao(c)" class="btn-icon">✏️</button>
                <button (click)="excluirUma(c)" class="btn-icon danger">🗑️</button>
              </div>
            </li>
          </ul>
        </app-card>
      </div>
    </div>
  `,
  styles: [`
    .page-header { display: flex; justify-content: space-between; align-items: center; margin-bottom: var(--spacing-md); }
    .grid { display: grid; grid-template-columns: 1fr 1.5fr; gap: var(--spacing-lg); margin-top: var(--spacing-md); }
    .form { display: flex; flex-direction: column; gap: var(--spacing-md); }
    .form-group { display: flex; flex-direction: column; gap: 4px; }
    input[type="text"], select { padding: 8px; border: 1px solid #e2e8f0; border-radius: var(--border-radius); }
    .list { display: flex; flex-direction: column; gap: 8px; }
    .list-header { display: flex; align-items: center; gap: 8px; margin-bottom: 12px; padding-bottom: 8px; border-bottom: 1px solid #f1f5f9; }
    .item { display: flex; justify-content: space-between; align-items: center; padding: 8px; border-bottom: 1px solid #f1f5f9; }
    .item-info { display: flex; align-items: center; gap: var(--spacing-md); }
    .badge { font-size: 0.75rem; padding: 2px 8px; border-radius: 12px; background: #fee2e2; color: #ef4444; }
    .badge.receita { background: #dcfce7; color: #10b981; }
    .form-actions { display: flex; justify-content: flex-end; gap: var(--spacing-md); }
    .item-actions { display: flex; gap: 8px; }
    .btn-icon { background: none; border: none; cursor: pointer; font-size: 1.1rem; padding: 4px; border-radius: 4px; transition: background 0.2s; }
    .btn-icon:hover { background: #f1f5f9; }
    .btn-icon.danger:hover { background: #fee2e2; }
  `]
})
export class CategoriasComponent implements OnInit {
  categorias = signal<Categoria[]>([]);
  selecionadas = signal<Set<string>>(new Set());
  editando = signal<Categoria | null>(null);
  novo = { nome: '', tipo: TipoTransacao.Despesa };

  constructor(private service: FinanceiroService) {}

  ngOnInit() { this.carregar(); }

  carregar() { 
    this.service.getCategorias().subscribe(data => {
      this.categorias.set(data);
      // Limpa seleção se itens não existirem mais
      const ids = new Set(data.map(c => c.id));
      const s = new Set(this.selecionadas());
      s.forEach(id => { if (!ids.has(id)) s.delete(id); });
      this.selecionadas.set(s);
    }); 
  }

  iniciarEdicao(c: Categoria) {
    this.editando.set(c);
    this.novo = { ...c };
  }

  limpar() {
    this.editando.set(null);
    this.novo = { nome: '', tipo: TipoTransacao.Despesa };
  }

  toggleSelecionada(id: string) {
    const s = new Set(this.selecionadas());
    if (s.has(id)) s.delete(id);
    else s.add(id);
    this.selecionadas.set(s);
  }

  toggleTodas(event: any) {
    if (event.target.checked) {
      this.selecionadas.set(new Set(this.categorias().map(c => c.id)));
    } else {
      this.selecionadas.set(new Set());
    }
  }

  excluirUma(c: Categoria) {
    if (!confirm(`Deseja excluir a categoria '${c.nome}'?`)) return;
    this.service.excluirCategorias([c.id]).subscribe({
      next: () => {
        this.carregar();
        alert('Categoria excluída!');
      },
      error: (err) => alert(err.error || 'Erro ao excluir: ' + err.message)
    });
  }

  excluirSelecionadas() {
    if (!confirm(`Deseja excluir ${this.selecionadas().size} categorias?`)) return;
    const ids = Array.from(this.selecionadas());
    this.service.excluirCategorias(ids).subscribe({
      next: () => {
        this.selecionadas.set(new Set());
        this.carregar();
        alert('Categorias excluídas com sucesso!');
      },
      error: (err) => alert(err.error || 'Erro ao excluir: ' + err.message)
    });
  }

  salvar() {
    const obs = this.editando() 
      ? this.service.atualizarCategoria({ ...this.editando()!, ...this.novo })
      : this.service.criarCategoria(this.novo.nome, this.novo.tipo);

    obs.subscribe({
      next: () => {
        this.limpar();
        this.carregar();
      },
      error: (err) => alert('Erro ao salvar: ' + (err.error?.message || err.message))
    });
  }
}
