using Microsoft.AspNetCore.Identity;
using UserConfirmation.Data.Models;
using UserConfirmation.Services.CacheStore;
using UserConfirmation.Services.Confirmations;
using UserConfirmation.Services.MessageQueue;
using UserConfirmation.Shared.Models;

namespace UserConfirmation.Services.Accounts;
public class AccountService(UserManager<ApplicationUser> userManager,
    SignInManager<ApplicationUser> signInManager,
    IConfirmationService confirmationService,
    IMessageQueueService messageQueueService,
    ITempPasswordStore tempPasswordStore) : IAccountService
{
    private readonly UserManager<ApplicationUser> _userManager = userManager;
    private readonly SignInManager<ApplicationUser> _signInManager = signInManager;
    private readonly IConfirmationService _confirmationService = confirmationService;
    private readonly IMessageQueueService _messageQueueService = messageQueueService;
    private readonly ITempPasswordStore _tempPasswordStore = tempPasswordStore;

    public async Task<IdentityResult> RegisterUserAsync(RegisterModel model)
    {
        var user = new ApplicationUser { UserName = model.UserName, Email = model.Email };
        return await _userManager.CreateAsync(user, model.Password);
    }

    public async Task<string> LoginUserAsync(LoginModel model)
    {
        var user = await _userManager.FindByEmailAsync(model.Email);
        if (user != null)
        {
            var code = _confirmationService.SendConfirmationCode(user.Id);
            var callBackUrl = $"http://localhost:5141/api/account/confirm?userId={user.Id}&code={code}";
            _tempPasswordStore.StorePassword(user.Id, model.Password);

            return callBackUrl;
        }
        return string.Empty;
    }

    public async Task<SignInResult> ConfirmUserAsync(string userId, string code)
    {
        var user = await _userManager.FindByIdAsync(userId);

        if (user != null)
        {
            var queueResult = await _messageQueueService.RecieveMessage();

            if (_confirmationService.ValidateConfirmationCodeAsync(userId, code).Result)
            {
                var password = _tempPasswordStore.RetrievePassword(userId);
                if (password != null)
                {
                    return await _signInManager.PasswordSignInAsync(user.UserName, password, false, false);
                }
            }
        }
        return SignInResult.Failed;
    }
}