import { Component, Input, Output, EventEmitter } from '@angular/core';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-button',
  standalone: true,
  imports: [CommonModule],
  template: `
    <button 
      [type]="type" 
      [disabled]="disabled" 
      [class]="'btn btn-' + variant + ' btn-' + size"
      (click)="onClick.emit($event)">
      <ng-content></ng-content>
    </button>
  `,
  styles: [`
    .btn {
      padding: var(--spacing-sm) var(--spacing-lg);
      border-radius: var(--border-radius);
      border: none;
      font-weight: 600;
      cursor: pointer;
      transition: opacity 0.2s, transform 0.1s;
      display: inline-flex;
      align-items: center;
      justify-content: center;
      gap: var(--spacing-sm);
    }

    .btn-sm {
      padding: var(--spacing-xs) var(--spacing-md);
      font-size: 0.875rem;
    }

    .btn-md {
      padding: var(--spacing-sm) var(--spacing-lg);
      font-size: 1rem;
    }

    .btn:active {
      transform: scale(0.98);
    }

    .btn:disabled {
      opacity: 0.5;
      cursor: not-allowed;
    }

    .btn-primary {
      background-color: var(--color-primary);
      color: white;
    }

    .btn-outline {
      background-color: transparent;
      border: 1px solid var(--color-text-secondary);
      color: var(--color-text-primary);
    }

    .btn-ghost {
      background-color: transparent;
      color: var(--color-text-secondary);
    }

    .btn-ghost:hover {
      background-color: rgba(0,0,0,0.05);
    }
  `]
})
export class ButtonComponent {
  @Input() type: 'button' | 'submit' = 'button';
  @Input() variant: 'primary' | 'outline' | 'ghost' = 'primary';
  @Input() size: 'sm' | 'md' = 'md';
  @Input() disabled = false;
  @Output() onClick = new EventEmitter<MouseEvent>();
}
