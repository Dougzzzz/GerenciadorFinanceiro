import { Component, Input, Output, EventEmitter, signal, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { CardComponent } from '../../shared/components/card/card.component';
import { ButtonComponent } from '../../shared/components/button/button.component';
import { Categoria, ContaBancaria, CartaoCredito } from '../../core/models/financeiro.model';
import { FinanceiroService } from '../../core/services/financeiro.service';
import { TransacaoPreview } from '../../core/models/importacao.model';

export interface ImportConfig {
  categoriaId: string;
  contaId?: string;
  cartaoId?: string;
}

@Component({
  selector: 'app-transacoes-import',
  standalone: true,
  imports: [CommonModule, CardComponent, ButtonComponent, FormsModule],
  template: `
    <app-card title="Importar Extrato Financeiro">
      <div class="import-container">
        <!-- Passo 1: Seleção de Arquivo e Destino -->
        <div class="setup-section" *ngIf="!previewItems().length">
          <div class="upload-zone" [class.has-file]="!!selectedFile">
            <label for="fileInput" class="upload-label">
              <span class="icon">📁</span>
              <span class="text">{{ selectedFile ? selectedFile.name : 'Selecione ou arraste um arquivo (CSV ou Excel)' }}</span>
              <small *ngIf="selectedFile">Clique para trocar de arquivo</small>
            </label>
            <input type="file" (change)="onFileChange($event)" accept=".csv, .xls, .xlsx" id="fileInput" hidden>
          </div>

          <div class="destination-selectors">
            <div class="group">
              <span class="label-fake">Onde deseja importar?</span>
              <div class="radio-cards">
                <div class="radio-card" [class.active]="destino === 'conta'" 
                     (click)="setDestino('conta')" (keydown.enter)="setDestino('conta')" tabindex="0" role="button">
                  <span class="icon">🏦</span>
                  <span class="title">Conta Bancária</span>
                </div>
                <div class="radio-card" [class.active]="destino === 'cartao'" 
                     (click)="setDestino('cartao')" (keydown.enter)="setDestino('cartao')" tabindex="0" role="button">
                  <span class="icon">💳</span>
                  <span class="title">Cartão de Crédito</span>
                </div>
              </div>
            </div>

            <div class="group" *ngIf="destino === 'conta'">
              <label for="contaId">Selecione a Conta</label>
              <select id="contaId" [(ngModel)]="config.contaId" name="contaId">
                <option [value]="undefined" disabled>Escolha um banco...</option>
                <option *ngFor="let c of contas" [value]="c.id">{{ c.nomeBanco }}</option>
              </select>
            </div>

            <div class="group" *ngIf="destino === 'cartao'">
              <label for="cartaoId">Selecione o Cartão</label>
              <select id="cartaoId" [(ngModel)]="config.cartaoId" name="cartaoId">
                <option [value]="undefined" disabled>Escolha um cartão...</option>
                <option *ngFor="let c of cartoes" [value]="c.id">{{ c.nome }}</option>
              </select>
            </div>
          </div>

          <div class="actions">
            <app-button variant="outline" (clicked)="canceled.emit()">Cancelar</app-button>
            <app-button [disabled]="!canProcess()" (clicked)="gerarPreview()">
              Verificar Transações
            </app-button>
          </div>
        </div>

        <!-- Passo 2: Preview das Transações -->
        <div class="preview-section" *ngIf="previewItems().length > 0">
          <div class="preview-header">
            <h3>Preview: {{ previewItems().length }} transações encontradas</h3>
            <p>Destino: <strong>{{ getDestinoNome() }}</strong></p>
          </div>

          <div class="table-container">
            <table class="preview-table">
              <thead>
                <tr>
                  <th>Data</th>
                  <th>Descrição</th>
                  <th>Categoria</th>
                  <th class="text-right">Valor</th>
                </tr>
              </thead>
              <tbody>
                <tr *ngFor="let item of previewItems()">
                  <td>{{ item.data | date:'dd/MM' }}</td>
                  <td>
                    <div class="desc-cell">
                      <span>{{ item.descricao }}</span>
                      <small class="original-cat" *ngIf="item.categoriaOriginalCsv">CSV: {{ item.categoriaOriginalCsv }}</small>
                    </div>
                  </td>
                  <td>
                    <select [(ngModel)]="item.categoriaEscolhidaId" class="cat-select">
                      <option [value]="null">Criar nova: "{{ item.categoriaOriginalCsv || item.descricao }}"</option>
                      <optgroup label="Sugestões" *ngIf="item.categoriasSugeridas.length">
                        <option *ngFor="let s of item.categoriasSugeridas" [value]="s.categoriaId">
                          {{ s.nomeCategoria }} ({{ s.similaridadeFormatada }})
                        </option>
                      </optgroup>
                      <optgroup label="Todas as Categorias">
                        <option *ngFor="let cat of categorias" [value]="cat.id">{{ cat.nome }}</option>
                      </optgroup>
                    </select>
                  </td>
                  <td class="text-right" [class.negativo]="item.valor < 0" [class.positivo]="item.valor > 0">
                    {{ item.valor | currency:'BRL' }}
                  </td>
                </tr>
              </tbody>
            </table>
          </div>

          <div class="actions">
            <app-button variant="outline" (clicked)="limparPreview()">Voltar</app-button>
            <app-button (clicked)="confirmarImportacao()">
              Confirmar e Salvar
            </app-button>
          </div>
        </div>
      </div>
    </app-card>
  `,
  styles: [`
    .import-container { display: flex; flex-direction: column; gap: var(--spacing-lg); }
    
    .upload-zone { border: 2px dashed #cbd5e1; border-radius: var(--border-radius); padding: var(--spacing-xl); text-align: center; transition: all 0.2s; }
    .upload-zone.has-file { border-color: var(--color-primary); background: #f0f7ff; }
    .upload-label { cursor: pointer; display: flex; flex-direction: column; align-items: center; gap: 8px; }
    .upload-label .icon { font-size: 2rem; }
    .upload-label .text { font-weight: 500; color: var(--color-text-primary); }
    
    .destination-selectors { display: flex; flex-direction: column; gap: var(--spacing-md); margin-top: var(--spacing-md); }
    .radio-cards { display: grid; grid-template-columns: 1fr 1fr; gap: var(--spacing-md); }
    .radio-card { border: 1px solid #e2e8f0; padding: var(--spacing-md); border-radius: var(--border-radius); cursor: pointer; display: flex; align-items: center; gap: 12px; transition: all 0.2s; }
    .radio-card:hover { border-color: var(--color-primary); background: #f8fafc; }
    .radio-card.active { border-color: var(--color-primary); background: #f0f7ff; box-shadow: 0 0 0 1px var(--color-primary); }
    .radio-card .icon { font-size: 1.2rem; }
    
    .group { display: flex; flex-direction: column; gap: 4px; }
    .group label, .label-fake { font-size: 0.875rem; font-weight: 600; color: var(--color-sidebar); }
    select { padding: 10px; border-radius: var(--border-radius); border: 1px solid #e2e8f0; }

    .table-container { max-height: 500px; overflow-y: auto; border: 1px solid #e2e8f0; border-radius: var(--border-radius); margin: var(--spacing-md) 0; }
    .preview-table { width: 100%; border-collapse: collapse; font-size: 0.875rem; }
    .preview-table th { background: #f8fafc; position: sticky; top: 0; padding: 12px; text-align: left; border-bottom: 2px solid #e2e8f0; z-index: 10; }
    .preview-table td { padding: 10px 12px; border-bottom: 1px solid #f1f5f9; vertical-align: middle; }
    
    .desc-cell { display: flex; flex-direction: column; }
    .original-cat { font-size: 0.7rem; color: #94a3b8; }
    .cat-select { width: 100%; padding: 4px; border-radius: 4px; border: 1px solid #cbd5e1; font-size: 0.8rem; }

    .text-right { text-align: right; }
    .negativo { color: #f43f5e; font-weight: 600; }
    .positivo { color: #10b981; font-weight: 600; }

    .actions { display: flex; justify-content: flex-end; gap: var(--spacing-md); margin-top: var(--spacing-md); }
  `]
})
export class TransacoesImportComponent {
  private financeiroService = inject(FinanceiroService);

  @Input() categorias: Categoria[] = [];
  @Input() contas: ContaBancaria[] = [];
  @Input() cartoes: CartaoCredito[] = [];
  
  @Output() imported = new EventEmitter<void>();
  @Output() canceled = new EventEmitter<void>();

  selectedFile: File | null = null;
  destino: 'conta' | 'cartao' | null = null;
  config: ImportConfig = { categoriaId: '', contaId: undefined, cartaoId: undefined };
  
  previewItems = signal<TransacaoPreview[]>([]);

  onFileChange(event: Event) { 
    const input = event.target as HTMLInputElement;
    if (input.files && input.files.length > 0) {
      this.selectedFile = input.files[0]; 
      this.previewItems.set([]); 
    }
  }

  setDestino(d: 'conta' | 'cartao') {
    this.destino = d;
    this.config.contaId = undefined;
    this.config.cartaoId = undefined;
  }

  canProcess(): boolean {
    const hasFile = !!this.selectedFile;
    const hasDestino = (this.destino === 'conta' && !!this.config.contaId) || 
                       (this.destino === 'cartao' && !!this.config.cartaoId);
    return hasFile && hasDestino;
  }

  gerarPreview() {
    if (!this.selectedFile) return;

    this.financeiroService.getPreviewImportacao(this.selectedFile, this.config.contaId, this.config.cartaoId)
      .subscribe({
        next: (res) => {
          this.previewItems.set(res.transacoes);
        },
        error: (err: Error) => alert('Erro ao gerar preview: ' + err.message)
      });
  }

  limparPreview() {
    this.previewItems.set([]);
  }

  confirmarImportacao() {
    if (!this.selectedFile) return;

    this.financeiroService.confirmarImportacao(this.previewItems(), this.config.contaId, this.config.cartaoId)
      .subscribe({
        next: (res) => {
          if (res.sucesso) {
            let msg = `Sucesso! ${res.totalImportado} transações importadas.`;
            if (res.totalIgnorado > 0) {
              msg += `\n${res.totalIgnorado} transações duplicadas foram ignoradas.`;
            }
            alert(msg);
            this.imported.emit();
          } else {
            alert('Erro: ' + res.mensagemErro);
          }
        },
        error: (err: Error) => alert('Erro técnico: ' + err.message)
      });
  }

  getDestinoNome(): string {
    if (this.destino === 'conta') {
      return this.contas.find(c => c.id === this.config.contaId)?.nomeBanco || 'Conta selecionada';
    }
    return this.cartoes.find(c => c.id === this.config.cartaoId)?.nome || 'Cartão selecionado';
  }
}
