import { ComponentFixture, TestBed } from '@angular/core/testing';
import { DashboardRecentTransactionsComponent } from './dashboard-recent-transactions.component';
import { TipoTransacao } from '../../core/models/financeiro.model';

describe('DashboardRecentTransactionsComponent', () => {
  let fixture: ComponentFixture<DashboardRecentTransactionsComponent>;
  let component: DashboardRecentTransactionsComponent;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [DashboardRecentTransactionsComponent],
    }).compileComponents();

    fixture = TestBed.createComponent(DashboardRecentTransactionsComponent);
    component = fixture.componentInstance;
  });

  it('deve renderizar uma mensagem vazia quando nao houver transacoes', () => {
    component.transacoes = [];

    fixture.detectChanges();

    const text = fixture.nativeElement.textContent as string;

    expect(text).toContain('Nenhuma transação encontrada.');
  });

  it('deve renderizar descricao, categoria e classe visual do valor', () => {
    component.transacoes = [
      {
        id: '1',
        data: '2026-04-10T00:00:00Z',
        descricao: 'Salario',
        valor: 3500,
        tipo: TipoTransacao.Receita,
        categoriaId: 'cat-1',
        categoria: 'Receita',
        nomeCartao: '',
        finalCartao: '',
        parcela: '',
        cotacao: 1,
      },
      {
        id: '2',
        data: '2026-04-11T00:00:00Z',
        descricao: 'Mercado',
        valor: -120,
        tipo: TipoTransacao.Despesa,
        categoriaId: 'cat-2',
        categoria: 'Alimentacao',
        nomeCartao: '',
        finalCartao: '',
        parcela: '',
        cotacao: 1,
      },
    ];

    fixture.detectChanges();

    const text = fixture.nativeElement.textContent as string;
    const values = fixture.nativeElement.querySelectorAll('tbody td.text-right');

    expect(text).toContain('Salario');
    expect(text).toContain('Mercado');
    expect(text).toContain('Receita');
    expect(text).toContain('Alimentacao');
    expect(values[0].classList.contains('income')).toBeTrue();
    expect(values[1].classList.contains('expense')).toBeTrue();
  });

  it('deve usar o nome da navegacao quando ele estiver disponivel', () => {
    component.transacoes = [
      {
        id: '1',
        data: '2026-04-10T00:00:00Z',
        descricao: 'Assinatura',
        valor: -50,
        tipo: TipoTransacao.Despesa,
        categoriaId: 'cat-1',
        categoria: 'Fallback',
        nomeCartao: '',
        finalCartao: '',
        parcela: '',
        cotacao: 1,
        categoriaNavigation: {
          id: 'cat-1',
          nome: 'Streaming',
          tipo: TipoTransacao.Despesa,
        },
      },
    ];

    fixture.detectChanges();

    expect(fixture.nativeElement.textContent).toContain('Streaming');
    expect(fixture.nativeElement.textContent).not.toContain('Fallback');
  });
});
