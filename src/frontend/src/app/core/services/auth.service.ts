import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, tap } from 'rxjs';
import { LoginRequestModel } from '../models/login-request.model';
import { LoginResponseModel } from '../models/login-response.model';

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private readonly chaveToken = 'kyly_token';

  constructor(private readonly http: HttpClient) {}

  login(dados: LoginRequestModel): Observable<LoginResponseModel> {
    return this.http
      .post<LoginResponseModel>('/api/auth/login', dados)
      .pipe(tap(resposta => localStorage.setItem(this.chaveToken, resposta.token)));
  }

  get token(): string | null {
    return localStorage.getItem(this.chaveToken);
  }

  get estaAutenticado(): boolean {
    return !!this.token;
  }

  logout(): void {
    localStorage.removeItem(this.chaveToken);
  }
}