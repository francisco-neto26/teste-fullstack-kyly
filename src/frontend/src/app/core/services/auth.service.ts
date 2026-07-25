import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, tap } from 'rxjs';
import { LoginRequestModel } from '../models/login-request.model';
import { LoginResponseModel } from '../models/login-response.model';

// Serviço de autenticação para gerenciar login, logout e estado de autenticação do usuário
@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private readonly chaveToken = 'kyly_token';
  private readonly chaveUsuario = 'kyly_usuario';

  constructor(private readonly http: HttpClient) {}

  login(dados: LoginRequestModel): Observable<LoginResponseModel> {
    return this.http
      .post<LoginResponseModel>('/api/auth/login', dados)
      .pipe(
        tap(resposta => {
          localStorage.setItem(this.chaveToken, resposta.token);
          localStorage.setItem(this.chaveUsuario, dados.username);
        })
      );
  }

  get token(): string | null {
    return localStorage.getItem(this.chaveToken);
  }

  get usuario(): string {
    return localStorage.getItem(this.chaveUsuario) || 'Administrador';
  }

  get estaAutenticado(): boolean {
    return !!this.token;
  }

  logout(): void {
    localStorage.removeItem(this.chaveToken);
    localStorage.removeItem(this.chaveUsuario);
  }
}
