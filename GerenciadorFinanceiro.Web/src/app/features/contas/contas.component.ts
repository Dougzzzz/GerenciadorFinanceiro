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
        <app-contas-form 
          [editando]="!!editando()" 
          [novo]="novo()"
          (onSalvar)="salvar($event)"
          (onLimpar)="limpar()">
        </app-contas-form>
        <app-contas-list 
          [contas]="contas()"
          (onIniciarEdicao)="iniciarEdicao($event)"
          (onExcluir)="excluir($event)">
        </app-contas-list>
      </div>
    </div>
  `,
  styles: [`
    .grid { display: grid; grid-template-columns: 1fr 1.5fr; gap: var(--spacing-lg); margin-top: var(--spacing-xl); }
  `]
})
export class ContasComponent implements OnInit {
  contas = signal<ContaBancaria[]>([]);
  editando = signal<ContaBancaria | null>(null);
  novo = signal<any>({ nomeBanco: '', saldoInicial: 0, provedor: 0 });

  constructor(private service: FinanceiroService) {}

  ngOnInit() { this.carregar(); }

  carregar() { 
    this.service.getContas().subscribe(data => this.contas.set(data)); 
  }

  iniciarEdicao(conta: ContaBancaria) {
    this.editando.set(conta);
    this.novo.set({
      nomeBanco: conta.nomeBanco,
      saldoInicial: conta.saldoAtual,
      provedor: conta.provedor
    });
  }

  limpar() {
    this.editando.set(null);
    this.novo.set({ nomeBanco: '', saldoInicial: 0, provedor: 0 });
  }

  salvar(dados: any) {
    const atual = this.editando();
    const obs = atual 
      ? this.service.atualizarConta(atual.id, dados.nomeBanco, dados.saldoInicial, dados.provedor)
      : this.service.criarConta(dados.nomeBanco, dados.saldoInicial, dados.provedor);

    obs.subscribe({
      next: () => {
        this.carregar();
        this.limpar();
      },
      error: (err) => alert('Erro ao salvar conta: ' + (err.error?.message || err.message))
    });
  }

  excluir(id: string) {
    if (confirm('Deseja realmente excluir esta conta?')) {
      this.service.excluirConta(id).subscribe({
        next: () => this.carregar(),
        error: (err) => alert('Erro ao excluir conta: ' + (err.error?.message || err.message))
      });
    }
  }
}
