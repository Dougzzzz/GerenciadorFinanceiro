import { HttpErrorResponse, HttpInterceptorFn } from '@angular/common/http';
import { catchError, throwError } from 'rxjs';

interface ApiErrorResponse {
  message?: string;
  details?: string | null;
  statusCode?: number;
  traceId?: string;
}

function extractMessage(error: HttpErrorResponse): string {
  const payload = error.error;

  if (typeof payload === 'string' && payload.trim().length > 0) {
    return payload;
  }

  if (payload && typeof payload === 'object') {
    const apiError = payload as ApiErrorResponse;

    if (apiError.message && apiError.message.trim().length > 0) {
      return apiError.message;
    }
  }

  if (error.status === 0) {
    return 'Nao foi possivel se comunicar com a API. Verifique a conexao e tente novamente.';
  }

  return 'Ocorreu um erro ao processar sua solicitacao.';
}

export const apiErrorInterceptor: HttpInterceptorFn = (_req, next) =>
  next(_req).pipe(
    catchError((error: unknown) => {
      if (error instanceof HttpErrorResponse) {
        console.error('API Error:', error);
        return throwError(() => new Error(extractMessage(error)));
      }

      return throwError(() => error);
    })
  );
