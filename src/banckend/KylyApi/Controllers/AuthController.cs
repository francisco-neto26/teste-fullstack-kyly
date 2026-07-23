using KylyApi.DTOs;
using KylyApi.Models;
using KylyApi.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace KylyApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ITokenService _tokenService;

    public AuthController(UserManager<ApplicationUser> userManager, ITokenService tokenService)
    {
        _userManager = userManager;
        _tokenService = tokenService;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterDto dto)
    {
        // 1. Verifica se o usuário já existe
        var userExists = await _userManager.FindByNameAsync(dto.Username);
        if (userExists != null)
            return BadRequest("Nome de usuário já está em uso.");

        // 2. Cria a nova instância do usuário
        var user = new ApplicationUser
        {
            UserName = dto.Username,
            Email = dto.Email,
            NomeCompleto = dto.NomeCompleto
        };

        // 3. O UserManager criptografa a senha com Hash + Salt automaticamente ao criar
        var result = await _userManager.CreateAsync(user, dto.Password);

        if (!result.Succeeded)
            return BadRequest(result.Errors);

        return Ok(new { message = "Usuário registrado com sucesso!" });
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginDto dto)
    {
        // 1. Procura o usuário pelo Username
        var user = await _userManager.FindByNameAsync(dto.Username);
        if (user == null)
            return Unauthorized("Usuário ou senha inválidos.");

        // 2. Valida a senha enviada contra o Hash armazenado no banco
        var isPasswordValid = await _userManager.CheckPasswordAsync(user, dto.Password);
        if (!isPasswordValid)
            return Unauthorized("Usuário ou senha inválidos.");

        // 3. Gera o Token JWT se a senha for válida
        var token = _tokenService.GerarToken(user);

        return Ok(new TokenResponseDto
        {
            Token = token,
            Expiracao = DateTime.UtcNow.AddHours(8)
        });
    }
}
