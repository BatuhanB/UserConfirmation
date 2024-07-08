using UserConfirmation.Data.Models;

namespace UserConfirmation.Services.Token;
public interface ITokenService
{
    Task<string> GenerateToken(ApplicationUser user);
}
