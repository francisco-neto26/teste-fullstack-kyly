using System.ComponentModel.DataAnnotations;

namespace KylyApi.DTOs;
// DTO para login de usuário, usado para receber dados do usuário na API.
public class LoginDto
{
    [Required(ErrorMessage = "O nome de usuário é obrigatório.")]
    public string Username { get; set; } = string.Empty;

    [Required(ErrorMessage = "A senha é obrigatória.")]
    public string Password { get; set; } = string.Empty;
}
