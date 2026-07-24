using KylyApi.DTOs;

namespace KylyApi.Services;
// Interface para o serviço de produtos, definindo os métodos que devem ser implementados.
public interface IProdutoService
{
    Task<PaginacaoResponse<ProdutoResponse>> BuscarAsync(string termo, int pagina);
}