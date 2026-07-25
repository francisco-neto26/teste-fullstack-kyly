import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { PaginacaoModel } from '../models/paginacao.model';
import { ProdutoModel } from '../models/produto.model';

// Serviço para gerenciar operações relacionadas a produtos
@Injectable({
  providedIn: 'root'
})
export class ProdutoService {
  constructor(private readonly http: HttpClient) {}

  buscar(termo: string, pagina: number): Observable<PaginacaoModel<ProdutoModel>> {
    const params = new HttpParams()
      .set('termo', termo)
      .set('pagina', pagina);

    return this.http.get<PaginacaoModel<ProdutoModel>>('/api/produtos', { params });
  }
}