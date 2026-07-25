using KylyApi.DTOs;
using KylyApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

// Controller responsável por lidar com operações relacionadas a produtos, incluindo busca e paginação.
namespace KylyApi.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ProdutosController : ControllerBase
{
    private readonly IProdutoService _produtoService;

    public ProdutosController(IProdutoService produtoService)
    {
        _produtoService = produtoService;
    }

    [HttpGet]
    // Retorna uma lista paginada de produtos que correspondem ao termo de busca.
    [ProducesResponseType(typeof(PaginacaoResponse<ProdutoResponse>), StatusCodes.Status200OK)]
    // Retorna BadRequest se o termo de busca estiver vazio ou nulo.
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    // Retorna Unauthorized se o usuário não estiver autenticado.
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<PaginacaoResponse<ProdutoResponse>>> Buscar(
        [FromQuery] string? termo,
        [FromQuery] int pagina = 1)
    {
        
        if (string.IsNullOrWhiteSpace(termo))
        {
            return BadRequest("Informe um termo para realizar a busca.");
        }
        
        if (pagina < 1)
        {
            return BadRequest("A página deve ser maior ou igual a 1.");
        }

        // Chama o serviço para buscar os produtos com base no termo e na página fornecidos.
        var resultado = await _produtoService.BuscarAsync(termo, pagina);

        return Ok(resultado);
    }
}