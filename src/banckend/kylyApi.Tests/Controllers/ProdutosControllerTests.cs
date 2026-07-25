using Xunit;
using Moq;
using KylyApi.Controllers;
using KylyApi.Services;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
// Testes unitários para o ProdutosController, verificando o comportamento do método Buscar em diferentes cenários.
public class ProdutosControllerTests
{
    [Fact]
    public async Task Buscar_SemTermo_RetornaBadRequest()
    {
        var service = new Mock<IProdutoService>();
        var controller = new ProdutosController(service.Object);

        var resultado = await controller.Buscar("", 1);

        var badRequest = Assert.IsType<BadRequestObjectResult>(resultado.Result);
        Assert.Contains("termo", badRequest.Value?.ToString(), StringComparison.OrdinalIgnoreCase);
    }
}