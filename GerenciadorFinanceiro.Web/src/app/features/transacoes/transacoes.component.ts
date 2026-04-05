import { Component, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FinanceiroService } from '../../core/services/financeiro.service';
import { Transacao, Categoria, ContaBancaria, CartaoCredito } from '../../core/models/financeiro.model';
import { CardComponent } from '../../shared/components/card/card.component';
import { ButtonComponent } from '../../shared/components/button/button.component';
import { FormsModule } from '@angular/forms';

@Component({
  selector: 'app-transacoes',
  standalone: true,
  imports: [CommonModule, CardComponent, ButtonComponent, FormsModule],
  template: `
    <div class="transacoes">
      <header class="page-header">
        <h1>Transações</h1>
        <div class="actions">
          <app-button *ngIf="selecionadas().size > 0" variant="ghost" (onClick)="excluirSelecionadas()">
            Excluir ({{ selecionadas().size }})
          </app-button>
          <app-button variant="outline" (onClick)="mostrarNovo.set(!mostrarNovo())">
            {{ mostrarNovo() ? 'Fechar' : 'Nova Transação' }}
          </app-button>
          <app-button (onClick)="mostrarImportacao.set(!mostrarImportacao())">
            {{ mostrarImportacao() ? 'Fechar' : 'Importar Extrato' }}
          </app-button>
        </div>
      </header>

      <!-- Formulário de Nova Transação -->
      <app-card *ngIf="mostrarNovo()" title="Nova Transação">
        <form (submit)="salvarNovaTransacao()" class="form-grid">
          <div class="form-group">
            <label>Data</label>
            <input type="date" [(ngModel)]="novaTransacao.data" name="data" required>
          </div>
          <div class="form-group">
            <label>Descrição</label>
            <input type="text" [(ngModel)]="novaTransacao.descricao" name="descricao" placeholder="Ex: Almoço" required>
          </div>
          <div class="form-group">
            <label>Valor (R$)</label>
            <input type="number" step="0.01" [(ngModel)]="novaTransacao.valor" name="valor" placeholder="Negativo para despesas" required>
          </div>
          <div class="form-group">
            <label>Categoria</label>
            <select [(ngModel)]="novaTransacao.categoriaId" name="categoriaId" required>
              <option *ngFor="let c of categorias()" [value]="c.id">{{ c.nome }}</option>
            </select>
          </div>
          <div class="form-group">
            <label>Conta (Opcional)</label>
            <select [(ngModel)]="novaTransacao.contaBancariaId" name="contaBancariaId">
              <option [value]="undefined">Nenhuma</option>
              <option *ngFor="let c of contas()" [value]="c.id">{{ c.nomeBanco }}</option>
            </select>
          </div>
          <div class="form-group">
            <label>Cartão (Opcional)</label>
            <select [(ngModel)]="novaTransacao.cartaoCreditoId" name="cartaoCreditoId">
              <option [value]="undefined">Nenhum</option>
              <option *ngFor="let c of cartoes()" [value]="c.id">{{ c.nome }}</option>
            </select>
          </div>
          <div class="form-actions-full">
            <app-button type="submit" [disabled]="!novaTransacao.descricao || !novaTransacao.valor || !novaTransacao.categoriaId">
              Salvar Transação
            </app-button>
          </div>
        </form>
      </app-card>

      <!-- Formulário de Importação -->
      <app-card *ngIf="mostrarImportacao()" title="Importar Extrato CSV">
        <div class="import-form">
          <div class="form-group">
            <label>Arquivo CSV</label>
            <input type="file" (change)="onFileSelected($event)" accept=".csv">
          </div>
          
          <div class="form-row">
            <div class="form-group">
              <label>Conta Bancária</label>
              <select [(ngModel)]="importConfig.contaId" (change)="importConfig.cartaoId = undefined">
                <option [value]="undefined">Nenhuma</option>
                <option *ngFor="let c of contas()" [value]="c.id">{{ c.nomeBanco }}</option>
              </select>
            </div>

            <div class="form-group">
              <label>ou Cartão de Crédito</label>
              <select [(ngModel)]="importConfig.cartaoId" (change)="importConfig.contaId = undefined">
                <option [value]="undefined">Nenhum</option>
                <option *ngFor="let c of cartoes()" [value]="c.id">{{ c.nome }}</option>
              </select>
            </div>

            <div class="form-group">
              <label>Categoria Padrão (Opcional)</label>
              <select [(ngModel)]="importConfig.categoriaId">
                <option [value]="''">Nenhuma (Usar 'Outros')</option>
                <option *ngFor="let c of categorias()" [value]="c.id">{{ c.nome }}</option>
              </select>
            </div>
          </div>

          <div class="form-actions">
            <app-button variant="outline" (onClick)="mostrarImportacao.set(false)">Cancelar</app-button>
            <app-button [disabled]="!selectedFile || (!importConfig.contaId && !importConfig.cartaoId)" (onClick)="importar()">
              Processar Arquivo
            </app-button>
          </div>
        </div>
      </app-card>

      <app-card title="Histórico de Lançamentos">
        <table class="table">
          <thead>
            <tr>
              <th width="40"><input type="checkbox" (change)="toggleTodas($event)"></th>
              <th>Data</th>
              <th>Descrição</th>
              <th>Categoria</th>
              <th>Cartão/Conta</th>
              <th class="text-right">Valor</th>
              <th width="80">Ações</th>
            </tr>
          </thead>
          <tbody>
            <tr *ngFor="let t of transacoes()">
              <!-- Seleção -->
              <td>
                <input type="checkbox" [checked]="selecionadas().has(t.id)" (change)="toggleSelecionada(t.id)">
              </td>

              <!-- Data -->
              <td>
                <input *ngIf="editandoId() === t.id" type="date" [(ngModel)]="tempEdit().data" class="edit-input">
                <span *ngIf="editandoId() !== t.id">{{ t.data | date:'dd/MM/yyyy' }}</span>
              </td>

              <!-- Descrição -->
              <td>
                <div *ngIf="editandoId() === t.id" class="edit-group">
                  <input type="text" [(ngModel)]="tempEdit().descricao" class="edit-input">
                </div>
                <div *ngIf="editandoId() !== t.id" class="desc-cell">
                  <strong>{{ t.descricao }}</strong>
                  <small *ngIf="t.parcela">{{ t.parcela }}</small>
                </div>
              </td>

              <!-- Categoria -->
              <td>
                <select *ngIf="editandoId() === t.id" [(ngModel)]="tempEdit().categoriaId" class="edit-input">
                  <option *ngFor="let c of categorias()" [value]="c.id">{{ c.nome }}</option>
                </select>
                <span *ngIf="editandoId() !== t.id" class="badge">
                  {{ t.categoriaNavigation?.nome || t.categoria }}
                </span>
              </td>

              <!-- Destino -->
              <td>
                <div *ngIf="editandoId() === t.id" class="edit-group">
                  <select [(ngModel)]="tempEdit().contaBancariaId" class="edit-input">
                    <option [value]="undefined">Nenhuma</option>
                    <option *ngFor="let c of contas()" [value]="c.id">{{ c.nomeBanco }}</option>
                  </select>
                  <select [(ngModel)]="tempEdit().cartaoCreditoId" class="edit-input">
                    <option [value]="undefined">Nenhum</option>
                    <option *ngFor="let c of cartoes()" [value]="c.id">{{ c.nome }}</option>
                  </select>
                </div>
                <span *ngIf="editandoId() !== t.id">
                  {{ t.cartaoCreditoNavigation?.nome || t.contaBancariaNavigation?.nomeBanco || t.nomeCartao || 'Nenhum' }}
                </span>
              </td>

              <!-- Valor -->
              <td class="text-right">
                <input *ngIf="editandoId() === t.id" type="number" step="0.01" [(ngModel)]="tempEdit().valor" class="edit-input text-right">
                <span *ngIf="editandoId() !== t.id" [class.income]="t.valor > 0" [class.expense]="t.valor < 0">
                  {{ t.valor | currency:'BRL' }}
                </span>
              </td>

              <!-- Ações da Linha -->
              <td>
                <div class="row-actions">
                  <button *ngIf="editandoId() !== t.id" (click)="iniciarEdicao(t)" class="btn-icon">✏️</button>
                  <button *ngIf="editandoId() === t.id" (click)="salvarEdicao()" class="btn-icon success">✅</button>
                  <button *ngIf="editandoId() === t.id" (click)="cancelarEdicao()" class="btn-icon danger">❌</button>
                </div>
              </td>
            </tr>
          </tbody>
        </table>
      </app-card>
    </div>
  `,
  styles: [`
    .page-header { display: flex; justify-content: space-between; align-items: center; margin-bottom: var(--spacing-xl); }
    .import-form { display: flex; flex-direction: column; gap: var(--spacing-lg); }
    .form-row { display: grid; grid-template-columns: repeat(3, 1fr); gap: var(--spacing-md); }
    .form-group { display: flex; flex-direction: column; gap: var(--spacing-xs); }
    .form-group label { font-size: 0.875rem; font-weight: 500; color: var(--color-text-secondary); }
    select, input, .edit-input { padding: var(--spacing-sm); border-radius: var(--border-radius); border: 1px solid #e2e8f0; background: white; font-size: 0.875rem; }
    .edit-input { width: 100%; padding: 4px; }
    .form-actions { display: flex; justify-content: flex-end; gap: var(--spacing-md); margin-top: var(--spacing-md); }
    .table { width: 100%; border-collapse: collapse; }
    .table th { text-align: left; padding: var(--spacing-md); border-bottom: 2px solid #f1f5f9; }
    .table td { padding: var(--spacing-md); border-bottom: 1px solid #f1f5f9; vertical-align: middle; }
    .text-right { text-align: right; }
    .income { color: var(--color-income); font-weight: 600; }
    .expense { color: var(--color-expense); font-weight: 600; }
    .desc-cell { display: flex; flex-direction: column; }
    .desc-cell small { color: var(--color-text-secondary); font-size: 0.75rem; }
    .badge { background: #f1f5f9; padding: 2px 8px; border-radius: 12px; font-size: 0.75rem; }
    .form-grid { display: grid; grid-template-columns: repeat(auto-fit, minmax(200px, 1fr)); gap: var(--spacing-md); }
    .form-actions-full { grid-column: 1 / -1; display: flex; justify-content: flex-end; margin-top: var(--spacing-md); }
    .row-actions { display: flex; gap: 8px; }
    .btn-icon { background: none; border: none; cursor: pointer; font-size: 1.1rem; padding: 4px; border-radius: 4px; transition: background 0.2s; }
    .btn-icon:hover { background: #f1f5f9; }
    .btn-icon.success:hover { background: #dcfce7; }
    .btn-icon.danger:hover { background: #fee2e2; }
    .edit-group { display: flex; flex-direction: column; gap: 4px; }
  `]
})
export class TransacoesComponent implements OnInit {
  transacoes = signal<Transacao[]>([]);
  categorias = signal<Categoria[]>([]);
  contas = signal<ContaBancaria[]>([]);
  cartoes = signal<CartaoCredito[]>([]);
  
