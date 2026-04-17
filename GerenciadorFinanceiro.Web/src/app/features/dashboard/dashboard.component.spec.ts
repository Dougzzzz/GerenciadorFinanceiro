import { ComponentFixture, TestBed } from '@angular/core/testing';
import { of } from 'rxjs';
import { FinanceiroService } from '../../core/services/financeiro.service';
import { TipoTransacao } from '../../core/models/financeiro.model';
import { DashboardComponent } from './dashboard.component';

describe('DashboardComponent', () => {
  let fixture: ComponentFixture<DashboardComponent>;
  let component: DashboardComponent;
  let financeiroService: jasmine.SpyObj<FinanceiroService>;

  beforeEach(async () => {
    financeiroService = jasmine.createSpyObj<FinanceiroService>('FinanceiroService', [
      'getTransacoes',
      'getResumoMetas',
    ]);

    financeiroService.getTransacoes.and.returnValue(of([]));
    financeiroService.getResumoMetas.and.returnValue(of([]));

    await TestBed.configureTestingModule({
      imports: [DashboardComponent],
      providers: [{ provide: FinanceiroService, useValue: financeiroService }],
    }).compileComponents();

    fixture = TestBed.createComponent(DashboardComponent);
    component = fixture.componentInstance;
  });

  it('deve carregar transacoes e resumo de metas na inicializacao', () => {
    const transacoes = [
      {
        id: '1',
        data: '2026-04-10T00:00:00Z',
        descricao: 'Salario',
        valor: 5000,
        tipo: TipoTransacao.Receita,
        categoriaId: 'cat-1',
        categoria: 'Salario',
        nomeCartao: '',
        finalCartao: '',
        parcela: '',
        cotacao: 1,
      },
    ];
    const metasResumo = [
      {
        categoria: 'Alimentacao',
        meta: 1000,
        gastoAtual: 650,
        percentual: 65,
      },
    ];

    financeiroService.getTransacoes.and.returnValue(of(transacoes));
    financeiroService.getResumoMetas.and.returnValue(of(metasResumo));

    fixture.detectChanges();

    expect(financeiroService.getTransacoes).toHaveBeenCalledOnceWith();
    expect(financeiroService.getResumoMetas).toHaveBeenCalled();
    expect(component.transacoes()).toEqual(transacoes);
    expect(component.metasResumo()).toEqual(metasResumo);
  });

  it('deve calcular total de receitas, despesas e saldo corretamente', () => {
    component.transacoes.set([
      {
        id: '1',
        data: '2026-04-10T00:00:00Z',
        descricao: 'Salario',
        valor: 5000,
        tipo: TipoTransacao.Receita,
        categoriaId: 'cat-1',
        categoria: 'Salario',
        nomeCartao: '',
        finalCartao: '',
        parcela: '',
        cotacao: 1,
      },
      {
        id: '2',
        data: '2026-04-11T00:00:00Z',
        descricao: 'Mercado',
        valor: -1200,
        tipo: TipoTransacao.Despesa,
        categoriaId: 'cat-2',
        categoria: 'Alimentacao',
        nomeCartao: '',
        finalCartao: '',
        parcela: '',
        cotacao: 1,
      },
      {
        id: '3',
        data: '2026-04-12T00:00:00Z',
        descricao: 'Freelance',
        valor: 800,
        tipo: TipoTransacao.Receita,
        categoriaId: 'cat-3',
        categoria: 'Extra',
        nomeCartao: '',
        finalCartao: '',
        parcela: '',
        cotacao: 1,
      },
    ]);

    expect(component.totalReceitas()).toBe(5800);
    expect(component.totalDespesas()).toBe(1200);
    expect(component.saldo()).toBe(4600);
  });

  it('deve retornar apenas as 5 transacoes mais recentes em ordem decrescente', () => {
    component.transacoes.set([
      { id: '1', data: '2026-04-01T00:00:00Z', descricao: 'T1', valor: 10, tipo: TipoTransacao.Receita, categoriaId: 'c', categoria: 'A', nomeCartao: '', finalCartao: '', parcela: '', cotacao: 1 },
      { id: '2', data: '2026-04-02T00:00:00Z', descricao: 'T2', valor: 10, tipo: TipoTransacao.Receita, categoriaId: 'c', categoria: 'A', nomeCartao: '', finalCartao: '', parcela: '', cotacao: 1 },
      { id: '3', data: '2026-04-03T00:00:00Z', descricao: 'T3', valor: 10, tipo: TipoTransacao.Receita, categoriaId: 'c', categoria: 'A', nomeCartao: '', finalCartao: '', parcela: '', cotacao: 1 },
      { id: '4', data: '2026-04-04T00:00:00Z', descricao: 'T4', valor: 10, tipo: TipoTransacao.Receita, categoriaId: 'c', categoria: 'A', nomeCartao: '', finalCartao: '', parcela: '', cotacao: 1 },
      { id: '5', data: '2026-04-05T00:00:00Z', descricao: 'T5', valor: 10, tipo: TipoTransacao.Receita, categoriaId: 'c', categoria: 'A', nomeCartao: '', finalCartao: '', parcela: '', cotacao: 1 },
      { id: '6', data: '2026-04-06T00:00:00Z', descricao: 'T6', valor: 10, tipo: TipoTransacao.Receita, categoriaId: 'c', categoria: 'A', nomeCartao: '', finalCartao: '', parcela: '', cotacao: 1 },
    ]);

    expect(component.transacoesRecentes().map(t => t.id)).toEqual(['6', '5', '4', '3', '2']);
  });
});
