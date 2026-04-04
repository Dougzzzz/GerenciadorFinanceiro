import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { SidebarComponent } from './shared/layout/sidebar/sidebar.component';
import { HeaderComponent } from './shared/layout/header/header.component';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [CommonModule, RouterModule, SidebarComponent, HeaderComponent],
  template: `
    <div class="app-container">
      <app-sidebar></app-sidebar>
      <div class="main-content">
        <app-header></app-header>
        <main class="content">
          <router-outlet></router-outlet>
        </main>
      </div>
    </div>
  `,
  styles: [`
    .app-container {
      display: flex;
    }

    .main-content {
      flex: 1;
      margin-left: 240px;
      min-height: 100vh;
      display: flex;
      flex-direction: column;
    }

    .content {
      padding: var(--spacing-xl);
      flex: 1;
    }
  `]
})
export class AppComponent {
  title = 'GerenciadorFinanceiro.Web';
}
