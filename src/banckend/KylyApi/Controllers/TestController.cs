using Microsoft.AspNetCore.Mvc;

//classe de teste para verificar se a API está online e funcionando corretamente
namespace KylyApi.Controllers;
//classe de teste para verificar se a API está online e funcionando corretamente
[ApiController]
[Route("api/[controller]")]
public class TestController : ControllerBase
{
    [HttpGet]
    public IActionResult GetStatus()
    {
        return Ok(new { status = "API Online", timestamp = DateTime.UtcNow });
    }
}