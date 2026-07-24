export interface PaginacaoModel<T> {
  paginaAtual: number;
  tamanhoPagina: number;
  totalRegistros: number;
  totalPaginas: number;
  itens: T[];
}