  selecionadas = signal<Set<string>>(new Set());
  mostrarImportacao = signal(false);
  mostrarNovo = signal(false);

  // Estado de edição
  editandoId = signal<string | null>(null);
  tempEdit = signal<any>(null);

  selectedFile: File | null = null;
  importConfig = {
    categoriaId: '',
    contaId: undefined as string | undefined,
    cartaoId: undefined as string | undefined
  };

  novaTransacao = this.getNovoObjetoTransacao();

  constructor(private financeiroService: FinanceiroService) {}

  ngOnInit(): void {
    this.carregarDados();
  }

  getNovoObjetoTransacao() {
    return {
      data: new Date().toISOString().split('T')[0],
      descricao: '',
      valor: 0,
      categoriaId: '',
      contaBancariaId: undefined as string | undefined,
      cartaoCreditoId: undefined as string | undefined
    };
  }

  carregarDados(): void {
    this.financeiroService.getTransacoes().subscribe(data => this.transacoes.set(data));
    this.financeiroService.getCategorias().subscribe(data => {
      this.categorias.set(data);
      if (data.length > 0 && !this.novaTransacao.categoriaId) {
        this.novaTransacao.categoriaId = data[0].id;
      }
    });
    this.financeiroService.getContas().subscribe(data => this.contas.set(data));
    this.financeiroService.getCartoes().subscribe(data => this.cartoes.set(data));
  }

