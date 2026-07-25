import { inject } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';
import { AuthService } from '../services/auth.service';

// Guarda de rota para proteger as rotas que exigem autenticação
export const authGuard: CanActivateFn = () => {
  const authService = inject(AuthService);
  const router = inject(Router);

  return authService.estaAutenticado
    ? true
    : router.createUrlTree(['/login']);
};