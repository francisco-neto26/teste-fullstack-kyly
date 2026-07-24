namespace KylyApi.DTOs;
// DTO para resposta de token, usado para enviar o token JWT e sua expiração ao cliente.
public class TokenResponseDto
{
    public string Token { get; set; } = string.Empty;
    public DateTime Expiracao { get; set; }
}
