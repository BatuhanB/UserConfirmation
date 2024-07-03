﻿using Microsoft.AspNetCore.Identity;
using UserConfirmation.Data.Models;
using UserConfirmation.Services.Confirmations;
using UserConfirmation.Shared.Models;

namespace UserConfirmation.Services.Accounts;
public class AccountService(UserManager<ApplicationUser> userManager,
    SignInManager<ApplicationUser> signInManager,
    IConfirmationService confirmationService) : IAccountService
{
    private readonly UserManager<ApplicationUser> _userManager = userManager;
    private readonly SignInManager<ApplicationUser> _signInManager = signInManager;
    private readonly IConfirmationService _confirmationService = confirmationService;

    public async Task<IdentityResult> RegisterUserAsync(RegisterModel model)
    {
        var user = new ApplicationUser { UserName = model.UserName, Email = model.Email };
        return await _userManager.CreateAsync(user, model.Password);
    }

    public async Task<SignInResult> LoginUserAsync(LoginModel model)
    {
        var user = await _userManager.FindByEmailAsync(model.Email);
        if (user != null)
        {
            var code = _confirmationService.SendConfirmationCode(user.Id);

            Thread.Sleep(1000);

            if (code is not null)
            {
                if (_confirmationService.ValidateConfirmationCodeAsync(user.Id, code).Result)
                {
                    var result = await _signInManager.PasswordSignInAsync(user, model.Password, false, false);

                    return result;
                }
            }
        }
        return SignInResult.Failed;
    }
}
