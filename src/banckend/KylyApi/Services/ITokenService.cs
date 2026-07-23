using KylyApi.Models;

namespace KylyApi.Services;
// Interface para serviço de geração de tokens JWT.
public interface ITokenService
{
    string GerarToken(ApplicationUser usuario);
}
