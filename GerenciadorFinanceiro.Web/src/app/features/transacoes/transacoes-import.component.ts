import { Component, Input, Output, EventEmitter, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { CardComponent } from '../../shared/components/card/card.component';
import { ButtonComponent } from '../../shared/components/button/button.component';
import { Categoria, ContaBancaria, CartaoCredito, Transacao } from '../../core/models/financeiro.model';

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
              <span class="text">{{ selectedFile ? selectedFile.name : 'Selecione ou arraste um arquivo CSV' }}</span>
              <small *ngIf="selectedFile">Clique para trocar de arquivo</small>
            </label>
            <input type="file" (change)="onFileChange($event)" accept=".csv" id="fileInput" hidden>
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
                  <th class="text-right">Valor</th>
                </tr>
              </thead>
              <tbody>
                <tr *ngFor="let item of previewItems()">
                  <td>{{ item.data }}</td>
                  <td>{{ item.descricao }}</td>
                  <td class="text-right" [class.negativo]="item.valor && item.valor < 0" [class.positivo]="item.valor && item.valor > 0">
                    {{ item.valor | currency:'BRL' }}
                  </td>
                </tr>
              </tbody>
            </table>
          </div>

          <div class="actions">
            <app-button variant="outline" (clicked)="limparPreview()">Voltar</app-button>
            <app-button (clicked)="confirmarImportacao()">
              Confirmar e Salvar no Banco
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

    .table-container { max-height: 400px; overflow-y: auto; border: 1px solid #e2e8f0; border-radius: var(--border-radius); margin: var(--spacing-md) 0; }
    .preview-table { width: 100%; border-collapse: collapse; font-size: 0.875rem; }
    .preview-table th { background: #f8fafc; position: sticky; top: 0; padding: 12px; text-align: left; border-bottom: 2px solid #e2e8f0; }
    .preview-table td { padding: 10px 12px; border-bottom: 1px solid #f1f5f9; }
    .text-right { text-align: right; }
    .negativo { color: #f43f5e; font-weight: 600; }
    .positivo { color: #10b981; font-weight: 600; }

    .actions { display: flex; justify-content: flex-end; gap: var(--spacing-md); margin-top: var(--spacing-md); }
  `]
})
export class TransacoesImportComponent {
  @Input() categorias: Categoria[] = [];
  @Input() contas: ContaBancaria[] = [];
  @Input() cartoes: CartaoCredito[] = [];
  @Output() imported = new EventEmitter<{file: File, config: ImportConfig}>();
  @Output() canceled = new EventEmitter<void>();

  selectedFile: File | null = null;
  destino: 'conta' | 'cartao' | null = null;
  config: ImportConfig = { categoriaId: '', contaId: undefined, cartaoId: undefined };
  
  previewItems = signal<Partial<Transacao>[]>([]);

  onFileChange(event: Event) { 
    const input = event.target as HTMLInputElement;
    if (input.files && input.files.length > 0) {
      this.selectedFile = input.files[0]; 
      this.previewItems.set([]); // Reset preview se mudar o arquivo
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

    const reader = new FileReader();
    reader.onload = (e: ProgressEvent<FileReader>) => {
      const text = e.target?.result as string;
      if (!text) return;

      const lines = text.split('\n').slice(1); // Pular cabeçalho
      const items = lines
        .filter((line: string) => line.trim().length > 0)
        .map((line: string) => {
          const parts = line.split(','); // Assumindo CSV simples para o preview
          return {
            data: parts[0] || 'N/A',
            descricao: parts[1] || 'Sem descrição',
            valor: parseFloat(parts[2]) || 0
          } as Partial<Transacao>;
        })
        .slice(0, 10); // Mostrar apenas as primeiras 10 para preview
      
      this.previewItems.set(items);
    };
    reader.readAsText(this.selectedFile);
  }

  limparPreview() {
    this.previewItems.set([]);
  }

  confirmarImportacao() {
    if (this.selectedFile) {
      this.imported.emit({ file: this.selectedFile, config: this.config });
    }
  }

  getDestinoNome(): string {
    if (this.destino === 'conta') {
      return this.contas.find(c => c.id === this.config.contaId)?.nomeBanco || 'Conta selecionada';
    }
    return this.cartoes.find(c => c.id === this.config.cartaoId)?.nome || 'Cartão selecionado';
  }
}
