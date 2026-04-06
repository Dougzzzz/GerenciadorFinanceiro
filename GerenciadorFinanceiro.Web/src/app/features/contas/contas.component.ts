import { Component, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FinanceiroService } from '../../core/services/financeiro.service';
import { ContaBancaria } from '../../core/models/financeiro.model';
import { ContasFormComponent } from './contas-form.component';
import { ContasListComponent } from './contas-list.component';

@Component({
  selector: 'app-contas',
  standalone: true,
  imports: [CommonModule, ContasFormComponent, ContasListComponent],
  template: `
    <div class="contas">
      <h1>Contas Bancárias</h1>
      
      <div class="grid">
        <app-contas-form (onSalvar)="salvar($event)"></app-contas-form>
        <app-contas-list [contas]="contas()"></app-contas-list>
      </div>
    </div>
  `,
  styles: [`
    .grid { display: grid; grid-template-columns: 1fr 1.5fr; gap: var(--spacing-lg); margin-top: var(--spacing-xl); }
  `]
})
export class ContasComponent implements OnInit {
  contas = signal<ContaBancaria[]>([]);

  constructor(private service: FinanceiroService) {}

  ngOnInit() { this.carregar(); }

  carregar() { 
    this.service.getContas().subscribe(data => this.contas.set(data)); 
  }

  salvar(dados: { nomeBanco: string, saldoInicial: number }) {
    this.service.criarConta(dados.nomeBanco, dados.saldoInicial).subscribe(() => {
      this.carregar();
    });
  }
}
