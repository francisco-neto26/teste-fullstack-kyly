import { inject } from '@angular/core';
import { HttpInterceptorFn } from '@angular/common/http';
import { AuthService } from '../services/auth.service';

// interceptor para adicionar o token de autenticação nas requisições HTTP
export const authInterceptor: HttpInterceptorFn = (req, next) => {
  const token = inject(AuthService).token;

  if (!token || !req.url.startsWith('/api/')) {
    return next(req);
  }

  return next(
    req.clone({
      setHeaders: {
        Authorization: `Bearer ${token}`
      }
    })
  );
};