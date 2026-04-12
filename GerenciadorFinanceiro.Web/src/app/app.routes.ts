import { Routes } from '@angular/router';
import { DashboardComponent } from './features/dashboard/dashboard.component';
import { TransacoesComponent } from './features/transacoes/transacoes.component';
import { CategoriasComponent } from './features/categorias/categorias.component';
import { ContasComponent } from './features/contas/contas.component';
import { CartoesComponent } from './features/cartoes/cartoes.component';

export const routes: Routes = [
  { path: '', component: DashboardComponent },
  { path: 'transacoes', component: TransacoesComponent },
  { path: 'categorias', component: CategoriasComponent },
  { path: 'contas', component: ContasComponent },
  { path: 'cartoes', component: CartoesComponent },
  { path: 'metas-gastos', loadComponent: () => import('./features/metas-gastos/metas-gastos.component').then(m => m.MetasGastosComponent) },
  { path: '**', redirectTo: '' }
];
