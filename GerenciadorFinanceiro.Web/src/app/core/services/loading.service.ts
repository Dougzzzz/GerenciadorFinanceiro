import { Injectable, signal } from '@angular/core';

@Injectable({
  providedIn: 'root'
})
export class LoadingService {
  private activeRequests = signal(0);
  
  // Exposto como signal apenas leitura para o componente
  isLoading = signal(false);

  show() {
    this.activeRequests.update(v => v + 1);
    this.isLoading.set(true);
  }

  hide() {
    this.activeRequests.update(v => Math.max(0, v - 1));
    if (this.activeRequests() === 0) {
      this.isLoading.set(false);
    }
  }
}
