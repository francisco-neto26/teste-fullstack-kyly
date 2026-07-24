import { Component, EventEmitter, Input, Output } from '@angular/core';
import { TableModule } from 'primeng/table';
import { PaginatorModule, PaginatorState } from 'primeng/paginator';
import { TagModule } from 'primeng/tag';
import { ProdutoModel } from '../../../core/models/produto.model';

@Component({
  selector: 'app-tabela-produtos',
  standalone: true,
  imports: [TableModule, PaginatorModule, TagModule],
  templateUrl: './tabela-produtos.component.html',
  styleUrl: './tabela-produtos.component.css'
})
export class TabelaProdutosComponent {
  @Input({ required: true }) produtos: ProdutoModel[] = [];
  @Input({ required: true }) totalRegistros = 0;
  @Input({ required: true }) paginaAtual = 1;

  @Output() mudarPagina = new EventEmitter<number>();

  get totalPaginas(): number {
    return Math.max(1, Math.ceil(this.totalRegistros / 15));
  }

  aoMudarPagina(evento: PaginatorState): void {
    this.mudarPagina.emit((evento.page ?? 0) + 1);
  }
}
