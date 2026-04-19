import { Component, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FinanceiroService } from '../../core/services/financeiro.service';
import { MetaGasto, Categoria, TipoTransacao } from '../../core/models/financeiro.model';
import { ButtonComponent } from '../../shared/components/button/button.component';
import { MetasGastosFormComponent } from './metas-gastos-form.component';
import { MetasGastosListComponent } from './metas-gastos-list.component';
import { forkJoin } from 'rxjs';

@Component({
  selector: 'app-metas-gastos',
  standalone: true,
  imports: [CommonModule, ButtonComponent, MetasGastosFormComponent, MetasGastosListComponent],
  template: `
    <div class="metas-gastos">
      <header class="page-header">
        <h1>Metas de Gastos</h1>
        <div class="actions">
          <app-button *ngIf="selecionadas().size > 0" variant="ghost" (clicked)="excluirSelecionadas()">
            Excluir ({{ selecionadas().size }})
          </app-button>
        </div>
      </header>

      <div class="grid">
        <app-metas-gastos-form
          [categorias]="categoriasDespesa"
          [editando]="!!editando()"
          [novo]="novo"
          (saved)="onSave($event)"
          (cleared)="limpar()">
        </app-metas-gastos-form>

        <app-metas-gastos-list
          [metas]="metas()"
          [categorias]="categorias"
          [selecionadas]="selecionadas()"
          (editClicked)="iniciarEdicao($event)"
          (deleteClicked)="onDelete($event)"
          (selectionToggled)="toggleSelecionada($event)"
          (allSelectedToggled)="toggleTodas($event)">
        </app-metas-gastos-list>
      </div>
    </div>
  `,
  styles: [`
    .page-header { display: flex; justify-content: space-between; align-items: center; margin-bottom: var(--spacing-md); }
    .grid { display: grid; grid-template-columns: 1fr 1.5fr; gap: var(--spacing-lg); margin-top: var(--spacing-md); }
  `]
})
export class MetasGastosComponent implements OnInit {
  metas = signal<MetaGasto[]>([]);
  categorias: Categoria[] = [];
  categoriasDespesa: Categoria[] = [];
  selecionadas = signal<Set<string>>(new Set());
  
  editando = signal<MetaGasto | null>(null);
  novo: Partial<MetaGasto> = { categoriaId: '', valorLimite: 0 };

  constructor(private financeiroService: FinanceiroService) {}

  ngOnInit() {
    this.carregarDados();
  }

  carregarDados() {
    forkJoin({
      metas: this.financeiroService.getMetas(),
      categorias: this.financeiroService.getCategorias()
    }).subscribe(({ metas, categorias }) => {
      this.metas.set(metas);
      this.categorias = categorias;
      this.categoriasDespesa = categorias.filter(c => c.tipo === TipoTransacao.Despesa);
      
      const ids = new Set(metas.map(m => m.id));
      const s = new Set(this.selecionadas());
      s.forEach(id => { if (!ids.has(id)) s.delete(id); });
      this.selecionadas.set(s);
    });
  }

  iniciarEdicao(meta: MetaGasto) {
    this.editando.set(meta);
    this.novo = { ...meta };
  }

  limpar() {
    this.editando.set(null);
    this.novo = { categoriaId: '', valorLimite: 0, mes: undefined, ano: undefined };
  }

  onSave(meta: Partial<MetaGasto>) {
    const obs = this.editando()
      ? this.financeiroService.atualizarMeta(this.editando()!.id, meta.valorLimite!)
      : this.financeiroService.criarMeta(meta);

    obs.subscribe({
      next: () => {
        this.limpar();
        this.carregarDados();
      },
      error: (err: Error) => alert('Erro ao salvar meta: ' + err.message)
    });
  }

  onDelete(meta: MetaGasto) {
    if (!confirm(`Deseja realmente excluir a meta da categoria '${this.getCategoriaNome(meta.categoriaId)}'?`)) return;
    this.financeiroService.excluirMeta(meta.id).subscribe(() => {
      this.carregarDados();
    });
  }

  excluirSelecionadas() {
    if (!confirm(`Deseja excluir ${this.selecionadas().size} metas?`)) return;
    const ids = Array.from(this.selecionadas());
    forkJoin(ids.map(id => this.financeiroService.excluirMeta(id))).subscribe(() => {
      this.selecionadas.set(new Set());
      this.carregarDados();
    });
  }

  toggleSelecionada(id: string) {
    const s = new Set(this.selecionadas());
    if (s.has(id)) s.delete(id); else s.add(id);
    this.selecionadas.set(s);
  }

  toggleTodas(event: Event) {
    const checkbox = event.target as HTMLInputElement;
    this.selecionadas.set(checkbox.checked ? new Set(this.metas().map(m => m.id)) : new Set());
  }

  private getCategoriaNome(id: string): string {
    return this.categorias.find(c => c.id === id)?.nome || 'Desconhecida';
  }
}
