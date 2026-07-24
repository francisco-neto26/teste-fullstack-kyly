using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using KylyApi.Models;
using Microsoft.IdentityModel.Tokens;

namespace KylyApi.Services;

public class TokenService : ITokenService
{
    private readonly IConfiguration _config;

    public TokenService(IConfiguration config)
    {
        _config = config;
    }

    public string GerarToken(ApplicationUser usuario)
    {
        // Define os dados (claims) contidos dentro do Token JWT
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, usuario.Id),
            new Claim(ClaimTypes.Name, usuario.UserName ?? string.Empty),
            new Claim(ClaimTypes.Email, usuario.Email ?? string.Empty)
        };

        // Lê a chave que veio do .env via IConfiguration do .NET
        var secretKey = _config["JwtSettings:SecretKey"] 
            ?? throw new InvalidOperationException("A chave JWT_SECRET_KEY não foi encontrada no ambiente.");
        
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));

        // Define as credenciais de assinatura (Algoritmo HMAC SHA256)
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        // Configura as informações do Token
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddHours(8), // Expiração de 8 horas
            SigningCredentials = creds,
            Issuer = _config["JwtSettings:Issuer"],
            Audience = _config["JwtSettings:Audience"]
        };

        // Gera e retorna a string do Token JWT
        var tokenHandler = new JwtSecurityTokenHandler();
        var token = tokenHandler.CreateToken(tokenDescriptor);

        return tokenHandler.WriteToken(token);
    }
}
