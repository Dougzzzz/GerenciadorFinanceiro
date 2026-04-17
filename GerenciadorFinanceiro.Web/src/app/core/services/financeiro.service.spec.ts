import { TestBed } from '@angular/core/testing';
import { provideHttpClient } from '@angular/common/http';
import {
  HttpTestingController,
  provideHttpClientTesting,
} from '@angular/common/http/testing';
import { FinanceiroService } from './financeiro.service';
import { TipoTransacao } from '../models/financeiro.model';

describe('FinanceiroService', () => {
  let service: FinanceiroService;
  let httpMock: HttpTestingController;

  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [FinanceiroService, provideHttpClient(), provideHttpClientTesting()],
    });

    service = TestBed.inject(FinanceiroService);
    httpMock = TestBed.inject(HttpTestingController);
  });

  afterEach(() => {
    httpMock.verify();
  });

  it('deve buscar transacoes sem filtros quando nenhum filtro for informado', () => {
    service.getTransacoes().subscribe();

    const req = httpMock.expectOne(request => request.method === 'GET' && request.url === '/api/transacoes');

    expect(req.request.params.keys().length).toBe(0);

    req.flush([]);
  });

  it('deve enviar apenas filtros preenchidos e preservar datas no formato yyyy-mm-dd', () => {
    service
      .getTransacoes({
        dataInicial: '2026-04-01',
        dataFinal: '',
        categoriaId: 'categoria-1',
        tipo: TipoTransacao.Despesa,
      })
      .subscribe();

    const req = httpMock.expectOne('/api/transacoes?dataInicial=2026-04-01&categoriaId=categoria-1&tipo=1');

    expect(req.request.method).toBe('GET');
    expect(req.request.params.get('dataInicial')).toBe('2026-04-01');
    expect(req.request.params.get('dataFinal')).toBeNull();
    expect(req.request.params.get('categoriaId')).toBe('categoria-1');
    expect(req.request.params.get('tipo')).toBe('1');

    req.flush([]);
  });

  it('deve converter objetos Date para yyyy-mm-dd ao buscar transacoes', () => {
    service
      .getTransacoes({
        dataInicial: new Date('2026-04-05T15:30:00.000Z'),
      })
      .subscribe();

    const req = httpMock.expectOne('/api/transacoes?dataInicial=2026-04-05');

    expect(req.request.params.get('dataInicial')).toBe('2026-04-05');

    req.flush([]);
  });

  it('deve converter a data para ISO ao criar transacao', () => {
    service
      .criarTransacao({
        descricao: 'Salario',
        data: '2026-04-10',
        valor: 3500,
      })
      .subscribe();

    const req = httpMock.expectOne('/api/transacoes');

    expect(req.request.method).toBe('POST');
    expect(req.request.body.data).toBe(new Date('2026-04-10').toISOString());

    req.flush({});
  });

  it('deve converter a data para ISO ao atualizar transacao', () => {
    service
      .atualizarTransacao({
        id: 'transacao-1',
        descricao: 'Mercado',
        data: '2026-04-11',
        valor: -150,
        tipo: TipoTransacao.Despesa,
        categoriaId: 'categoria-1',
        categoria: 'Alimentacao',
        nomeCartao: '',
        finalCartao: '',
        parcela: '',
        cotacao: 1,
      })
      .subscribe();

    const req = httpMock.expectOne('/api/transacoes/transacao-1');

    expect(req.request.method).toBe('PUT');
    expect(req.request.body.data).toBe(new Date('2026-04-11').toISOString());

    req.flush({});
  });
});
