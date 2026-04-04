import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';

@Component({
  selector: 'app-sidebar',
  standalone: true,
  imports: [CommonModule, RouterModule],
  template: `
    <aside class="sidebar">
      <div class="logo">
        <h2>Controle<span>+</span></h2>
      </div>
      <nav>
        <ul>
          <li><a routerLink="/" routerLinkActive="active" [routerLinkActiveOptions]="{exact: true}">Dashboard</a></li>
          <li><a routerLink="/transacoes" routerLinkActive="active">Transações</a></li>
          <li><a routerLink="/contas" routerLinkActive="active">Contas</a></li>
          <li><a routerLink="/cartoes" routerLinkActive="active">Cartões</a></li>
        </ul>
      </nav>
    </aside>
  `,
  styles: [`
    .sidebar {
      width: 240px;
      height: 100vh;
      background-color: var(--color-sidebar);
      color: var(--color-sidebar-text);
      padding: var(--spacing-xl) 0;
      position: fixed;
      left: 0;
      top: 0;
    }

    .logo {
      padding: 0 var(--spacing-xl);
      margin-bottom: var(--spacing-xl);
    }

    .logo h2 {
      color: white;
      font-size: 1.5rem;
      letter-spacing: -1px;
    }

    .logo span {
      color: var(--color-income);
    }

    nav ul li a {
      display: block;
      padding: var(--spacing-md) var(--spacing-xl);
      color: #94a3b8;
      transition: all 0.2s;
      border-left: 4px solid transparent;
    }

    nav ul li a:hover {
      background-color: rgba(255,255,255,0.05);
      color: white;
    }

    nav ul li a.active {
      background-color: rgba(59, 130, 246, 0.1);
      color: white;
      border-left-color: var(--color-primary);
    }
  `]
})
export class SidebarComponent {}
