import { ComponentFixture, TestBed } from '@angular/core/testing';
import { of, throwError } from 'rxjs';
import { FinanceiroService } from '../../core/services/financeiro.service';
import { FiltroTransacao } from '../../core/models/filtros.model';
import { TipoTransacao } from '../../core/models/financeiro.model';
import { TransacoesComponent } from './transacoes.component';

describe('TransacoesComponent', () => {
  let fixture: ComponentFixture<TransacoesComponent>;
  let component: TransacoesComponent;
  let financeiroService: jasmine.SpyObj<FinanceiroService>;

  beforeEach(async () => {
    financeiroService = jasmine.createSpyObj<FinanceiroService>('FinanceiroService', [
      'getTransacoes',
      'getCategorias',
      'getContas',
      'getCartoes',
      'criarTransacao',
      'importarExtrato',
      'atualizarTransacao',
      'excluirTransacoes',
    ]);

    financeiroService.getTransacoes.and.returnValue(of([]));
    financeiroService.getCategorias.and.returnValue(of([]));
    financeiroService.getContas.and.returnValue(of([]));
    financeiroService.getCartoes.and.returnValue(of([]));
    financeiroService.criarTransacao.and.returnValue(
      of({
        id: 'transacao-nova',
        data: '2026-04-17T00:00:00Z',
        descricao: 'Nova transacao',
        valor: 100,
        tipo: TipoTransacao.Receita,
        categoriaId: 'cat-1',
        categoria: 'Receita',
        nomeCartao: '',
        finalCartao: '',
        parcela: '',
        cotacao: 1,
      }),
    );
    financeiroService.importarExtrato.and.returnValue(of('ok'));
    financeiroService.atualizarTransacao.and.returnValue(of({}));
    financeiroService.excluirTransacoes.and.returnValue(of({}));

    await TestBed.configureTestingModule({
      imports: [TransacoesComponent],
      providers: [{ provide: FinanceiroService, useValue: financeiroService }],
    }).compileComponents();

    fixture = TestBed.createComponent(TransacoesComponent);
    component = fixture.componentInstance;
  });

  it('deve carregar transacoes, categorias, contas e cartoes na inicializacao', () => {
    const transacoes = [
      {
        id: 'transacao-1',
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
    ];
    const categorias = [{ id: 'cat-1', nome: 'Receita', tipo: TipoTransacao.Receita }];
    const contas = [{ id: 'conta-1', nomeBanco: 'Inter', saldoAtual: 1000, provedor: 0 }];
    const cartoes = [{ id: 'cartao-1', nome: 'Nubank', limite: 5000, diaFechamento: 10, diaVencimento: 20, provedor: 2 }];

    financeiroService.getTransacoes.and.returnValue(of(transacoes));
    financeiroService.getCategorias.and.returnValue(of(categorias));
    financeiroService.getContas.and.returnValue(of(contas));
    financeiroService.getCartoes.and.returnValue(of(cartoes));

    fixture.detectChanges();

    expect(financeiroService.getTransacoes).toHaveBeenCalledOnceWith(undefined);
    expect(financeiroService.getCategorias).toHaveBeenCalled();
    expect(financeiroService.getContas).toHaveBeenCalled();
    expect(financeiroService.getCartoes).toHaveBeenCalled();
    expect(component.transacoes()).toEqual(transacoes);
    expect(component.categorias()).toEqual(categorias);
    expect(component.contas()).toEqual(contas);
    expect(component.cartoes()).toEqual(cartoes);
  });

  it('deve aplicar o filtro usando os valores do formulario', () => {
    const filtro: FiltroTransacao = {
      dataInicial: '2026-04-01',
      dataFinal: '2026-04-30',
      tipo: 1,
      categoriaId: 'cat-1',
      ordenarPor: 'Data',
      direcao: 'Desc',
    };

    fixture.detectChanges();
    financeiroService.getTransacoes.calls.reset();
    component.filterForm.patchValue(filtro);

    component.aplicarFiltro();

    expect(financeiroService.getTransacoes).toHaveBeenCalledOnceWith(filtro);
  });

  it('deve resetar os filtros e recarregar os dados ao limpar', () => {
    fixture.detectChanges();
    financeiroService.getTransacoes.calls.reset();

    component.filterForm.patchValue({
      dataInicial: '2026-04-01',
      tipo: 1,
      categoriaId: 'cat-1',
      ordenarPor: 'Valor',
      direcao: 'Asc',
    });

    component.limparFiltros();

    expect(component.filterForm.value).toEqual({
      dataInicial: null,
      dataFinal: null,
      tipo: null,
      categoriaId: null,
      ordenarPor: 'Data',
      direcao: 'Desc',
    });
    expect(financeiroService.getTransacoes).toHaveBeenCalledOnceWith(undefined);
  });

  it('deve formatar a data e preparar a transacao para edicao', () => {
    const transacao = {
      id: 'transacao-1',
      data: '2026-04-11T15:30:00Z',
      descricao: 'Mercado',
      valor: -120,
      tipo: TipoTransacao.Despesa,
      categoriaId: 'cat-1',
      categoria: 'Alimentacao',
      nomeCartao: '',
      finalCartao: '',
      parcela: '',
      cotacao: 1,
    };

    component.iniciarEdicao(transacao);

    expect(component.editandoId()).toBe('transacao-1');
    expect(component.tempEdit()).toEqual(
      jasmine.objectContaining({
        id: 'transacao-1',
        data: '2026-04-11',
        descricao: 'Mercado',
      }),
    );
  });

  it('deve excluir as transacoes selecionadas quando o usuario confirmar', () => {
    spyOn(window, 'confirm').and.returnValue(true);

    fixture.detectChanges();
    financeiroService.getTransacoes.calls.reset();
    component.selecionadas.set(new Set(['transacao-1', 'transacao-2']));

    component.excluirSelecionadas();

    expect(window.confirm).toHaveBeenCalledWith('Excluir selecionadas?');
    expect(financeiroService.excluirTransacoes).toHaveBeenCalledOnceWith(['transacao-1', 'transacao-2']);
    expect(component.selecionadas().size).toBe(0);
    expect(financeiroService.getTransacoes).toHaveBeenCalledOnceWith(component.filterForm.value);
  });

  it('nao deve excluir nada quando o usuario cancelar a confirmacao', () => {
    spyOn(window, 'confirm').and.returnValue(false);

    fixture.detectChanges();
    component.selecionadas.set(new Set(['transacao-1']));

    component.excluirSelecionadas();

    expect(financeiroService.excluirTransacoes).not.toHaveBeenCalled();
    expect(component.selecionadas().size).toBe(1);
  });

  it('deve alternar a visibilidade do formulario de nova transacao', () => {
    expect(component.mostrarNovo()).toBeFalse();
    component.mostrarNovo.set(true);
    expect(component.mostrarNovo()).toBeTrue();
  });

  it('deve alternar a visibilidade do formulario de importacao', () => {
    expect(component.mostrarImportacao()).toBeFalse();
    component.mostrarImportacao.set(true);
    expect(component.mostrarImportacao()).toBeTrue();
  });

  it('deve atualizar o formulario e recarregar ao ordenar', () => {
    fixture.detectChanges();
    financeiroService.getTransacoes.calls.reset();
    
    component.aoOrdenar({ coluna: 'Valor', direcao: 'Asc' });
    
    expect(component.filterForm.get('ordenarPor')?.value).toBe('Valor');
    expect(component.filterForm.get('direcao')?.value).toBe('Asc');
    expect(financeiroService.getTransacoes).toHaveBeenCalled();
  });

  it('deve recarregar dados quando aoImportarSucesso for chamado', () => {
    fixture.detectChanges();
    financeiroService.getTransacoes.calls.reset();
    
    component.aoImportarSucesso();
    
    expect(component.mostrarImportacao()).toBeFalse();
    expect(financeiroService.getTransacoes).toHaveBeenCalled();
  });

  it('deve alertar quando salvar nova transacao falhar', () => {
    spyOn(window, 'alert');
    financeiroService.criarTransacao.and.returnValue(
      throwError(() => new Error('Falha ao salvar')),
    );

    fixture.detectChanges();

    component.salvarNovaTransacao({
      descricao: 'Mercado',
      valor: -50,
      categoriaId: 'cat-1',
      data: '2026-04-17',
    });

    expect(window.alert).toHaveBeenCalledWith('Erro ao salvar: Falha ao salvar');
  });
});
