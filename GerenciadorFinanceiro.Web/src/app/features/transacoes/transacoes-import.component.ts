import { Component, Input, Output, EventEmitter } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { CardComponent } from '../../shared/components/card/card.component';
import { ButtonComponent } from '../../shared/components/button/button.component';
import { Categoria, ContaBancaria, CartaoCredito } from '../../core/models/financeiro.model';

@Component({
  selector: 'app-transacoes-import',
  standalone: true,
  imports: [CommonModule, CardComponent, ButtonComponent, FormsModule],
  template: `
    <app-card title="Importar Extrato CSV">
      <div class="import-form">
        <div class="form-group">
          <label>Arquivo CSV</label>
          <input type="file" (change)="onFileChange($event)" accept=".csv">
        </div>
        
        <div class="form-row">
          <div class="form-group">
            <label>Conta Bancária</label>
            <select [(ngModel)]="config.contaId" (change)="config.cartaoId = undefined">
              <option [value]="undefined">Nenhuma</option>
              <option *ngFor="let c of contas" [value]="c.id">{{ c.nomeBanco }}</option>
            </select>
          </div>
          <div class="form-group">
            <label>Cartão de Crédito</label>
            <select [(ngModel)]="config.cartaoId" (change)="config.contaId = undefined">
              <option [value]="undefined">Nenhum</option>
              <option *ngFor="let c of cartoes" [value]="c.id">{{ c.nome }}</option>
            </select>
          </div>
          <div class="form-group">
            <label>Categoria Padrão (Opcional)</label>
            <select [(ngModel)]="config.categoriaId">
              <option [value]="''">Nenhuma (Usar 'Outros')</option>
              <option *ngFor="let c of categorias" [value]="c.id">{{ c.nome }}</option>
            </select>
          </div>
        </div>

        <div class="form-actions">
          <app-button variant="outline" (onClick)="onCancel.emit()">Cancelar</app-button>
          <app-button [disabled]="!selectedFile || (!config.contaId && !config.cartaoId)" (onClick)="importar()">
            Processar Arquivo
          </app-button>
        </div>
      </div>
    </app-card>
  `,
  styles: [`
    .import-form { display: flex; flex-direction: column; gap: var(--spacing-lg); }
    .form-row { display: grid; grid-template-columns: repeat(3, 1fr); gap: var(--spacing-md); }
    .form-group { display: flex; flex-direction: column; gap: var(--spacing-xs); }
    .form-group label { font-size: 0.875rem; font-weight: 500; color: var(--color-text-secondary); }
    select, input { padding: var(--spacing-sm); border-radius: var(--border-radius); border: 1px solid #e2e8f0; background: white; font-size: 0.875rem; }
    .form-actions { display: flex; justify-content: flex-end; gap: var(--spacing-md); margin-top: var(--spacing-md); }
  `]
})
export class TransacoesImportComponent {
  @Input() categorias: Categoria[] = [];
  @Input() contas: ContaBancaria[] = [];
  @Input() cartoes: CartaoCredito[] = [];
  @Output() onImport = new EventEmitter<{file: File, config: any}>();
  @Output() onCancel = new EventEmitter<void>();

  selectedFile: File | null = null;
  config = { categoriaId: '', contaId: undefined as string | undefined, cartaoId: undefined as string | undefined };

  onFileChange(event: any) { this.selectedFile = event.target.files[0]; }
  importar() { if (this.selectedFile) this.onImport.emit({ file: this.selectedFile, config: this.config }); }
}
