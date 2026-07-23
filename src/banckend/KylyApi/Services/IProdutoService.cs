using KylyApi.DTOs;

namespace KylyApi.Services;

public interface IProdutoService
{
    Task<PaginacaoResponse<ProdutoResponse>> BuscarAsync(string termo, int pagina);
}