using System.ComponentModel.DataAnnotations;

// DTO para registro de usuário, usado para receber dados do usuário na API.
namespace KylyApi.DTOs;
// DTO para registro de usuário, usado para receber dados do usuário na API.
public class RegisterDto
{
    [Required]
    public string Username { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required]
    [MinLength(6, ErrorMessage = "A senha deve ter no mínimo 6 caracteres.")]
    public string Password { get; set; } = string.Empty;

    public string? NomeCompleto { get; set; }
}
