namespace KylyApi.DTOs;

// Classe genérica para encapsular respostas paginadas, incluindo informações sobre a página atual, tamanho da página, total de registros e total de páginas.
public class PaginacaoResponse<T>
{
    public int PaginaAtual { get; set; }
    public int TamanhoPagina { get; set; }
    public int TotalRegistros { get; set; }
    public int TotalPaginas { get; set; }
    public List<T> Itens { get; set; } = [];
}