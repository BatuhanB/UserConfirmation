using Microsoft.AspNetCore.Identity;
using UserConfirmation.Shared.Models;

namespace UserConfirmation.Services.Accounts;
public interface IAccountService
{
    Task<IdentityResult> RegisterUserAsync(RegisterModel model);
    Task<(string userId,string code)> LoginUserAsync(LoginModel model);
    Task<SignInResult> ConfirmUserAsync(string userId,string code);
}
