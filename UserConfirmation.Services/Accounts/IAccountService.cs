using Microsoft.AspNetCore.Identity;
using UserConfirmation.Shared.Models;

namespace UserConfirmation.Services.Accounts;
public interface IAccountService
{
    Task<IdentityResult> RegisterUserAsync(RegisterModel model);
    Task<SignInResult> LoginUserAsync(LoginModel model);
}