  // Lógica de Edição
  iniciarEdicao(t: Transacao) {
    this.editandoId.set(t.id);
    // Cria uma cópia profunda para edição
    this.tempEdit.set({
      ...t,
      data: new Date(t.data).toISOString().split('T')[0] // Converte para input date
    });
  }

  cancelarEdicao() {
    this.editandoId.set(null);
    this.tempEdit.set(null);
  }

  salvarEdicao() {
    const editada = this.tempEdit();
    this.financeiroService.atualizarTransacao(editada).subscribe({
      next: () => {
        this.cancelarEdicao();
        this.carregarDados();
        alert('Transação atualizada!');
      },
      error: (err) => alert('Erro ao atualizar: ' + err.message)
    });
  }

  toggleSelecionada(id: string) {
    const s = new Set(this.selecionadas());
    if (s.has(id)) s.delete(id);
    else s.add(id);
    this.selecionadas.set(s);
  }

  toggleTodas(event: any) {
    if (event.target.checked) {
      this.selecionadas.set(new Set(this.transacoes().map(t => t.id)));
    } else {
      this.selecionadas.set(new Set());
    }
  }

  excluirSelecionadas() {
    if (!confirm(`Deseja excluir ${this.selecionadas().size} transações?`)) return;

    const ids = Array.from(this.selecionadas());
    this.financeiroService.excluirTransacoes(ids).subscribe({
      next: () => {
        this.selecionadas.set(new Set());
        this.carregarDados();
        alert('Excluídas com sucesso!');
      },
      error: (err) => alert('Erro ao excluir: ' + err.message)
    });
  }

  salvarNovaTransacao(): void {
    this.financeiroService.criarTransacao(this.novaTransacao).subscribe({
      next: () => {
        this.mostrarNovo.set(false);
        this.novaTransacao = this.getNovoObjetoTransacao();
        this.carregarDados();
        alert('Transação salva com sucesso!');
      },
      error: (err) => alert('Erro ao salvar: ' + err.message)
    });
  }

  onFileSelected(event: any): void {
    this.selectedFile = event.target.files[0];
  }

  importar(): void {
    if (!this.selectedFile) return;

    this.financeiroService.importarExtrato(
      this.selectedFile,
      this.importConfig.categoriaId,
      this.importConfig.contaId,
      this.importConfig.cartaoId
    ).subscribe({
      next: () => {
        this.mostrarImportacao.set(false);
        this.selectedFile = null;
        this.carregarDados();
        alert('Extrato importado com sucesso!');
      },
      error: (err) => alert('Erro na importação: ' + err.message)
    });
  }
}
