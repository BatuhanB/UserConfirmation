
using Microsoft.AspNetCore.Identity;
using UserConfirmation.Data.Models;

namespace UserConfirmation.Services.Confirmations;
public class ConfirmationService(UserManager<ApplicationUser> userManager) : IConfirmationService
{
    private readonly UserManager<ApplicationUser> _userManager = userManager;

    public async Task GenerateAndSendConfirmationCodeAsync(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user != null)
        {
            user.ConfirmationCode = GenerateCode();
            await _userManager.UpdateAsync(user);

            // Send confirmation code via email or other means
            SendConfirmationCode(user.Email, user.ConfirmationCode);
        }
    }

    private string GenerateCode()
    {
        // Implement your code generation logic here
        return new Random().Next(100000, 999999).ToString();
    }

    private void SendConfirmationCode(string email, string confirmationCode)
    {
        // Implement email sending logic here
    }
}
