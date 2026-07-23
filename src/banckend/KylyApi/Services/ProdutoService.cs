using KylyApi.Data;
using KylyApi.DTOs;
using Microsoft.EntityFrameworkCore;

namespace KylyApi.Services;

public class ProdutoService : IProdutoService
{
    private const int TamanhoPagina = 15;

    private readonly AppDbContext _context;

    public ProdutoService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<PaginacaoResponse<ProdutoResponse>> BuscarAsync(string termo, int pagina)
    {
        var termoBusca = termo.Trim();
        var filtro = $"%{termoBusca}%";

        // Busca por código ou por palavra presente na descrição do produto.
        var consulta = _context.Produtos
            .AsNoTracking()
            .Where(produto =>
                EF.Functions.ILike(produto.CodigoProduto, filtro) ||
                EF.Functions.ILike(produto.DescProduto, filtro))
            .Select(produto => new
            {
                Produto = produto,

                // A menor prioridade encontrada prevalece: 1 antes de 2.
                Prioridade = _context.ListasRelevancia
                    .Where(lista => lista.CodigoProduto == produto.CodigoProduto)
                    .Select(lista => (int?)lista.Prioridade)
                    .Min()
            });

        var totalRegistros = await consulta.CountAsync();

        var produtos = await consulta
            // Produtos sem prioridade recebem 3 e aparecem após listas 1 e 2.
            .OrderBy(item => item.Prioridade ?? 3)
            .ThenBy(item => item.Produto.CodigoProduto)
            .ThenBy(item => item.Produto.DescProduto)
            .Skip((pagina - 1) * TamanhoPagina)
            .Take(TamanhoPagina)
            .Select(item => new ProdutoResponse
            {
                Id = item.Produto.Id,
                CodigoProduto = item.Produto.CodigoProduto,
                DescProduto = item.Produto.DescProduto,
                CodigoCor = item.Produto.CodigoCor,
                DescCor = item.Produto.DescCor,
                CodigoTamanho = item.Produto.CodigoTamanho,
                DescTamanho = item.Produto.DescTamanho
            })
            .ToListAsync();

        return new PaginacaoResponse<ProdutoResponse>
        {
            PaginaAtual = pagina,
            TamanhoPagina = TamanhoPagina,
            TotalRegistros = totalRegistros,
            TotalPaginas = (int)Math.Ceiling(totalRegistros / (double)TamanhoPagina),
            Itens = produtos
        };
    }
}