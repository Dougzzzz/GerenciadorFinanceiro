import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { LoadingService } from '../../../core/services/loading.service';

@Component({
  selector: 'app-loading',
  standalone: true,
  imports: [CommonModule],
  template: `
    <div class="loading-overlay" *ngIf="loadingService.isLoading()">
      <div class="loader-content">
        <div class="spinner"></div>
        <p>A carregar dados...</p>
      </div>
    </div>
  `,
  styles: [`
    .loading-overlay {
      position: fixed;
      top: 0;
      left: 0;
      width: 100%;
      height: 100%;
      background: rgba(255, 255, 255, 0.7);
      backdrop-filter: blur(2px);
      display: flex;
      justify-content: center;
      align-items: center;
      z-index: 9999;
    }

    .loader-content {
      display: flex;
      flex-direction: column;
      align-items: center;
      gap: var(--spacing-md);
      color: var(--color-sidebar);
    }

    .spinner {
      width: 50px;
      height: 50px;
      border: 5px solid #e2e8f0;
      border-top: 5px solid var(--color-primary);
      border-radius: 50%;
      animation: spin 1s linear infinite;
    }

    @keyframes spin {
      0% { transform: rotate(0deg); }
      100% { transform: rotate(360deg); }
    }

    p { font-weight: 600; font-size: 1.1rem; }
  `]
})
export class LoadingComponent {
  constructor(public loadingService: LoadingService) {}
}
