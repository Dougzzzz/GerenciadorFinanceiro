import { Component, Input } from '@angular/core';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-card',
  standalone: true,
  imports: [CommonModule],
  template: `
    <div class="card">
      <div class="card-header" *ngIf="title">
        <h3>{{ title }}</h3>
        <ng-content select="[header-actions]"></ng-content>
      </div>
      <div class="card-body">
        <ng-content></ng-content>
      </div>
    </div>
  `,
  styles: [`
    .card {
      background-color: var(--color-card-bg);
      border-radius: var(--border-radius);
      box-shadow: var(--shadow-sm);
      overflow: hidden;
      margin-bottom: var(--spacing-md);
      border: 1px solid #e2e8f0;
    }

    .card-header {
      padding: var(--spacing-md) var(--spacing-lg);
      border-bottom: 1px solid #e2e8f0;
      display: flex;
      justify-content: space-between;
      align-items: center;
    }

    .card-header h3 {
      font-size: 1rem;
      font-weight: 600;
      color: var(--color-sidebar);
    }

    .card-body {
      padding: var(--spacing-lg);
    }
  `]
})
export class CardComponent {
  @Input() title?: string;
}
