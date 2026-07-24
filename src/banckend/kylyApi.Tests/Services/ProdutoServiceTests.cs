using Xunit;
using Testcontainers.PostgreSql;
using Microsoft.EntityFrameworkCore;
using KylyApi.Data;
using KylyApi.Services;
using KylyApi.Models;
using FluentAssertions;
using System.Linq;
using System.Threading.Tasks;
public class ProdutoServiceTests : IAsyncLifetime
{
    private PostgreSqlContainer _postgres = null!;
    private AppDbContext _context = null!;
    private ProdutoService _service = null!;

    public async Task InitializeAsync()
    {
        _postgres = new PostgreSqlBuilder().Build();
        await _postgres.StartAsync();

        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseNpgsql(_postgres.GetConnectionString())
            .Options;

        _context = new AppDbContext(options);
        await _context.Database.MigrateAsync();

        // Seed mínimo: 3 produtos + listas de relevância
        await SeedDadosAsync(_context);

        _service = new ProdutoService(_context);
    }

    [Fact]
    public async Task Buscar_DevePriorizarLista1AntesDaLista2()
    {
        var resultado = await _service.BuscarAsync("CAMISETA", 1);

        resultado.Itens.First().CodigoProduto.Should().Be("105725"); // exemplo
    }

    public async Task DisposeAsync()
    {
        if (_context != null)
        {
            await _context.DisposeAsync();
        }
        if (_postgres != null)
        {
            await _postgres.DisposeAsync();
        }
    }

    private async Task SeedDadosAsync(AppDbContext context)
    {
        var p1 = new Produto
        {
            Id = "105725.1",
            CodigoProduto = "105725",
            DescProduto = "CAMISETA MASCULINA",
            CodigoCor = "0001",
            DescCor = "AZUL",
            CodigoTamanho = "P",
            DescTamanho = "P"
        };
        var p2 = new Produto
        {
            Id = "105726.1",
            CodigoProduto = "105726",
            DescProduto = "CAMISETA FEMININA",
            CodigoCor = "0002",
            DescCor = "VERMELHO",
            CodigoTamanho = "M",
            DescTamanho = "M"
        };
        var p3 = new Produto
        {
            Id = "105727.1",
            CodigoProduto = "105727",
            DescProduto = "CAMISETA UNISSEX",
            CodigoCor = "0003",
            DescCor = "PRETO",
            CodigoTamanho = "G",
            DescTamanho = "G"
        };
        await context.Produtos.AddRangeAsync(p1, p2, p3);
        await context.SaveChangesAsync();
        var lr1 = new ListaRelevancia
        {
            ProdutoId = p1.Id,
            CodigoProduto = p1.CodigoProduto,
            Prioridade = 1
        };
        var lr2 = new ListaRelevancia
        {
            ProdutoId = p2.Id,
            CodigoProduto = p2.CodigoProduto,
            Prioridade = 2
        };
        await context.ListasRelevancia.AddRangeAsync(lr1, lr2);
        await context.SaveChangesAsync();
    }

}