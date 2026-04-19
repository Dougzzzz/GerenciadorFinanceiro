import { Component, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FinanceiroService } from '../../core/services/financeiro.service';
import { Categoria, TipoTransacao } from '../../core/models/financeiro.model';
import { ButtonComponent } from '../../shared/components/button/button.component';
import { CategoriasFormComponent } from './categorias-form.component';
import { CategoriasListComponent } from './categorias-list.component';

@Component({
  selector: 'app-categorias',
  standalone: true,
  imports: [CommonModule, ButtonComponent, CategoriasFormComponent, CategoriasListComponent],
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
        <app-categorias-form
          [editando]="!!editando()"
          [novo]="novo"
          (onSalvar)="salvar()"
          (onLimpar)="limpar()">
        </app-categorias-form>

        <app-categorias-list
          [categorias]="categorias()"
          [selecionadas]="selecionadas()"
          (onIniciarEdicao)="iniciarEdicao($event)"
          (onExcluirUma)="excluirUma($event)"
          (onToggleSelecionada)="toggleSelecionada($event)"
          (onToggleTodas)="toggleTodas($event)">
        </app-categorias-list>
      </div>
    </div>
  `,
  styles: [`
    .page-header { display: flex; justify-content: space-between; align-items: center; margin-bottom: var(--spacing-md); }
    .grid { display: grid; grid-template-columns: 1fr 1.5fr; gap: var(--spacing-lg); margin-top: var(--spacing-md); }
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
    if (s.has(id)) s.delete(id); else s.add(id);
    this.selecionadas.set(s);
  }

  toggleTodas(ids: string[]) {
    if (ids.length === 0) {
      this.selecionadas.set(new Set());
    } else {
      this.selecionadas.set(new Set(ids));
    }
  }

  excluirUma(c: Categoria) {
    if (!confirm(`Deseja excluir a categoria '${c.nome}'?`)) return;
    this.service.excluirCategorias([c.id]).subscribe({
      next: () => { this.carregar(); alert('Categoria excluída!'); },
      error: (err) => alert(err.error || 'Erro ao excluir: ' + err.message)
    });
  }

  excluirSelecionadas() {
    if (!confirm(`Deseja excluir ${this.selecionadas().size} categorias?`)) return;
    const ids = Array.from(this.selecionadas());
    this.service.excluirCategorias(ids).subscribe({
      next: () => { this.selecionadas.set(new Set()); this.carregar(); alert('Categorias excluídas!'); },
      error: (err) => alert(err.error || 'Erro ao excluir: ' + err.message)
    });
  }

  salvar() {
    const obs = this.editando() 
      ? this.service.atualizarCategoria({ ...this.editando()!, ...this.novo })
      : this.service.criarCategoria(this.novo.nome, this.novo.tipo);

    obs.subscribe({
      next: () => { this.limpar(); this.carregar(); },
      error: (err) => alert('Erro ao salvar: ' + (err.error?.message || err.message))
    });
  }
}
