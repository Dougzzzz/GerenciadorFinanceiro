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
      'getResumoMensal',
    ]);

    financeiroService.getTransacoes.and.returnValue(of([]));
    financeiroService.getResumoMetas.and.returnValue(of([]));
    financeiroService.getResumoMensal.and.returnValue(of({
      totalReceitas: 0,
      totalDespesas: 0,
      saldo: 0,
      mes: 4,
      ano: 2026,
      gastosPorCategoria: [],
    }));

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
    const resumoMensal = {
      totalReceitas: 5000,
      totalDespesas: 1000,
      saldo: 4000,
      mes: 4,
      ano: 2026,
      gastosPorCategoria: [],
    };

    financeiroService.getTransacoes.and.returnValue(of(transacoes));
    financeiroService.getResumoMetas.and.returnValue(of(metasResumo));
    financeiroService.getResumoMensal.and.returnValue(of(resumoMensal));

    fixture.detectChanges();

    expect(financeiroService.getTransacoes).toHaveBeenCalledOnceWith();
    expect(financeiroService.getResumoMetas).toHaveBeenCalled();
    expect(financeiroService.getResumoMensal).toHaveBeenCalled();
    expect(component.transacoes()).toEqual(transacoes);
    expect(component.metasResumo()).toEqual(metasResumo);
    expect(component.resumo()).toEqual(resumoMensal);
  });

  it('deve calcular total de receitas, despesas e saldo corretamente a partir do resumo', () => {
    component.resumo.set({
      totalReceitas: 5800,
      totalDespesas: 1200,
      saldo: 4600,
      mes: 4,
      ano: 2026,
      gastosPorCategoria: [],
    });

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
