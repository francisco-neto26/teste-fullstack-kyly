import { Routes } from '@angular/router';
import { authGuard } from './core/guards/auth.guard';
import { LoginComponent } from './features/auth/login/login.component';
import { BuscaProdutosComponent } from './features/produtos/busca-produtos/busca-produtos.component';

export const routes: Routes = [
  {
    path: 'login',
    component: LoginComponent
  },
  {
    path: 'produtos',
    component: BuscaProdutosComponent,
    canActivate: [authGuard]
  },
  {
    path: '',
    pathMatch: 'full',
    redirectTo: 'produtos'
  },
  {
    path: '**',
    redirectTo: 'produtos'
  }
];