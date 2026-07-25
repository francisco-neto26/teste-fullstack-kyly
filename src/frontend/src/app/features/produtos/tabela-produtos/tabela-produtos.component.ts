import { Component, EventEmitter, Input, Output } from '@angular/core';
import { TableModule } from 'primeng/table';
import { PaginatorModule, PaginatorState } from 'primeng/paginator';
import { TagModule } from 'primeng/tag';
import { ButtonModule } from 'primeng/button';
import { DialogModule } from 'primeng/dialog';
import { ProdutoModel } from '../../../core/models/produto.model';
import { ProdutoService } from '../../../core/services/produto.service';
import { forkJoin, finalize } from 'rxjs';

// Componente de tabela de produtos
@Component({
  selector: 'app-tabela-produtos',
  standalone: true,
  imports: [TableModule, PaginatorModule, TagModule, ButtonModule, DialogModule],
  templateUrl: './tabela-produtos.component.html',
  styleUrl: './tabela-produtos.component.css'
})
export class TabelaProdutosComponent {
  @Input({ required: true }) produtos: ProdutoModel[] = [];
  @Input({ required: true }) totalRegistros = 0;
  @Input({ required: true }) paginaAtual = 1;
  @Input() termo = '';

  @Output() mudarPagina = new EventEmitter<number>();

  carregandoExportacao = false;
  exibirModalExportacao = false;

  produtoSelecionado: ProdutoModel | null = null;
  exibirModalDetalhes = false;
  codigoCopiado = false;

  constructor(private readonly produtoService: ProdutoService) {}

  get totalPaginas(): number {
    return Math.max(1, Math.ceil(this.totalRegistros / 15));
  }

  aoMudarPagina(evento: PaginatorState): void {
    this.mudarPagina.emit((evento.page ?? 0) + 1);
  }

  // Função para abrir o modal de detalhes do produto
  selecionarProduto(produto: ProdutoModel): void {
    this.produtoSelecionado = produto;
    this.codigoCopiado = false;
    this.exibirModalDetalhes = true;
  }

  // funcao para copiar o código do produto para a área de transferência
  copiarCodigo(codigo: string, evento?: Event): void {
    if (evento) {
      evento.stopPropagation();
    }
    navigator.clipboard.writeText(codigo).then(() => {
      this.codigoCopiado = true;
      setTimeout(() => (this.codigoCopiado = false), 2000);
    });
  }

  // Função para solicitar a exportação dos produtos, seja da página atual ou de todas as páginas
  solicitarExportacao(): void {
    if (!this.totalRegistros) return;

    if (this.totalPaginas === 1) {
      this.exportarPaginaAtual();
    } else {
      this.exibirModalExportacao = true;
    }
  }
  // Função para exportar os produtos da página atual
  exportarPaginaAtual(): void {
    this.exibirModalExportacao = false;
    this.gerarDownloadCsv(this.produtos, `produtos_pagina_${this.paginaAtual}.csv`);
  }
  // Função para exportar todos os produtos, de todas as páginas
  exportarTudo(): void {
    this.exibirModalExportacao = false;
    this.carregandoExportacao = true;

    const requisicoes = [];
    for (let p = 1; p <= this.totalPaginas; p++) {
      requisicoes.push(this.produtoService.buscar(this.termo, p));
    }
    // Usando forkJoin para esperar todas as requisições completarem
    forkJoin(requisicoes)
      .pipe(finalize(() => (this.carregandoExportacao = false)))
      .subscribe({
        next: respostas => {
          const todosProdutos = respostas.flatMap(r => r.itens);
          const termoNome = this.termo ? this.termo.replace(/[^a-zA-Z0-9]/g, '_') : 'todos';
          this.gerarDownloadCsv(todosProdutos, `produtos_completo_${termoNome}.csv`);
        },
        error: () => {
          this.exportarPaginaAtual();
        }
      });
  }

  // Função para gerar o arquivo CSV a partir da lista de produtos
  private gerarDownloadCsv(listaProdutos: ProdutoModel[], nomeArquivo: string): void {
    const cabecalho = ['ID', 'Código Produto', 'Descrição Produto', 'Código Cor', 'Descrição Cor', 'Código Tamanho', 'Descrição Tamanho'];

    const linhas = listaProdutos.map(p => [
      `"${p.id ?? ''}"`,
      `"${p.codigoProduto ?? ''}"`,
      `"${p.descProduto ?? ''}"`,
      `"${p.codigoCor ?? ''}"`,
      `"${p.descCor ?? ''}"`,
      `"${p.codigoTamanho ?? ''}"`,
      `"${p.descTamanho ?? ''}"`
    ].join(';'));

    const conteudoCsv = '\uFEFF' + [cabecalho.join(';'), ...linhas].join('\n');
    const blob = new Blob([conteudoCsv], { type: 'text/csv;charset=utf-8;' });
    const url = URL.createObjectURL(blob);

    const link = document.createElement('a');
    link.href = url;
    link.download = nomeArquivo;
    link.click();
    URL.revokeObjectURL(url);
  }
}
