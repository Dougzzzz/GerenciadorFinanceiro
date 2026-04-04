import { Routes } from '@angular/router';
import { DashboardComponent } from './features/dashboard/dashboard.component';
import { TransacoesComponent } from './features/transacoes/transacoes.component';

export const routes: Routes = [
  { path: '', component: DashboardComponent },
  { path: 'transacoes', component: TransacoesComponent },
  { path: '**', redirectTo: '' }
];
