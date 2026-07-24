import { Component, EventEmitter, Input, Output } from '@angular/core';
import { finalize } from 'rxjs';
import { MessageModule } from 'primeng/message';
import { ProdutoModel } from '../../../core/models/produto.model';
import { ProdutoService } from '../../../core/services/produto.service';
import { AuthService } from '../../../core/services/auth.service';
import { Router } from '@angular/router';
import { BarraBuscaComponent } from '../barra-busca/barra-busca.component';
import { TabelaProdutosComponent } from '../tabela-produtos/tabela-produtos.component';
import { CabecalhoComponent } from '../../../shared/components/cabecalho/cabecalho.component';
import { CarregandoComponent } from '../../../shared/components/carregando/carregando.component';
import { EstadoVazioComponent } from '../../../shared/components/estado-vazio/estado-vazio.component';

@Component({
  selector: 'app-busca-produtos',
  standalone: true,
  imports: [
    MessageModule,
    BarraBuscaComponent,
    TabelaProdutosComponent,
    CabecalhoComponent,
    CarregandoComponent,
    EstadoVazioComponent
  ],
  templateUrl: './busca-produtos.component.html',
  styleUrl: './busca-produtos.component.css'
})
export class BuscaProdutosComponent {
  termo = '';
  produtos: ProdutoModel[] = [];
  paginaAtual = 1;
  totalRegistros = 0;
  carregando = false;
  erro = '';

  get descricaoSemResultados(): string {
    return `Não encontramos resultados para "${this.termo}". Tente outro código ou palavra da descrição.`;
  }

  constructor(
    private readonly produtoService: ProdutoService,
    private readonly authService: AuthService,
    private readonly router: Router
  ) {}

  pesquisar(termo: string): void {
    this.termo = termo;
    this.paginaAtual = 1;
    this.carregarProdutos();
  }

  mudarPagina(pagina: number): void {
    this.paginaAtual = pagina;
    this.carregarProdutos();
  }

  sair(): void {
    this.authService.logout();
    this.router.navigate(['/login']);
  }

  private carregarProdutos(): void {
    this.carregando = true;
    this.erro = '';

    this.produtoService.buscar(this.termo, this.paginaAtual)
      .pipe(finalize(() => this.carregando = false))
      .subscribe({
        next: resposta => {
          this.produtos = resposta.itens;
          this.totalRegistros = resposta.totalRegistros;
        },
        error: () => {
          this.produtos = [];
          this.totalRegistros = 0;
          this.erro = 'Não foi possível realizar a busca. Tente novamente.';
        }
      });
  }
}
