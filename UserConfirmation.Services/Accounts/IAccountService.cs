using Microsoft.AspNetCore.Identity;
using UserConfirmation.Shared.Models;

namespace UserConfirmation.Services.Accounts;
public interface IAccountService
{
    Task<IdentityResult> RegisterUserAsync(RegisterModel model);
    Task<string> LoginUserAsync(LoginModel model);
    Task<SignInResult> ConfirmUserAsync(string userId,string code);
}
