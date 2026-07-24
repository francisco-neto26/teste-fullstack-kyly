using Microsoft.AspNetCore.Identity;

namespace KylyApi.Models;
//tabela de Usuários do sistema.
// Herda de IdentityUser para já ganhar campos como Email, Telefone e Hash de Senha de forma segura.
public class ApplicationUser : IdentityUser
{
    //inclui estes por fazer sentido e para exemplificar como adicionar campos extras a IdentityUser
    public string? NomeCompleto { get; set; }
    public DateTime DataCriacao { get; set; } = DateTime.UtcNow;
}
