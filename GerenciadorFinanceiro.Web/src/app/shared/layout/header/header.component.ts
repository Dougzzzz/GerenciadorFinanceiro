import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-header',
  standalone: true,
  imports: [CommonModule],
  template: `
    <header class="header">
      <div class="user-info">
        <div class="avatar">AP</div>
        <span>Ana Paula</span>
      </div>
    </header>
  `,
  styles: [`
    .header {
      height: 64px;
      background-color: var(--color-card-bg);
      border-bottom: 1px solid #e2e8f0;
      padding: 0 var(--spacing-xl);
      display: flex;
      justify-content: flex-end;
      align-items: center;
      position: sticky;
      top: 0;
      z-index: 100;
    }

    .user-info {
      display: flex;
      align-items: center;
      gap: var(--spacing-sm);
      font-weight: 500;
      color: var(--color-text-primary);
    }

    .avatar {
      width: 32px;
      height: 32px;
      background-color: var(--color-primary);
      color: white;
      border-radius: 50%;
      display: flex;
      align-items: center;
      justify-content: center;
      font-size: 0.75rem;
    }
  `]
})
export class HeaderComponent {}
