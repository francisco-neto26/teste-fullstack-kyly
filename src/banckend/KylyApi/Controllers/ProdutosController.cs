using KylyApi.DTOs;
using KylyApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

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
    [ProducesResponseType(typeof(PaginacaoResponse<ProdutoResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
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

        var resultado = await _produtoService.BuscarAsync(termo, pagina);

        return Ok(resultado);
    }
}