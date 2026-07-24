using KylyApi.Data;
using KylyApi.DTOs;
using Microsoft.EntityFrameworkCore;

namespace KylyApi.Services;

public class ProdutoService : IProdutoService
{
    // Quantidade de itens retornados por página.
    private const int TamanhoPagina = 15;

    private readonly AppDbContext _context;

    public ProdutoService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<PaginacaoResponse<ProdutoResponse>> BuscarAsync(string termo, int pagina)
    {
        // Remove espaços extras do termo de busca.
        var termoBusca = termo.Trim();

        //buscando o termo em qualquer posição da string.
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

                // Busca a prioridade do produto na lista de relevância.
                // A menor prioridade encontrada prevalece: 1 antes de 2.
                Prioridade = _context.ListasRelevancia
                    .Where(lista => lista.ProdutoId == produto.Id)
                    .Select(lista => (int?)lista.Prioridade)
                    .Min()
            });

        // Conta o total de registros que atendem ao filtro, antes da paginação.
        var totalRegistros = await consulta.CountAsync();

        var produtos = await consulta
            // Produtos sem prioridade recebem 3 e vem depois dos com prioridade 1 e 2.
            .OrderBy(item => item.Prioridade ?? 3)
            // Critério de desempate: ordena pelo Id do produto.
            .ThenBy(item => item.Produto.Id)
            // Aplica a paginação (pula os registros das páginas anteriores).
            .Skip((pagina - 1) * TamanhoPagina)
            // Pega apenas os itens da página atual.
            .Take(TamanhoPagina)
            // Projeção para o DTO de resposta.
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

        // Monta o objeto de resposta paginada.
        return new PaginacaoResponse<ProdutoResponse>
        {
            PaginaAtual = pagina,
            TamanhoPagina = TamanhoPagina,
            TotalRegistros = totalRegistros,
            TotalPaginas = (int)Math.Ceiling(totalRegistros / (double)TamanhoPagina),
            Itens = produtos
        };

        /*sql para validar no banco
        --busca pelo codigo
        select * from produtos a
        where upper(a.desc_produto) like upper('%tiara%')
        order by ((select prioridade from listas_relevancia b
        where a.id = b.produto_id), a.id)

        --busca por descricao do produto    
        select * from produtos a
        where a.codigo_produto = '702833'
        order by ((select prioridade from listas_relevancia b
        where a.id = b.produto_id), a.id) 
        */

    }
}