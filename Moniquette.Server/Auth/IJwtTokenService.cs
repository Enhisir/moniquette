using Moniquette.Common.Models;

namespace Moniquette.Server.Auth;

public interface IJwtTokenService
{
    string CreateToken(Session session);

    JwtValidationResult ValidateToken(string token);
}
