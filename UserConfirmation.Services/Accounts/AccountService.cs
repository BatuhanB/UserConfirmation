using Microsoft.AspNetCore.Identity;
using UserConfirmation.Data.Models;
using UserConfirmation.Services.Confirmations;
using UserConfirmation.Services.MessageQueue;
using UserConfirmation.Shared.Models;

namespace UserConfirmation.Services.Accounts;
public class AccountService : IAccountService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly IMessageQueueService _messageQueueService;
    private readonly IConfirmationService _confirmationService;

    public AccountService(UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager,
        IMessageQueueService messageQueueService,
        IConfirmationService confirmationService)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _messageQueueService = messageQueueService;
        _confirmationService = confirmationService;
    }

    public async Task<IdentityResult> RegisterUserAsync(RegisterModel model)
    {
        var code = _confirmationService.GenerateCode();
        var user = new ApplicationUser { UserName = model.UserName, Email = model.Email,ConfirmationCode = code };
        return await _userManager.CreateAsync(user, model.Password);
    }

    public async Task<SignInResult> LoginUserAsync(LoginModel model)
    {
        var user = await _userManager.FindByEmailAsync(model.Email);
        if (user != null)
        {
            var result = await _signInManager.PasswordSignInAsync(user, model.Password, false, false);
            if (result.Succeeded)
            {
                _messageQueueService.SendMessage(new ConfirmationRequest { UserId = user.Id });
            }
            return result;
        }
        return SignInResult.Failed;
    }
}
