namespace KylyApi.DTOs;

public class PaginacaoResponse<T>
{
    public int PaginaAtual { get; set; }
    public int TamanhoPagina { get; set; }
    public int TotalRegistros { get; set; }
    public int TotalPaginas { get; set; }
    public List<T> Itens { get; set; } = [];
}