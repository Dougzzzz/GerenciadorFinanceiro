import { TestBed } from '@angular/core/testing';
import { HttpClient, provideHttpClient, withInterceptors, HttpErrorResponse } from '@angular/common/http';
import { HttpTestingController, provideHttpClientTesting } from '@angular/common/http/testing';
import { apiErrorInterceptor } from './api-error.interceptor';

describe('apiErrorInterceptor', () => {
  let httpClient: HttpClient;
  let httpMock: HttpTestingController;

  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [
        provideHttpClient(withInterceptors([apiErrorInterceptor])),
        provideHttpClientTesting(),
      ],
    });

    httpClient = TestBed.inject(HttpClient);
    httpMock = TestBed.inject(HttpTestingController);
  });

  afterEach(() => {
    httpMock.verify();
  });

  it('deve extrair mensagem quando o payload e uma string', (done) => {
    httpClient.get('/test').subscribe({
      error: (err: Error) => {
        expect(err.message).toBe('Erro customizado string');
        done();
      }
    });

    const req = httpMock.expectOne('/test');
    req.flush('Erro customizado string', { status: 400, statusText: 'Bad Request' });
  });

  it('deve extrair mensagem de um objeto de erro padrao da API', (done) => {
    httpClient.get('/test').subscribe({
      error: (err: Error) => {
        expect(err.message).toBe('Mensagem do objeto');
        done();
      }
    });

    const req = httpMock.expectOne('/test');
    req.flush({ message: 'Mensagem do objeto' }, { status: 500, statusText: 'Internal Error' });
  });

  it('deve retornar mensagem amigavel para erro de conexao (status 0)', (done) => {
    httpClient.get('/test').subscribe({
      error: (err: Error) => {
        expect(err.message).toContain('Nao foi possivel se comunicar');
        done();
      }
    });

    const req = httpMock.expectOne('/test');
    req.error(new ProgressEvent('error'));
  });

  it('deve retornar mensagem generica quando o payload e vazio ou invalido', (done) => {
    httpClient.get('/test').subscribe({
      error: (err: Error) => {
        expect(err.message).toBe('Ocorreu um erro ao processar sua solicitacao.');
        done();
      }
    });

    const req = httpMock.expectOne('/test');
    req.flush(null, { status: 404, statusText: 'Not Found' });
  });

  it('deve repassar erros que nao sao HttpErrorResponse', (done) => {
    // Para este caso, precisamos simular um erro que não é HttpErrorResponse 
    // O interceptor faz: if (error instanceof HttpErrorResponse)
    // Erros manuais no pipe de um observable poderiam testar isso, mas aqui focamos no InterceptorFn
    
    // Teste de cobertura para o 'return throwError(() => error);' final
    // Como interceptamos o 'next', vamos simular o throw direto
    const nextFn: any = () => { throw 'Erro que nao e HTTP'; };
    try {
        apiErrorInterceptor(null as any, nextFn).subscribe();
    } catch (e) {
        expect(e).toBe('Erro que nao e HTTP');
        done();
    }
  });
});
